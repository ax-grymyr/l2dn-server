using L2Dn.Packets;

namespace L2Dn.GameServer.Network.OutgoingPackets;

public readonly struct JoinPledgePacket: IOutgoingPacket
{
    private readonly int _pledgeId;
	
    public JoinPledgePacket(int pledgeId)
    {
        _pledgeId = pledgeId;
    }
	
    public void WriteContent(PacketBitWriter writer)
    {
        writer.WritePacketCode(OutgoingPacketCodes.JOIN_PLEDGE);
        
        writer.WriteInt32(_pledgeId);
    }
}