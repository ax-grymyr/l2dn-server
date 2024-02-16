using L2Dn.GameServer.Model.Actor;
using L2Dn.Packets;

namespace L2Dn.GameServer.Network.OutgoingPackets;

public readonly struct ServerObjectInfoPacket: IOutgoingPacket
{
    private readonly Npc _activeChar;
    private readonly int _x;
    private readonly int _y;
    private readonly int _z;
    private readonly int _heading;
    private readonly int _displayId;
    private readonly bool _isAttackable;
    private readonly double _collisionHeight;
    private readonly double _collisionRadius;
    private readonly string _name;

    public ServerObjectInfoPacket(Npc activeChar, Creature actor)
    {
        _activeChar = activeChar;
        _displayId = _activeChar.getTemplate().getDisplayId();
        _isAttackable = _activeChar.isAutoAttackable(actor);
        _collisionHeight = _activeChar.getCollisionHeight();
        _collisionRadius = _activeChar.getCollisionRadius();
        _x = _activeChar.getX();
        _y = _activeChar.getY();
        _z = _activeChar.getZ();
        _heading = _activeChar.getHeading();
        _name = _activeChar.getTemplate().isUsingServerSideName() ? _activeChar.getTemplate().getName() : "";
    }

    public void WriteContent(PacketBitWriter writer)
    {
        writer.WritePacketCode(OutgoingPacketCodes.SERVER_OBJECT_INFO);
        writer.WriteInt32(_activeChar.getObjectId());
        writer.WriteInt32(_displayId + 1000000);
        writer.WriteString(_name); // name
        writer.WriteInt32(_isAttackable);
        writer.WriteInt32(_x);
        writer.WriteInt32(_y);
        writer.WriteInt32(_z);
        writer.WriteInt32(_heading);
        writer.WriteDouble(1.0); // movement multiplier
        writer.WriteDouble(1.0); // attack speed multiplier
        writer.WriteDouble(_collisionRadius);
        writer.WriteDouble(_collisionHeight);
        writer.WriteInt32((int)(_isAttackable ? _activeChar.getCurrentHp() : 0));
        writer.WriteInt32(_isAttackable ? _activeChar.getMaxHp() : 0);
        writer.WriteInt32(1); // object type
        writer.WriteInt32(0); // special effects
    }
}