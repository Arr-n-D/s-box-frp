using System.Collections.Concurrent;
using fRP.Networking.Interfaces;
using System;
using System.Text.Json;
using Sandbox;
using fRP.Networking.Packets;
using System.Threading.Tasks;

namespace fRP.Networking;
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

	public uint QueueMessage( IOutMessage message )
	{
		var type = message.GetType();

		string nMessage = JsonSerializer.Serialize( new Packet( Mappings.TypeToId[type], JsonSerializer.Serialize( message, type ), ++MessageIdAccumulator ) );

		outgoingMessageQueue.Enqueue( nMessage );

		return MessageIdAccumulator;
	}


	private async void HandleWrites()
	{
		for ( int i = 0; i < outgoingMessageQueue.Count; i++ )
		{
			outgoingMessageQueue.TryDequeue( out string message );
			await wsClient.ws.Send( message );
		}
	}

	public async Task<Packet> SendAndRetryMessage<T>( T message, int retries = 3, float timeout = 2f ) where T : IOutMessage
	{
		for ( var i = 0; i < retries; i++ )
		{
			var id = QueueMessage( message );
			var response = await this.wsClient.WaitForResponse( id, timeout );

			if ( response != null )
			{
				return response;
			}
		}
		return null;
	}

}
