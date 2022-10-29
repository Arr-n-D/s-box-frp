using System;
using System.Collections.Generic;
using fRP.Networking.Packets.Outbound;
// using fRP.Networking.Packets.Inbound;

namespace fRP.Networking.Packets
{
    public static class Mappings
    {
         public static readonly Dictionary<Type, ushort> TypeToId = new Dictionary<Type, ushort>
    {
        { typeof(AuthenticationPacket), 101 },
        // { typeof(RegisterMessage), 102 },
        // { typeof(HandshakeMessage), 1001 },
        // { typeof(ChatMessage), 1002 },
        // { typeof(MoveMessage), 1500 }
    };

    public static readonly Dictionary<ushort, Type> IdToType = new Dictionary<ushort, Type>
    {
        // { 0, typeof(Error) },
    };
    }
}