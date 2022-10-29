using System.IO.Enumeration;
using fRP.Networking.Interfaces;
using System.Text.Json.Serialization;
namespace fRP.Networking.Packets.Outbound
{
    public struct AuthenticationPacket : IOutMessage
    {
        [JsonPropertyName( "Token" )]
		public string Token { get; set; }
    }   
}