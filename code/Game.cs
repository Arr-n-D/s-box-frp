using Sandbox;
using System.Threading.Tasks;
using fRP.Networking;
using System.Collections.Concurrent;
using fRP.Networking.Interfaces;
using System;
using System.Text.Json;
using fRP.Networking.Packets;
using fRP.Networking.Packets.Outbound;
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

	public static frpGame fRPCurrent { get; protected set; }
	private bool outboundThreadStarted = false;
	private readonly ConcurrentQueue<IOutMessage> outgoingMessageQueue = new();
	public frpGame()
	{
		fRPCurrent = this;
		if ( IsServer )
		{
			wsClient = new( "ws://127.0.0.1:6001" );
			// wsClient.InitializeConnection();
			GameTask.RunInThreadAsync( ListenForData );
			DownloadAsset( "gvar.citizen_zombie" );

			_ = new fRPHud();

		}

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

	public void SendMessage( IOutMessage message )
	{
		outgoingMessageQueue.Enqueue( message );
	}


	private async void HandleWrites()
	{
		if ( outgoingMessageQueue.TryDequeue( out IOutMessage message ) )
		{
			var type = message.GetType();
		
			string nMessage = JsonSerializer.Serialize( new Packet
			{
				ID = Mappings.TypeToId[type],
				Content = JsonSerializer.Serialize( message, type )
			} );

			// Log.Info( message  );
			
			await wsClient.Send( nMessage );
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
		this.SendMessage( new AuthenticationPacket
		{
			Token = "format"
		} );

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
