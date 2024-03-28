using L2Dn.Packets;

namespace L2Dn.GameServer.Network.OutgoingPackets;

public readonly struct MagicSkillCanceledPacket: IOutgoingPacket
{
    private readonly int _objectId;
	
    public MagicSkillCanceledPacket(int objectId)
    {
        _objectId = objectId;
    }
	
    public void WriteContent(PacketBitWriter writer)
    {
        writer.WritePacketCode(OutgoingPacketCodes.MAGIC_SKILL_CANCELED);
        
        writer.WriteInt32(_objectId);
    }
}