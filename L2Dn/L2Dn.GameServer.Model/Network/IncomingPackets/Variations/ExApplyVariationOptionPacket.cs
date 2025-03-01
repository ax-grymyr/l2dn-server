using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Actor.Request;
using L2Dn.GameServer.Model.Items.Instances;
using L2Dn.GameServer.Network.OutgoingPackets.Variations;
using L2Dn.Network;
using L2Dn.Packets;

namespace L2Dn.GameServer.Network.IncomingPackets.Variations;

public struct ExApplyVariationOptionPacket: IIncomingPacket<GameSession>
{
    private int _enchantedObjectId;
    private int _option1;
    private int _option2;

    public void ReadContent(PacketBitReader reader)
    {
        _enchantedObjectId = reader.ReadInt32();
        _option1 = reader.ReadInt32();
        _option2 = reader.ReadInt32();
    }

    public ValueTask ProcessAsync(Connection connection, GameSession session)
    {
        Player? player = session.Player;
        if (player == null)
            return ValueTask.CompletedTask;

        VariationRequest? request = player.getRequest<VariationRequest>();
        if (request == null)
            return ValueTask.CompletedTask;

        Item? targetItem = request.getAugmentedItem();
        if (targetItem == null)
            return ValueTask.CompletedTask;

        VariationInstance? augment = request.getAugment();
        if (augment == null)
            return ValueTask.CompletedTask;

        int option1Id = augment.getOption1Id();
        int option2Id = augment.getOption2Id();

        if (targetItem.ObjectId != _enchantedObjectId || _option1 != option1Id || _option2 != option2Id)
        {
            player.sendPacket(new ApplyVariationOptionPacket(0, 0, 0, 0));
            return ValueTask.CompletedTask;
        }

        targetItem.setAugmentation(augment, true);

        player.sendPacket(new ApplyVariationOptionPacket(1, _enchantedObjectId, _option1, _option2));

        // Apply new augment.
        if (targetItem.isEquipped())
        {
            targetItem.getAugmentation()?.applyBonus(player);
        }

        // Recalculate all stats.
        player.getStat().recalculateStats(true);

        player.sendItemList();
        player.removeRequest<VariationRequest>();

        return ValueTask.CompletedTask;
    }
}