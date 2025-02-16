using L2Dn.GameServer.Model.Actor.Instances;
using L2Dn.Packets;

namespace L2Dn.GameServer.Network.OutgoingPackets;

public readonly struct ExAirShipInfoPacket: IOutgoingPacket
{
    // store some parameters, because they can be changed during broadcast
    private readonly AirShip _ship;
    private readonly int _x;
    private readonly int _y;
    private readonly int _z;
    private readonly int _heading;
    private readonly int _moveSpeed;
    private readonly int _rotationSpeed;
    private readonly int _captain;
    private readonly int _helm;

    public ExAirShipInfoPacket(AirShip ship)
    {
        _ship = ship;
        _x = ship.getX();
        _y = ship.getY();
        _z = ship.getZ();
        _heading = ship.getHeading();
        _moveSpeed = (int)ship.getStat().getMoveSpeed();
        _rotationSpeed = (int)ship.getStat().getRotationSpeed();
        _captain = ship.getCaptainId();
        _helm = ship.getHelmObjectId();
    }

    public void WriteContent(PacketBitWriter writer)
    {
        writer.WritePacketCode(OutgoingPacketCodes.EX_AIR_SHIP_INFO);
        
        writer.WriteInt32(_ship.ObjectId);
        writer.WriteInt32(_x);
        writer.WriteInt32(_y);
        writer.WriteInt32(_z);
        writer.WriteInt32(_heading);
        writer.WriteInt32(_captain);
        writer.WriteInt32(_moveSpeed);
        writer.WriteInt32(_rotationSpeed);
        writer.WriteInt32(_helm);
        if (_helm != 0)
        {
            // TODO: unhardcode these!
            writer.WriteInt32(0x16e); // Controller X
            writer.WriteInt32(0x00); // Controller Y
            writer.WriteInt32(0x6b); // Controller Z
            writer.WriteInt32(0x15c); // Captain X
            writer.WriteInt32(0x00); // Captain Y
            writer.WriteInt32(0x69); // Captain Z
        }
        else
        {
            writer.WriteInt32(0);
            writer.WriteInt32(0);
            writer.WriteInt32(0);
            writer.WriteInt32(0);
            writer.WriteInt32(0);
            writer.WriteInt32(0);
        }

        writer.WriteInt32(_ship.getFuel());
        writer.WriteInt32(_ship.getMaxFuel());
    }
}