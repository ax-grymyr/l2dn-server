using L2Dn.GameServer.Model.Actor.Instances;
using L2Dn.Packets;

namespace L2Dn.GameServer.Network.OutgoingPackets;

public readonly struct StaticObjectInfoPacket: IOutgoingPacket
{
    private readonly int _staticObjectId;
    private readonly int _objectId;
    private readonly int _type;
    private readonly bool _isTargetable;
    private readonly int _meshIndex;
    private readonly bool _isClosed;
    private readonly bool _isEnemy;
    private readonly int _maxHp;
    private readonly int _currentHp;
    private readonly bool _showHp;
    private readonly int _damageGrade;
	
    public StaticObjectInfoPacket(StaticObject staticObject)
    {
        _staticObjectId = staticObject.getId();
        _objectId = staticObject.ObjectId;
        _type = 0;
        _isTargetable = true;
        _meshIndex = staticObject.getMeshIndex();
        _isClosed = false;
        _isEnemy = false;
        _maxHp = 0;
        _currentHp = 0;
        _showHp = false;
        _damageGrade = 0;
    }
	
    public StaticObjectInfoPacket(Door door, bool targetable)
    {
        _staticObjectId = door.getId();
        _objectId = door.ObjectId;
        _type = 1;
        _isTargetable = door.isTargetable() || targetable;
        _meshIndex = door.getMeshIndex();
        _isClosed = !door.isOpen();
        _isEnemy = door.isEnemy();
        _maxHp = door.getMaxHp();
        _currentHp = (int) door.getCurrentHp();
        _showHp = door.isShowHp();
        _damageGrade = door.getDamage();
    }
	
    public void WriteContent(PacketBitWriter writer)
    {
        writer.WritePacketCode(OutgoingPacketCodes.STATIC_OBJECT);
        
        writer.WriteInt32(_staticObjectId);
        writer.WriteInt32(_objectId);
        writer.WriteInt32(_type);
        writer.WriteInt32(_isTargetable);
        writer.WriteInt32(_meshIndex);
        writer.WriteInt32(_isClosed);
        writer.WriteInt32(_isEnemy);
        writer.WriteInt32(_currentHp);
        writer.WriteInt32(_maxHp);
        writer.WriteInt32(_showHp);
        writer.WriteInt32(_damageGrade);
    }
}