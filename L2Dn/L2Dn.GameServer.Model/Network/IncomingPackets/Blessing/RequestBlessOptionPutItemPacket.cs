using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Items.Instances;
using L2Dn.GameServer.Network.Enums;
using L2Dn.GameServer.Network.OutgoingPackets.Blessing;
using L2Dn.Network;
using L2Dn.Packets;

namespace L2Dn.GameServer.Network.IncomingPackets.Blessing;

public struct RequestBlessOptionPutItemPacket: IIncomingPacket<GameSession>
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

        if (player.isProcessingTransaction() || player.isInStoreMode())
        {
            player.sendPacket(SystemMessageId.YOU_CANNOT_ENCHANT_WHILE_OPERATING_A_PRIVATE_STORE_OR_PRIVATE_WORKSHOP);
            return ValueTask.CompletedTask;
        }
		
        // first validation check - also over enchant check
        if (item.isBlessed())
        {
            player.sendPacket(SystemMessageId.AUGMENTATION_REQUIREMENTS_ARE_NOT_FULFILLED);
            player.sendPacket(new ExBlessOptionPutItemPacket(0));
            return ValueTask.CompletedTask;
        }
		
        player.sendPacket(new ExBlessOptionPutItemPacket(1));
        
        return ValueTask.CompletedTask;
    }
}