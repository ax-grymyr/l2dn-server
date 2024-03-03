using L2Dn.Packets;

namespace L2Dn.GameServer.Network.OutgoingPackets.Balok;

public readonly struct BalrogWarGetRewardPacket: IOutgoingPacket
{
    private readonly bool _available;
	
    public BalrogWarGetRewardPacket(bool available)
    {
        _available = available;
    }
	
    public void WriteContent(PacketBitWriter writer)
    {
        writer.WritePacketCode(OutgoingPacketCodes.EX_BALROGWAR_GET_REWARD);
        
        writer.WriteByte(_available);
    }
}