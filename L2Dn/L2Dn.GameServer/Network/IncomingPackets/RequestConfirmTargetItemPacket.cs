using L2Dn.GameServer.Data.Xml;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Items.Instances;
using L2Dn.GameServer.Network.Enums;
using L2Dn.GameServer.Network.OutgoingPackets;
using L2Dn.Network;
using L2Dn.Packets;

namespace L2Dn.GameServer.Network.IncomingPackets;

public struct RequestConfirmTargetItemPacket: IIncomingPacket<GameSession>
{
    private int _itemObjId;

    public void ReadContent(PacketBitReader reader)
    {
        _itemObjId = reader.ReadInt32();
    }

    public ValueTask ProcessAsync(Connection connection, GameSession session)
    {
        Player? player = session.Player;
        if (player == null)
            return ValueTask.CompletedTask;

        Item item = player.getInventory().getItemByObjectId(_itemObjId);
        if (item == null)
            return ValueTask.CompletedTask;
		
        if (!VariationData.getInstance().hasFeeData(item.getId()))
        {
            player.sendPacket(SystemMessageId.THIS_IS_NOT_A_SUITABLE_ITEM);
            return ValueTask.CompletedTask;
        }
		
        if (!RefinePacketHelper.isValid(player, item))
        {
            // Different system message here
            if (item.isAugmented())
            {
                player.sendPacket(SystemMessageId.ONCE_AN_ITEM_IS_AUGMENTED_IT_CANNOT_BE_AUGMENTED_AGAIN);
                return ValueTask.CompletedTask;
            }
			
            player.sendPacket(SystemMessageId.THIS_IS_NOT_A_SUITABLE_ITEM);
            return ValueTask.CompletedTask;
        }
		
        player.sendPacket(new ExPutItemResultForVariationMakePacket(_itemObjId, item.getId()));
        
        return ValueTask.CompletedTask;
    }
}