using L2Dn.Packets;

namespace L2Dn.GameServer.Network.OutgoingPackets.PledgeV3;

public readonly struct ExPledgeLevelUpPacket: IOutgoingPacket
{
    private readonly int _level;
	
    public ExPledgeLevelUpPacket(int level)
    {
        _level = level;
    }
	
    public void WriteContent(PacketBitWriter writer)
    {
        writer.WritePacketCode(OutgoingPacketCodes.EX_PLEDGE_LEVEL_UP);
        
        writer.WriteInt32(_level);
    }
}