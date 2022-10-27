﻿using Sandbox;
using System.Threading.Tasks;
using fRP.Networking;
using System.Collections.Concurrent;
using fRP.Networking.Interfaces;
using System.Threading;
using System;
//
using System.Text.Json;
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
	 private Thread outboundThread;
	private readonly ConcurrentQueue<IOutMessage> outgoingMessageQueue = new ConcurrentQueue<IOutMessage>();
	// private readonly ConcurrentQueue<IInMessage> incomingMessageQueue = new ConcurrentQueue<IInMessage>();
	public frpGame()
	{
		if ( IsServer )
		{
			wsClient = new( "ws://127.0.0.1:6001" );
			wsClient.InitializeConnection();
			InitializeOutboundThread();
			DownloadAsset("gvar.citizen_zombie");
			
			_ = new fRPHud();
			
		}
		
	}

	private void InitializeOutboundThread()
	{
		try
            {
                outboundThread = new Thread(new ThreadStart(ListenForOutboundMessages))
                {
                    IsBackground = true
                };
                outboundThread.Start();
            }
            catch (Exception e)
            {
                // Debug.LogException(e);
            }
	}

	private async void ListenForOutboundMessages()
	{
		while (true)
		{
			if (outgoingMessageQueue.TryDequeue(out IOutMessage message))
			{
				string jsonMessage = JsonSerializer.Serialize( message );
				await wsClient.Send(jsonMessage);
			}
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
