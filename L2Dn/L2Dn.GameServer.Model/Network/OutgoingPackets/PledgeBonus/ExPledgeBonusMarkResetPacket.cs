using L2Dn.Packets;

namespace L2Dn.GameServer.Network.OutgoingPackets.PledgeBonus;

public readonly struct ExPledgeBonusMarkResetPacket: IOutgoingPacket
{
    public static readonly ExPledgeBonusMarkResetPacket STATIC_PACKET = default;
	
    public void WriteContent(PacketBitWriter writer)
    {
        writer.WritePacketCode(OutgoingPacketCodes.EX_PLEDGE_BONUS_MARK_RESET);
    }
}