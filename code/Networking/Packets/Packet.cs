using fRP.Networking.Interfaces;
using System.Text.Json.Serialization;

namespace fRP.Networking.Packets
{
    public struct Packet: IOutMessage, IInMessage
    {
        [JsonPropertyName( "ID" )]
        public ushort ID { get; set; }

        [JsonPropertyName( "Content" )]
        public string Content { get; set; }
    }
}