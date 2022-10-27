using System.Text.Json.Serialization;
using fRP.Networking.Interfaces;

namespace fRP.Networking
{
	public class OutgoingMessage : IOutMessage
	{
		[JsonPropertyName( "MessageType" )]
		public int MessageType { get; set; }

		// [JsonPropertyName( "IsServer" )]
		// public bool IsServer { get; set; }

		// [JsonPropertyName( "ServerId" )]
		// public string ServerId { get; set; }

		// [JsonPropertyName( "PlayerId" )]
		// public string PlayerId { get; set; }

		// [JsonPropertyName( "PlayerName" )]
		// public string PlayerName { get; set; }

		// [JsonPropertyName( "X-Auth-Token" )]
		// public string Token { get; set; }

		[JsonPropertyName( "Message" )]
		public byte[] Data { get; set; }
	}
}