using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Actor.Request;
using L2Dn.GameServer.Model.Items.Instances;
using L2Dn.GameServer.Network.Enums;
using L2Dn.GameServer.Network.OutgoingPackets.CompoundEnchant;
using L2Dn.Network;
using L2Dn.Packets;

namespace L2Dn.GameServer.Network.IncomingPackets.Enchanting.CompoundEnchanting;

public struct RequestNewEnchantRemoveTwoPacket: IIncomingPacket<GameSession>
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

        if (player.isInStoreMode())
        {
            player.sendPacket(SystemMessageId.YOU_CANNOT_DO_THAT_WHILE_IN_A_PRIVATE_STORE_OR_PRIVATE_WORKSHOP);
            player.sendPacket(ExEnchantOneFailPacket.STATIC_PACKET);
            return ValueTask.CompletedTask;
        }
		
        if (player.isProcessingTransaction() || player.isProcessingRequest())
        {
            player.sendPacket(SystemMessageId.YOU_CANNOT_USE_THIS_SYSTEM_DURING_TRADING_PRIVATE_STORE_AND_WORKSHOP_SETUP);
            player.sendPacket(ExEnchantOneFailPacket.STATIC_PACKET);
            return ValueTask.CompletedTask;
        }
		
        CompoundRequest request = player.getRequest<CompoundRequest>();
        if (request == null || request.isProcessing())
        {
            player.sendPacket(ExEnchantTwoRemoveFailPacket.STATIC_PACKET);
            return ValueTask.CompletedTask;
        }
		
        Item item = request.getItemTwo();
        if (item == null || item.getObjectId() != _objectId)
        {
            player.sendPacket(ExEnchantTwoRemoveFailPacket.STATIC_PACKET);
            return ValueTask.CompletedTask;
        }
		
        request.setItemTwo(0);
        if (request.getItemOne() == null)
        {
            player.removeRequest<CompoundRequest>();
        }
		
        player.sendPacket(ExEnchantTwoRemoveOkPacket.STATIC_PACKET);
        
        return ValueTask.CompletedTask;
    }
}