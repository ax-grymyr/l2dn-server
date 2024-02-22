using L2Dn.Packets;

namespace L2Dn.GameServer.Network.OutgoingPackets;

public readonly struct PledgeSkillListAddPacket: IOutgoingPacket
{
    private readonly int _id;
    private readonly int _level;
	
    public PledgeSkillListAddPacket(int id, int level)
    {
        _id = id;
        _level = level;
    }
	
    public void WriteContent(PacketBitWriter writer)
    {
        writer.WritePacketCode(OutgoingPacketCodes.PLEDGE_SKILL_LIST_ADD);
        
        writer.WriteInt32(_id);
        writer.WriteInt32(_level);
    }
}