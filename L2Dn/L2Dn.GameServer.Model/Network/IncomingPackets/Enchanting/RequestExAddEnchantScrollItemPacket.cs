using L2Dn.GameServer.Data.Xml;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Actor.Request;
using L2Dn.GameServer.Model.Items.Enchant;
using L2Dn.GameServer.Model.Items.Instances;
using L2Dn.GameServer.Network.Enums;
using L2Dn.GameServer.Network.OutgoingPackets.Enchanting;
using L2Dn.Network;
using L2Dn.Packets;

namespace L2Dn.GameServer.Network.IncomingPackets.Enchanting;

public struct RequestExAddEnchantScrollItemPacket: IIncomingPacket<GameSession>
{
    private int _scrollObjectId;

    public void ReadContent(PacketBitReader reader)
    {
        _scrollObjectId = reader.ReadInt32();
        //reader.ReadInt32(); // enchantObjectId?
    }

    public ValueTask ProcessAsync(Connection connection, GameSession session)
    {
        Player? player = session.Player;
        if (player == null)
            return ValueTask.CompletedTask;

        if (player.getRequest<EnchantItemRequest>() == null)
        {
            player.addRequest(new EnchantItemRequest(player, _scrollObjectId));
        }
        
        EnchantItemRequest request = player.getRequest<EnchantItemRequest>();
        request.setEnchantingScroll(_scrollObjectId);
		
        Item scroll = request.getEnchantingScroll();
        if (scroll == null)
        {
            // Message may be custom.
            player.sendPacket(SystemMessageId.AUGMENTATION_REQUIREMENTS_ARE_NOT_FULFILLED);
            player.sendPacket(new ExPutEnchantScrollItemResultPacket(0));
            request.setEnchantingItem(Player.ID_NONE);
            request.setEnchantingScroll(Player.ID_NONE);
            return ValueTask.CompletedTask;
        }
		
        EnchantScroll scrollTemplate = EnchantItemData.getInstance().getEnchantScroll(scroll);
        if (scrollTemplate == null)
        {
            // Message may be custom.
            player.sendPacket(SystemMessageId.AUGMENTATION_REQUIREMENTS_ARE_NOT_FULFILLED);
            player.sendPacket(new ExPutEnchantScrollItemResultPacket(0));
            request.setEnchantingScroll(Player.ID_NONE);
            return ValueTask.CompletedTask;
        }
		
        request.setTimestamp(DateTime.UtcNow);
        player.sendPacket(new ExPutEnchantScrollItemResultPacket(_scrollObjectId));
        
        return ValueTask.CompletedTask;
    }
}