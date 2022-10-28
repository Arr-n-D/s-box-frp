using Sandbox;
using System.Threading.Tasks;
using fRP.Networking;
using System.Collections.Concurrent;
using fRP.Networking.Interfaces;
using System;
using System.Text.Json;
using fRP.Networking.Packets;
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
	private  bool outboundThreadStarted = false;
	private readonly ConcurrentQueue<IOutMessage> outgoingMessageQueue = new ConcurrentQueue<IOutMessage>();
	// private readonly ConcurrentQueue<IInMessage> incomingMessageQueue = new ConcurrentQueue<IInMessage>();
	public frpGame()
	{
		fRPCurrent = this;
		if ( IsServer )
		{
			InitializeOutboundThread();

			// while ( !outboundThreadStarted )
			// {
			// 	Wait.Sleep( 100 );
			// }

			wsClient = new( "ws://127.0.0.1:6001" );
			wsClient.InitializeConnection();
			DownloadAsset("gvar.citizen_zombie");
			
			_ = new fRPHud();
			
		}
		
	}

	public void SendMessage(IOutMessage message)
	{
		outgoingMessageQueue.Enqueue(message);
	}

	private void InitializeOutboundThread()
	{
		try
            {
				GameTask.RunInThreadAsync(ListenForOutboundMessages);
				outboundThreadStarted = true;
                // outboundThread = new Thread(new ThreadStart(ListenForOutboundMessages))
                // {
                //     IsBackground = true
                // };
                // outboundThread.Start();
            }
            catch (Exception e)
            {
                // Debug.LogException(e);
            }
	}

	private async void ListenForOutboundMessages()
	{
		 if (outgoingMessageQueue.TryDequeue(out IOutMessage message))
            {
                var type = message.GetType();
				Log.Info($"Sending message of type {type}");
				// var json = JsonSerializer.Serialize(message);
				// await wsClient.Send(json);
			// }
				// string jsonMessage = JsonSerializer.Serialize( message );
                var nMessage = JsonSerializer.Serialize(new Packet
                {
                    ID = Mappings.TypeToId[type],
                    Content = JsonSerializer.SerializeToUtf8Bytes(message)
                });

				await wsClient.Send(nMessage);
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

		cl.Pawn = player;
	}

	static async Task DownloadAsset( string packageName)
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
