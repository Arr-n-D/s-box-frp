using Sandbox;
using System.Text.Json;
using System.Threading.Tasks;
using fRP.Networking.Interfaces;
using fRP.Networking.Packets;
using System.Collections.Generic;
using System.Linq;
namespace fRP.Networking;
/// <summary>
/// A basic WebSocketClient for sbox.
/// </summary>
public class WebSocketClient
{
	/// Declare our WebSocket object.
	public readonly WebSocket ws;

	/// Declare our connection string.
	private readonly string connectionString;


	private static List<Packet> Responses = new();

	/// <summary>
	/// Constructor for our WebSocketClient.
	/// </summary>
	/// <param name="connectionString">
	/// The connection string this client will use to connect.
	/// </param>
	public WebSocketClient( string connectionString )
	{
		ws = new();
		this.connectionString = connectionString;

		ws.OnMessageReceived += OnMessageReceived;
		ws.OnDisconnected += OnDisconnected;
	}


	/// <summary>
	/// Connect to the WebSocket Server.
	/// </summary>
	/// <returns>Whether connection was successful.</returns>
	public async Task<bool> Connect()
	{
		await ws.Connect( connectionString );
		return ws.IsConnected;
	}

	/// <summary>
	/// Disconnect from the WebSocket Server.
	/// </summary>
	/// <returns>Whether we successfully disconnected from the server.</returns>
	public bool Disconnect()
	{
		ws.Dispose();
		return !ws.IsConnected;
	}

	/// <summary>
	/// Sends a message to the WebSocket Server.
	/// </summary>
	/// <param name="message">The message to be sent.</param>
	public async Task Send( string message )
	{
		await ws.Send( message );
	}

	/// <summary>
	/// Handler for received messages.
	/// </summary>
	/// <param name="jsonMessage">
	/// The message received from the WebSocket Server.
	/// </param>
	private void OnMessageReceived( string jsonMessage )
	{
		try
		{
			var msg = JsonSerializer.Deserialize<Packet>( jsonMessage );
			msg.TimeSinceReceived = 0;
			Responses.Add( msg );
		}
		catch ( System.Exception e )
		{
			Log.Warning( e.Message );
		}
	}

	[Event.Tick]
	public static void OnTick()
	{
		for ( int i = Responses.Count - 1; i >= 0; i-- )
		{
			if ( (Responses[i]?.TimeSinceReceived ?? 0) > 7f )
			{
				Responses.RemoveAt( i );
			}
		}

	}

	/// <summary>
	/// Handler for when the client disconnects from the WebSocket Server.
	/// </summary>
	private void OnDisconnected( int status, string reason )
	{
		Log.Info( $"Disconnected from WebSocket Server with exit code {status} and reason {reason}." );
	}

	public async Task InitializeConnection()
	{
		Log.Info( "Connecting to WebSocket Server..." );
		bool connected = await this.Connect();
		if ( connected )
		{
			Log.Info( "Successfully connected to the WebSocket Server" );
		}
		else
		{
			Log.Info( "Failed to connect to the WebSocket Server" );
		}
	}

	public async Task<Packet> WaitForResponse( uint messageid, float timeout = 2f )
	{
		RealTimeUntil tu = timeout;
		while ( tu > 0 )
		{
			var response = Responses.FirstOrDefault( x => x.MessageID == messageid );
			if ( response != null ) return response;

			await GameTask.Yield();
		}
		
		return null;
	}

}

