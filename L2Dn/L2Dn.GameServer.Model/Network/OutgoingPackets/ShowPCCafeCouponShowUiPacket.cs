using L2Dn.Packets;

namespace L2Dn.GameServer.Network.OutgoingPackets;

public readonly struct ShowPCCafeCouponShowUiPacket: IOutgoingPacket
{
    public static readonly ShowPCCafeCouponShowUiPacket STATIC_PACKET = new();

    public void WriteContent(PacketBitWriter writer)
    {
        writer.WritePacketCode(OutgoingPacketCodes.SHOW_PCCAFE_COUPON_SHOW_UI);
    }
}