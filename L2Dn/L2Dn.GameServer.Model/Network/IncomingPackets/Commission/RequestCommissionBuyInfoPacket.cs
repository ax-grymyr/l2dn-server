using L2Dn.GameServer.InstanceManagers;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Commission;
using L2Dn.GameServer.Network.Enums;
using L2Dn.GameServer.Network.OutgoingPackets.Commission;
using L2Dn.Network;
using L2Dn.Packets;

namespace L2Dn.GameServer.Network.IncomingPackets.Commission;

public struct RequestCommissionBuyInfoPacket: IIncomingPacket<GameSession>
{
    private long _commissionId;

    public void ReadContent(PacketBitReader reader)
    {
        _commissionId = reader.ReadInt64();
        // packet.readInt(); // CommissionItemType
    }

    public ValueTask ProcessAsync(Connection connection, GameSession session)
    {
        Player? player = session.Player;
        if (player == null)
            return ValueTask.CompletedTask;

        if (!ItemCommissionManager.isPlayerAllowedToInteract(player))
        {
            player.sendPacket(ExCloseCommissionPacket.STATIC_PACKET);
            return ValueTask.CompletedTask;
        }
		
        if (!player.isInventoryUnder80(false) || player.getWeightPenalty() >= 3)
        {
            player.sendPacket(SystemMessageId.TO_BUY_CANCEL_YOU_NEED_TO_FREE_20_OF_WEIGHT_AND_10_OF_SLOTS_IN_YOUR_INVENTORY);
            player.sendPacket(ExResponseCommissionBuyInfoPacket.FAILED);
            return ValueTask.CompletedTask;
        }
		
        CommissionItem commissionItem = ItemCommissionManager.getInstance().getCommissionItem(_commissionId);
        if (commissionItem != null)
        {
            player.sendPacket(new ExResponseCommissionBuyInfoPacket(commissionItem));
        }
        else
        {
            player.sendPacket(SystemMessageId.ITEM_PURCHASE_IS_NOT_AVAILABLE_BECAUSE_THE_CORRESPONDING_ITEM_DOES_NOT_EXIST);
            player.sendPacket(ExResponseCommissionBuyInfoPacket.FAILED);
        }
        
        return ValueTask.CompletedTask;
    }
}