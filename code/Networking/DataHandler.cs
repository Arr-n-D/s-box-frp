using System.Collections.Concurrent;
using fRP.Networking.Interfaces;
using System;
using System.Text.Json;
using Sandbox;
using fRP.Networking.Packets;

namespace fRP.Networking
{
	public class DataHandler
	{
		public readonly WebSocketClient wsClient;

		private static uint MessageIdAccumulator;
		private readonly ConcurrentQueue<string> outgoingMessageQueue = new();

		public DataHandler()
		{
			wsClient = new( "ws://127.0.0.1:6001" );
		}
		public async void ListenForData()
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

		public Packet SendAndRetryMessage( IOutMessage message, int counter = 0 )
		{
			if ( counter >= 3 )
			{
				return null;
			} else {
				uint msgId = SendMessage( message );
				var response = this.wsClient.WaitForResponse( msgId ).GetAwaiter().GetResult();

				if ( response == null )
				{
					SendAndRetryMessage( message, counter + 1 );
				}

				return response;
			}
		}

	}
}