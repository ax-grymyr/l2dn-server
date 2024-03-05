using L2Dn.Packets;

namespace L2Dn.GameServer.Network.OutgoingPackets.Enchanting;

public readonly struct ExResetEnchantChallengePointPacket: IOutgoingPacket
{
    private readonly bool _result;
	
    public ExResetEnchantChallengePointPacket(bool result)
    {
        _result = result;
    }
	
    public void WriteContent(PacketBitWriter writer)
    {
        writer.WritePacketCode(OutgoingPacketCodes.EX_RESET_ENCHANT_CHALLENGE_POINT);
        
        writer.WriteByte(_result);
    }
}