using L2Dn.GameServer.Enums;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Interfaces;
using L2Dn.Packets;

namespace L2Dn.GameServer.Network.OutgoingPackets;

public readonly struct FlyToLocationPacket: IOutgoingPacket
{
    private readonly int _destX;
    private readonly int _destY;
    private readonly int _destZ;
    private readonly int _chaObjId;
    private readonly int _orgX;
    private readonly int _orgY;
    private readonly int _orgZ;
    private readonly FlyType _type;
    private readonly int _flySpeed;
    private readonly int _flyDelay;
    private readonly int _animationSpeed;

    public FlyToLocationPacket(Creature creature, int destX, int destY, int destZ, FlyType type)
    {
        _chaObjId = creature.getObjectId();
        _orgX = creature.getX();
        _orgY = creature.getY();
        _orgZ = creature.getZ();
        _destX = destX;
        _destY = destY;
        _destZ = destZ;
        _type = type;
        if (creature.isPlayer())
        {
            creature.getActingPlayer().setBlinkActive(true);
        }
    }

    public FlyToLocationPacket(Creature creature, int destX, int destY, int destZ, FlyType type, int flySpeed,
        int flyDelay, int animationSpeed)
    {
        _chaObjId = creature.getObjectId();
        _orgX = creature.getX();
        _orgY = creature.getY();
        _orgZ = creature.getZ();
        _destX = destX;
        _destY = destY;
        _destZ = destZ;
        _type = type;
        _flySpeed = flySpeed;
        _flyDelay = flyDelay;
        _animationSpeed = animationSpeed;
        if (creature.isPlayer())
        {
            creature.getActingPlayer().setBlinkActive(true);
        }
    }

    public FlyToLocationPacket(Creature creature, ILocational dest, FlyType type)
        : this(creature, dest.getX(), dest.getY(), dest.getZ(), type)
    {
    }

    public FlyToLocationPacket(Creature creature, ILocational dest, FlyType type, int flySpeed, int flyDelay,
        int animationSpeed)
        : this(creature, dest.getX(), dest.getY(), dest.getZ(), type, flySpeed, flyDelay, animationSpeed)
    {
    }

    public void WriteContent(PacketBitWriter writer)
    {
        writer.WritePacketCode(OutgoingPacketCodes.FLY_TO_LOCATION);
        
        writer.WriteInt32(_chaObjId);
        writer.WriteInt32(_destX);
        writer.WriteInt32(_destY);
        writer.WriteInt32(_destZ);
        writer.WriteInt32(_orgX);
        writer.WriteInt32(_orgY);
        writer.WriteInt32(_orgZ);
        writer.WriteInt32((int)_type);
        writer.WriteInt32(_flySpeed);
        writer.WriteInt32(_flyDelay);
        writer.WriteInt32(_animationSpeed);
    }
}