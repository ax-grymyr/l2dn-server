using L2Dn.Packets;

namespace L2Dn.GameServer.Network.OutgoingPackets.Enchanting;

public readonly struct ExSetEnchantChallengePointPacket: IOutgoingPacket
{
    private readonly bool _result;
	
    public ExSetEnchantChallengePointPacket(bool result)
    {
        _result = result;
    }
	
    public void WriteContent(PacketBitWriter writer)
    {
        writer.WritePacketCode(OutgoingPacketCodes.EX_SET_ENCHANT_CHALLENGE_POINT);
        
        writer.WriteByte(_result);
    }
}