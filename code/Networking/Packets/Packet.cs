using fRP.Networking.Interfaces;
using System.Text.Json.Serialization;
using Sandbox;

namespace fRP.Networking.Packets;
public class Packet : IOutMessage, IInMessage
{
	public ushort ID { get; set; }
	public string Content { get; set; }
	public uint MessageID { get; set; }
	public TimeSince TimeSinceReceived;
}