using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Network.Enums;
using L2Dn.Packets;

namespace L2Dn.GameServer.Network.OutgoingPackets;

public readonly struct TradeUpdatePacket: IOutgoingPacket
{
    private readonly int _sendType;
    private readonly TradeItem? _item;
    private readonly long _newCount;
    private readonly long _count;

    public TradeUpdatePacket(int sendType, Player? player, TradeItem? item, long count)
    {
        _sendType = sendType;
        _count = count;
        _item = item;
        _newCount = player == null || item == null ? 0 : player.getInventory().getItemByObjectId(item.getObjectId())?.getCount() ?? 0 - item.getCount();
    }

    public void WriteContent(PacketBitWriter writer)
    {
        writer.WritePacketCode(OutgoingPacketCodes.TRADE_UPDATE);

        writer.WriteByte((byte)_sendType);
        writer.WriteInt32(1);
        if (_sendType == 2)
        {
            writer.WriteInt32(1);
            writer.WriteInt16((_newCount > 0) && _item.getItem().isStackable() ? (short)3 : (short)2);
            InventoryPacketHelper.WriteItem(writer, _item, _count);
        }
    }
}