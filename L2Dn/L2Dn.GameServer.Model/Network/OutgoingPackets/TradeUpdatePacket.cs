using L2Dn.GameServer.Model;
using L2Dn.GameServer.Network.Enums;
using L2Dn.Packets;

namespace L2Dn.GameServer.Network.OutgoingPackets;

public readonly struct TradeUpdatePacket(TradeItem? item, long count, long newCount): IOutgoingPacket
{
    public void WriteContent(PacketBitWriter writer)
    {
        writer.WritePacketCode(OutgoingPacketCodes.TRADE_UPDATE);

        if (item is null)
        {
            writer.WriteByte(1); // sendType
            writer.WriteInt32(1);
        }
        else
        {
            writer.WriteByte(2); // sendType
            writer.WriteInt32(1);
            writer.WriteInt32(1);
            writer.WriteInt16(newCount > 0 && item.getItem().isStackable() ? (short)3 : (short)2);
            InventoryPacketHelper.WriteItem(writer, item, count);
        }
    }
}