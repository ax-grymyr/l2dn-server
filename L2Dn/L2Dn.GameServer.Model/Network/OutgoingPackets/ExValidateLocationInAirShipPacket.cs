﻿using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Utilities;
using L2Dn.Geometry;
using L2Dn.Packets;

namespace L2Dn.GameServer.Network.OutgoingPackets;

public readonly struct ExValidateLocationInAirShipPacket: IOutgoingPacket
{
    private readonly Player _player;
    private readonly int _shipId;
    private readonly Location _location;

    public ExValidateLocationInAirShipPacket(Player player)
    {
        _player = player;
        _shipId = _player.getAirShip()?.ObjectId ?? 0; // TODO: pass airShipId as parameter
        _location = new Location(player.getInVehiclePosition(), player.getHeading());
    }

    public void WriteContent(PacketBitWriter writer)
    {
        writer.WritePacketCode(OutgoingPacketCodes.EX_VALIDATE_LOCATION_IN_AIR_SHIP);

        writer.WriteInt32(_player.ObjectId);
        writer.WriteInt32(_shipId);
        writer.WriteLocation(_location);
    }
}