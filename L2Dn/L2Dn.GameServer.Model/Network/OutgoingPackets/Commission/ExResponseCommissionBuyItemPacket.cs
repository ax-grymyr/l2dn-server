using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Commission;
using L2Dn.Packets;

namespace L2Dn.GameServer.Network.OutgoingPackets.Commission;

public readonly struct ExResponseCommissionBuyItemPacket: IOutgoingPacket
{
    public static readonly ExResponseCommissionBuyItemPacket FAILED = new(null);

    private readonly CommissionItem? _commissionItem;

    public ExResponseCommissionBuyItemPacket(CommissionItem? commissionItem)
    {
        _commissionItem = commissionItem;
    }

    public void WriteContent(PacketBitWriter writer)
    {
        writer.WritePacketCode(OutgoingPacketCodes.EX_RESPONSE_COMMISSION_BUY_ITEM);

        writer.WriteInt32(_commissionItem != null);
        if (_commissionItem != null)
        {
            ItemInfo itemInfo = _commissionItem.getItemInfo();
            writer.WriteInt32(itemInfo.getEnchantLevel());
            writer.WriteInt32(itemInfo.getItem().Id);
            writer.WriteInt64(itemInfo.getCount());
        }
    }
}