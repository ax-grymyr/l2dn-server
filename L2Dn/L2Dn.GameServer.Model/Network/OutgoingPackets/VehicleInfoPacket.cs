﻿using L2Dn.GameServer.Model.Actor.Instances;
using L2Dn.Packets;

namespace L2Dn.GameServer.Network.OutgoingPackets;

/**
 * @author Maktakien
 */
public readonly struct VehicleInfoPacket: IOutgoingPacket
{
    private readonly int _objId;
    private readonly int _x;
    private readonly int _y;
    private readonly int _z;
    private readonly int _heading;
	
    public VehicleInfoPacket(Boat boat)
    {
        _objId = boat.ObjectId;
        _x = boat.getX();
        _y = boat.getY();
        _z = boat.getZ();
        _heading = boat.getHeading();
    }
	
    public void WriteContent(PacketBitWriter writer)
    {
        writer.WritePacketCode(OutgoingPacketCodes.VEHICLE_INFO);
        
        writer.WriteInt32(_objId);
        writer.WriteInt32(_x);
        writer.WriteInt32(_y);
        writer.WriteInt32(_z);
        writer.WriteInt32(_heading);
    }
}