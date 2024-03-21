using L2Dn.GameServer.Data.Xml;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Items.Instances;
using L2Dn.GameServer.Network.Enums;
using L2Dn.GameServer.Network.OutgoingPackets;
using L2Dn.GameServer.Utilities;
using L2Dn.Network;
using L2Dn.Packets;

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

        Item item = player.getInventory().getItemByObjectId(_objectId);
        if (item == null)
            return ValueTask.CompletedTask;
		
        if (item.getOwnerId() != player.getObjectId())
        {
            Util.handleIllegalPlayerAction(player,
                "Warning!! Character " + player.getName() + " of account " + player.getAccountName() +
                " tryied to destroy augment on item that doesn't own.", Config.DEFAULT_PUNISH);
            
            return ValueTask.CompletedTask;
        }
		
        if (!item.isAugmented())
        {
            player.sendPacket(SystemMessageId.AUGMENTATION_REMOVAL_CAN_ONLY_BE_DONE_ON_AN_AUGMENTED_ITEM);
            return ValueTask.CompletedTask;
        }
		
        if (item.isPvp() && !Config.ALT_ALLOW_AUGMENT_PVP_ITEMS)
        {
            player.sendPacket(SystemMessageId.THIS_IS_NOT_A_SUITABLE_ITEM);
            return ValueTask.CompletedTask;
        }
		
        long price = VariationData.getInstance().getCancelFee(item.getId(), item.getAugmentation().getMineralId());
        if (price < 0)
        {
            player.sendPacket(SystemMessageId.THIS_IS_NOT_A_SUITABLE_ITEM);
            return ValueTask.CompletedTask;
        }
		
        player.sendPacket(new ExPutItemResultForVariationCancelPacket(item, price));

        return ValueTask.CompletedTask;
    }
}