using L2Dn.GameServer.Data.Xml;
using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Items.Instances;
using L2Dn.GameServer.Network.Enums;
using L2Dn.GameServer.Network.OutgoingPackets;
using L2Dn.GameServer.Utilities;
using L2Dn.Network;
using L2Dn.Packets;
using Config = L2Dn.GameServer.Configuration.Config;

namespace L2Dn.GameServer.Network.IncomingPackets;

public struct RequestConfirmCancelItemPacket: IIncomingPacket<GameSession>
{
    private int _objectId;

    public void ReadContent(PacketBitReader reader)
    {
        _objectId = reader.ReadInt32();
    }

    public ValueTask ProcessAsync(Connection connection, GameSession session)
    {
        Player? player = session.Player;
        if (player == null)
            return ValueTask.CompletedTask;

        Item? item = player.getInventory().getItemByObjectId(_objectId);
        if (item == null)
            return ValueTask.CompletedTask;

        if (item.getOwnerId() != player.ObjectId)
        {
            Util.handleIllegalPlayerAction(player,
                "Warning!! Character " + player.getName() + " of account " + player.getAccountName() +
                " tryied to destroy augment on item that doesn't own.", Config.General.DEFAULT_PUNISH);

            return ValueTask.CompletedTask;
        }

        VariationInstance? augmentation = item.getAugmentation();
        if (!item.isAugmented() || augmentation == null)
        {
            player.sendPacket(SystemMessageId.AUGMENTATION_REMOVAL_CAN_ONLY_BE_DONE_ON_AN_AUGMENTED_ITEM);
            return ValueTask.CompletedTask;
        }

        if (item.isPvp() && !Config.Character.ALT_ALLOW_AUGMENT_PVP_ITEMS)
        {
            player.sendPacket(SystemMessageId.THIS_IS_NOT_A_SUITABLE_ITEM);
            return ValueTask.CompletedTask;
        }

        long price = VariationData.getInstance().getCancelFee(item.getId(), augmentation.getMineralId());
        if (price < 0)
        {
            player.sendPacket(SystemMessageId.THIS_IS_NOT_A_SUITABLE_ITEM);
            return ValueTask.CompletedTask;
        }

        player.sendPacket(new ExPutItemResultForVariationCancelPacket(item, price));

        return ValueTask.CompletedTask;
    }
}