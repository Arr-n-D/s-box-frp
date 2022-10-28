using fRP.Networking.Interfaces;
using System.Text.Json.Serialization;

namespace fRP.Networking.Packets
{
    public struct Packet: IOutMessage
    {
        [JsonPropertyName( "ID" )]
        public ushort ID;

        [JsonPropertyName( "Content" )]
        public byte[] Content;
    }
}