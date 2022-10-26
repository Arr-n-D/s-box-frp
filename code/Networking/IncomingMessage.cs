using System.Text.Json.Serialization;

namespace fRP.Networking
{
	public class IncomingMessage
	{
		[JsonPropertyName( "MessageType" )]
		public int MessageType { get; set; }

		[JsonPropertyName( "Message" )]
		public string Data { get; set; }
	}
}