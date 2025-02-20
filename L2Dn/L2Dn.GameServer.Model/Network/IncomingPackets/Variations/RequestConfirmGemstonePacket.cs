using L2Dn.GameServer.Data.Xml;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Items.Instances;
using L2Dn.GameServer.Model.Options;
using L2Dn.GameServer.Network.Enums;
using L2Dn.GameServer.Network.OutgoingPackets.Variations;
using L2Dn.Network;
using L2Dn.Packets;

namespace L2Dn.GameServer.Network.IncomingPackets.Variations;

public struct RequestConfirmGemstonePacket: IIncomingPacket<GameSession>
{
    private int _targetItemObjId;
    private int _mineralItemObjId;
    private int _feeItemObjId;
    private long _feeCount;

    public void ReadContent(PacketBitReader reader)
    {
        _targetItemObjId = reader.ReadInt32();
        _mineralItemObjId = reader.ReadInt32();
        _feeItemObjId = reader.ReadInt32();
        _feeCount = reader.ReadInt64();
    }

    public ValueTask ProcessAsync(Connection connection, GameSession session)
    {
        Player? player = session.Player;
        if (player == null)
            return ValueTask.CompletedTask;

        Item? targetItem = player.getInventory().getItemByObjectId(_targetItemObjId);
        if (targetItem == null)
            return ValueTask.CompletedTask;

        Item? refinerItem = player.getInventory().getItemByObjectId(_mineralItemObjId);
        if (refinerItem == null)
            return ValueTask.CompletedTask;

        Item? gemStoneItem = player.getInventory().getItemByObjectId(_feeItemObjId);
        if (gemStoneItem == null)
            return ValueTask.CompletedTask;

        VariationFee? fee = VariationData.getInstance().getFee(targetItem.getId(), refinerItem.getId());
        if (!RefinePacketHelper.isValid(player, targetItem, refinerItem, gemStoneItem, fee) || fee == null)
        {
            player.sendPacket(SystemMessageId.THIS_IS_NOT_A_SUITABLE_ITEM);
            return ValueTask.CompletedTask;
        }

        // Check for fee count.
        if (_feeCount != fee.getItemCount())
        {
            player.sendPacket(SystemMessageId.GEMSTONE_QUANTITY_IS_INCORRECT);
            return ValueTask.CompletedTask;
        }

        player.sendPacket(new ExPutCommissionResultForVariationMakePacket(_feeItemObjId, _feeCount, gemStoneItem.getId()));

        return ValueTask.CompletedTask;
    }
}