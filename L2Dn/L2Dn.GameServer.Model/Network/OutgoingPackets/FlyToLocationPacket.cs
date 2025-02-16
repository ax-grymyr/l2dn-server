using L2Dn.GameServer.Enums;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Utilities;
using L2Dn.Geometry;
using L2Dn.Packets;

namespace L2Dn.GameServer.Network.OutgoingPackets;

public readonly struct FlyToLocationPacket: IOutgoingPacket
{
    private readonly Location3D _destination;
    private readonly int _objectId;
    private readonly Location3D _origin;
    private readonly FlyType _type;
    private readonly int _flySpeed;
    private readonly int _flyDelay;
    private readonly int _animationSpeed;

    public FlyToLocationPacket(Creature creature, Location3D destination, FlyType type)
    {
        _objectId = creature.ObjectId;
        _origin = new Location3D(creature.getX(), creature.getY(), creature.getZ());
        _destination = destination;
        _type = type;
        if (creature.isPlayer())
        {
            creature.getActingPlayer().setBlinkActive(true);
        }
    }

    public FlyToLocationPacket(Creature creature, Location3D destination, FlyType type, int flySpeed, int flyDelay,
        int animationSpeed)
    {
        _objectId = creature.ObjectId;
        _origin = new Location3D(creature.getX(), creature.getY(), creature.getZ());
        _destination = destination;
        _type = type;
        _flySpeed = flySpeed;
        _flyDelay = flyDelay;
        _animationSpeed = animationSpeed;
        if (creature.isPlayer())
        {
            creature.getActingPlayer().setBlinkActive(true);
        }
    }

    public void WriteContent(PacketBitWriter writer)
    {
        writer.WritePacketCode(OutgoingPacketCodes.FLY_TO_LOCATION);
        
        writer.WriteInt32(_objectId);
        writer.WriteLocation3D(_destination);
        writer.WriteLocation3D(_origin);
        writer.WriteInt32((int)_type);
        writer.WriteInt32(_flySpeed);
        writer.WriteInt32(_flyDelay);
        writer.WriteInt32(_animationSpeed);
    }
}