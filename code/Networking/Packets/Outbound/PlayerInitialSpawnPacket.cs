using fRP.Networking.Interfaces;
using System.Text.Json.Serialization;

namespace fRP.Networking.Packets.Outbound
{
    public class PlayerInitialSpawnPacket : IOutMessage
    {
        [JsonPropertyName( "SteamId" )]
		public string SteamId { get; set; }
    }
}