using L2Dn.Packets;

namespace L2Dn.GameServer.Network.OutgoingPackets.PledgeV3;

public readonly struct ExPledgeDonationInfoPacket: IOutgoingPacket
{
    public void WriteContent(PacketBitWriter writer)
    {
        writer.WritePacketCode(OutgoingPacketCodes.EX_PLEDGE_DONATION_INFO);
        
        writer.WriteInt32(0);
        writer.WriteByte(1);
    }
}