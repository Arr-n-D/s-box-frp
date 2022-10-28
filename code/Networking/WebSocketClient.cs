using Sandbox;
using System.Text.Json;
using System.Threading.Tasks;
using fRP.Networking.Interfaces;
namespace fRP.Networking
{
	/// <summary>
	/// A basic WebSocketClient for sbox.
	/// </summary>
	public class WebSocketClient
	{
		/// Declare our WebSocket object.
		private readonly WebSocket ws;

		/// Declare our connection string.
		private readonly string connectionString;

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
			IncomingMessage message = JsonSerializer.Deserialize<IncomingMessage>( jsonMessage );

			// // Token
			// if ( message.MessageType == 0 )
			// {
			// 	TokenWrapper token = new();
			// 	token.Token = message.Text;
			// 	TokenManager.SaveToken( token );
			// }
			// Results of an information query
			// else if ( message.MessageType == 1 )
			// {
				Log.Info( message.Data );
			// }

		}

		/// <summary>
		/// Handler for when the client disconnects from the WebSocket Server.
		/// </summary>
		private void OnDisconnected( int status, string reason )
		{
			Log.Info( $"Disconnected from WebSocket Server with exit code {status} and reason {reason}." );
		}

		public async void InitializeConnection()
		{	
			Log.Info( "Connecting to WebSocket Server..." );
			bool connected = await this.Connect();
			if ( connected )
			{
				Log.Info( "Successfully connected to the WebSocket Server" );
			} else {
				Log.Info( "Failed to connect to the WebSocket Server" );
			}

			Log.Info( $"{Host.Name}: We are connected." );

			// Attempt to authenticate to WS Server.
			OutgoingMessage message = new();
			message.MessageType = 0;

			// if ( Game.Current.IsServer )
			// {
			// 	message.ServerId = "dev_server";
			// 	message.IsServer = true;
			// }
			// else
			// {
			// 	message.PlayerId = Local.PlayerId.ToString();
			// 	message.PlayerName = Local.DisplayName;
			// }

			// // message.Token = TokenManager.GetToken();

			// await this.Send( message );
		}
	}
	
}