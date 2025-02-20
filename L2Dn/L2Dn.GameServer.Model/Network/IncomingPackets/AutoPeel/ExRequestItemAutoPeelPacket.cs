using L2Dn.GameServer.Handlers;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Actor.Request;
using L2Dn.GameServer.Model.Holders;
using L2Dn.GameServer.Model.Items;
using L2Dn.GameServer.Model.Items.Instances;
using L2Dn.GameServer.Network.OutgoingPackets.AutoPeel;
using L2Dn.GameServer.Utilities;
using L2Dn.Network;
using L2Dn.Packets;

namespace L2Dn.GameServer.Network.IncomingPackets.AutoPeel;

public struct ExRequestItemAutoPeelPacket: IIncomingPacket<GameSession>
{
    private int _itemObjectId;
    private long _totalPeelCount;
    private long _remainingPeelCount;

    public void ReadContent(PacketBitReader reader)
    {
        _itemObjectId = reader.ReadInt32();
        _totalPeelCount = reader.ReadInt64();
        _remainingPeelCount = reader.ReadInt64();
    }

    public ValueTask ProcessAsync(Connection connection, GameSession session)
    {
        Player? player = session.Player;
        if (player == null)
            return ValueTask.CompletedTask;

        AutoPeelRequest? request = player.getRequest<AutoPeelRequest>();
        Item? item;
        if (request == null)
        {
            item = player.getInventory().getItemByObjectId(_itemObjectId);
            if (item == null || !item.isEtcItem() || item.getEtcItem().getExtractableItems() == null ||
                item.getEtcItem().getExtractableItems().Count == 0)
            {
                return ValueTask.CompletedTask;
            }

            request = new AutoPeelRequest(player, item);
            player.addRequest(request);
        }
        else if (request.isProcessing())
        {
            return ValueTask.CompletedTask;
        }

        request.setProcessing(true);

        item = request.getItem();
        if (item.ObjectId != _itemObjectId || item.getOwnerId() != player.ObjectId)
        {
            player.removeRequest<AutoPeelRequest>();
            return ValueTask.CompletedTask;
        }

        request.setTotalPeelCount(_totalPeelCount);
        request.setRemainingPeelCount(_remainingPeelCount);

        EtcItem etcItem = (EtcItem) item.getTemplate();
        if (etcItem.getExtractableItems() != null && etcItem.getExtractableItems().Count != 0)
        {
            IItemHandler? handler = ItemHandler.getInstance().getHandler(item.getEtcItem());
            if (handler != null && !handler.useItem(player, item, false))
            {
                request.setProcessing(false);
                player.sendPacket(new ExResultItemAutoPeelPacket(false, _totalPeelCount, _remainingPeelCount,
                    new List<ItemHolder>()));
            }
        }

        return ValueTask.CompletedTask;
    }
}