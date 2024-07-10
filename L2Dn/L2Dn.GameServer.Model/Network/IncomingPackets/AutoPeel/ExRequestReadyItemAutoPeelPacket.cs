using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Actor.Request;
using L2Dn.GameServer.Model.Items.Instances;
using L2Dn.GameServer.Network.OutgoingPackets.AutoPeel;
using L2Dn.GameServer.Utilities;
using L2Dn.Network;
using L2Dn.Packets;

namespace L2Dn.GameServer.Network.IncomingPackets.AutoPeel;

public struct ExRequestReadyItemAutoPeelPacket: IIncomingPacket<GameSession>
{
    private int _itemObjectId;

    public void ReadContent(PacketBitReader reader)
    {
        _itemObjectId = reader.ReadInt32();
    }

    public ValueTask ProcessAsync(Connection connection, GameSession session)
    {
        Player? player = session.Player;
        if (player == null)
            return ValueTask.CompletedTask;

        Item item = player.getInventory().getItemByObjectId(_itemObjectId);
        if (item == null || !item.isEtcItem() || item.getEtcItem().getExtractableItems() == null ||
            item.getEtcItem().getExtractableItems().Count == 0)
        {
            player.sendPacket(new ExReadyItemAutoPeelPacket(false, _itemObjectId));
            return ValueTask.CompletedTask;
        }

        player.addRequest(new AutoPeelRequest(player, item));
        player.sendPacket(new ExReadyItemAutoPeelPacket(true, _itemObjectId));
        
        return ValueTask.CompletedTask;
    }
}