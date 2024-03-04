using L2Dn.Packets;

namespace L2Dn.GameServer.Network.OutgoingPackets;

public readonly struct ExNotifyPremiumItemPacket: IOutgoingPacket
{
    public static readonly ExNotifyPremiumItemPacket STATIC_PACKET = new();

    public void WriteContent(PacketBitWriter writer)
    {
        writer.WritePacketCode(OutgoingPacketCodes.EX_NOTIFY_PREMIUM_ITEM);
    }
}