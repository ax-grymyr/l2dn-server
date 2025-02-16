﻿using L2Dn.GameServer.Model;
using L2Dn.GameServer.Utilities;
using L2Dn.Geometry;
using L2Dn.Packets;

namespace L2Dn.GameServer.Network.OutgoingPackets;

public readonly struct ValidateLocationPacket: IOutgoingPacket
{
    private readonly int _objectId;
    private readonly Location _location;
	
    public ValidateLocationPacket(WorldObject obj)
    {
        _objectId = obj.ObjectId;
        _location = obj.Location;
    }
	
    public void WriteContent(PacketBitWriter writer)
    {
        writer.WritePacketCode(OutgoingPacketCodes.VALIDATE_LOCATION);

        writer.WriteInt32(_objectId);
        writer.WriteLocation(_location);
        writer.WriteByte(0xff); // TODO: Find me!
    }
}