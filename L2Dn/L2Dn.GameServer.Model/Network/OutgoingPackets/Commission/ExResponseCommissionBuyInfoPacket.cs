using L2Dn.GameServer.Model.Commission;
using L2Dn.GameServer.Network.Enums;
using L2Dn.Packets;

namespace L2Dn.GameServer.Network.OutgoingPackets.Commission;

public readonly struct ExResponseCommissionBuyInfoPacket: IOutgoingPacket
{
    public static readonly ExResponseCommissionBuyInfoPacket FAILED = default;
	
    private readonly CommissionItem _commissionItem;
	
    public ExResponseCommissionBuyInfoPacket(CommissionItem commissionItem)
    {
        _commissionItem = commissionItem;
    }

    public void WriteContent(PacketBitWriter writer)
    {
        writer.WritePacketCode(OutgoingPacketCodes.EX_RESPONSE_COMMISSION_BUY_INFO);
        
        writer.WriteInt32(_commissionItem != null);
        if (_commissionItem != null)
        {
            writer.WriteInt64(_commissionItem.getPricePerUnit());
            writer.WriteInt64(_commissionItem.getCommissionId());
            writer.WriteInt32(0); // CommissionItemType seems client does not really need it.
            InventoryPacketHelper.WriteItem(writer, _commissionItem.getItemInfo());
        }
    }
}