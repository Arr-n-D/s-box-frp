using System;
using System.Collections.Generic;
using fRP.Networking.Packets.Outbound;
using fRP.Networking.Errors;
// using fRP.Networking.Packets.Inbound;

namespace fRP.Networking.Packets;
public static class Mappings
{
	public static readonly Dictionary<Type, ushort> TypeToId = new Dictionary<Type, ushort>
		{
			{ typeof(PlayerInitialSpawnPacket), 1 },
		};

	public static readonly Dictionary<ushort, Type> IdToType = new Dictionary<ushort, Type>
	{
		// { 1, typeof(Error) },
	};

	public static readonly Dictionary<ushort, Type> IdToErrorType = new Dictionary<ushort, Type>
	{
		{ 1, typeof(PlayerNotFoundError) },
	};
	
}