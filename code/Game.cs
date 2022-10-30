using Sandbox;
using System.Threading.Tasks;
using fRP.Networking;
using System.Collections.Concurrent;
using fRP.Networking.Interfaces;
using System;
using System.Text.Json;
using fRP.Networking.Packets;
using fRP.Networking.Packets.Outbound;
using Sandbox.Internal;
// You don't need to put things in a namespace, but it doesn't hurt.
//
namespace fRP;

/// <summary>
/// This is your game class. This is an entity that is created serverside when
/// the game starts, and is replicated to the client. 
/// 
/// You can use this to create things like HUDs and declare which player class
/// to use for spawned players.
/// </summary>
public partial class frpGame : Game
{
	private readonly WebSocketClient wsClient;

	private static uint MessageIdAccumulator;

	public static frpGame fRPCurrent { get; protected set; }
	// private bool outboundThreadStarted = false;
	private readonly ConcurrentQueue<string> outgoingMessageQueue = new();
	public frpGame()
	{
		fRPCurrent = this;
		if ( IsServer )
		{
			wsClient = new( "ws://127.0.0.1:6001" );
			GameTask.RunInThreadAsync( ListenForData );
			DownloadAssets();

			// _ = new fRPHud();

		}
	}

	private void DownloadAssets()
	{
		DownloadAsset( "gvar.citizen_zombie" );
	}

	private async void ListenForData()
	{
		try
		{
			await wsClient.InitializeConnection();
			while ( wsClient.ws.IsConnected )
			{
				HandleWrites();
				await GameTask.Yield();
			}
			Log.Error( "Disconnected" );
		}
		catch ( Exception e )
		{
			Log.Error( e );
		}
		finally
		{
			wsClient.ws?.Dispose();
		}
	}

	public uint SendMessage( IOutMessage message )
	{
		var type = message.GetType();

		string nMessage = JsonSerializer.Serialize( new Packet
		{
			ID = Mappings.TypeToId[type],
			Content = JsonSerializer.Serialize( message, type ),
			MessageID = ++MessageIdAccumulator
		} );

		outgoingMessageQueue.Enqueue( nMessage );

		return MessageIdAccumulator;
	}


	private async void HandleWrites()
	{
		if ( outgoingMessageQueue.TryDequeue( out string message ) )
		{
			await wsClient.Send( message );
		}

	}

	/// <summary>
	/// A client has joined the server. Make them a pawn to play with
	/// </summary>
	public override void ClientJoined( Client cl )
	{
		base.ClientJoined( cl );
		var player = new Player( cl );
		player.Respawn();

		uint msgId = this.SendMessage( new PlayerInitialSpawnPacket
		{
			SteamId = cl.PlayerId.ToString()
		} );

		Log.Info(msgId);
		var response = wsClient.WaitForResponse( msgId ).GetAwaiter().GetResult();

		if ( response == null )
		{
			Log.Error( $"WebSocket response failed:" );
			// return default;
		}

		Log.Info( response.Content );

		cl.Pawn = player;
	}

	static async Task DownloadAsset( string packageName )
	{
		var package = await Package.Fetch( packageName, false );
		if ( package == null || package.PackageType != Package.Type.Model || package.Revision == null )
		{
			// spawn error particles
			return;
		}

		var model = package.GetMeta( "PrimaryAsset", "models/dev/error.vmdl" );
		// downloads if not downloads, mounts if not mounted
		await package.MountAsync();

		Precache.Add( model );
	}
}
