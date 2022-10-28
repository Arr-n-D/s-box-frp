using System.IO.Enumeration;
using fRP.Networking.Interfaces;
using System.Text.Json.Serialization;
namespace fRP.Networking.Packets
{
    public class AuthenticationPacket : IOutMessage
    {
        [JsonPropertyName( "X-Auth-Token" )]
		public string Token { get; set; }
    }   
}