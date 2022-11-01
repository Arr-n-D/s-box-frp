using System;
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

	public Packet( ushort ID, string Content, uint MessageID )
	{
		this.ID = ID == 0 ? throw new ArgumentNullException( nameof( ID ) ) : ID;
		this.Content = Content ?? throw new ArgumentNullException( nameof( Content ) );
		this.MessageID = MessageID == 0 ? throw new ArgumentNullException( nameof( MessageID ) ) : MessageID;
	}
}