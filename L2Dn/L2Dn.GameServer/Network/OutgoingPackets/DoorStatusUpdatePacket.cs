using L2Dn.GameServer.Model.Actor.Instances;
using L2Dn.Packets;

namespace L2Dn.GameServer.Network.OutgoingPackets;

public readonly struct DoorStatusUpdatePacket: IOutgoingPacket
{
    private readonly Door _door;
	
    public DoorStatusUpdatePacket(Door door)
    {
        _door = door;
    }
	
    public void WriteContent(PacketBitWriter writer)
    {
        writer.WritePacketCode(OutgoingPacketCodes.DOOR_STATUS_UPDATE);
        
        writer.WriteInt32(_door.getObjectId());
        writer.WriteInt32(!_door.isOpen());
        writer.WriteInt32(_door.getDamage());
        writer.WriteInt32(_door.isEnemy());
        writer.WriteInt32(_door.getId());
        writer.WriteInt32((int) _door.getCurrentHp());
        writer.WriteInt32(_door.getMaxHp());
    }
}