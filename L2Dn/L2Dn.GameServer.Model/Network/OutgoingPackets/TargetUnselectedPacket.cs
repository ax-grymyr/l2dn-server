using L2Dn.GameServer.Model.Actor;
using L2Dn.Packets;

namespace L2Dn.GameServer.Network.OutgoingPackets;

public readonly struct TargetUnselectedPacket: IOutgoingPacket
{
    private readonly int _targetObjId;
    private readonly int _x;
    private readonly int _y;
    private readonly int _z;
	
    /**
     * @param creature
     */
    public TargetUnselectedPacket(Creature creature)
    {
        _targetObjId = creature.ObjectId;
        _x = creature.getX();
        _y = creature.getY();
        _z = creature.getZ();
    }
	
    public void WriteContent(PacketBitWriter writer)
    {
        writer.WritePacketCode(OutgoingPacketCodes.TARGET_UNSELECTED);
        
        writer.WriteInt32(_targetObjId);
        writer.WriteInt32(_x);
        writer.WriteInt32(_y);
        writer.WriteInt32(_z);
        writer.WriteInt32(0); // ?
    }
}