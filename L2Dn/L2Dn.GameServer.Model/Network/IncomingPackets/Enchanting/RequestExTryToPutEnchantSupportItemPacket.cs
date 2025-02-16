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

public struct RequestExTryToPutEnchantSupportItemPacket: IIncomingPacket<GameSession>
{
    private int _supportObjectId;
    private int _enchantObjectId;

    public void ReadContent(PacketBitReader reader)
    {
        _supportObjectId = reader.ReadInt32();
        _enchantObjectId = reader.ReadInt32();
    }

    public ValueTask ProcessAsync(Connection connection, GameSession session)
    {
        Player? player = session.Player;
        if (player == null)
            return ValueTask.CompletedTask;

        EnchantItemRequest request = player.getRequest<EnchantItemRequest>();
        if (request == null || request.isProcessing())
            return ValueTask.CompletedTask;
		
        request.setEnchantingItem(_enchantObjectId);
        request.setSupportItem(_supportObjectId);
		
        Item item = request.getEnchantingItem();
        Item scroll = request.getEnchantingScroll();
        Item support = request.getSupportItem();
        if (item == null || scroll == null || support == null)
        {
            // Message may be custom.
            player.sendPacket(SystemMessageId.AUGMENTATION_REQUIREMENTS_ARE_NOT_FULFILLED);
            request.setEnchantingItem(Player.ID_NONE);
            request.setSupportItem(Player.ID_NONE);
            return ValueTask.CompletedTask;
        }
		
        EnchantScroll? scrollTemplate = EnchantItemData.getInstance().getEnchantScroll(scroll.getId());
        EnchantSupportItem? supportTemplate = EnchantItemData.getInstance().getSupportItem(support.getId());
        if (scrollTemplate == null || supportTemplate == null || !scrollTemplate.isValid(item, supportTemplate))
        {
            // Message may be custom.
            player.sendPacket(SystemMessageId.AUGMENTATION_REQUIREMENTS_ARE_NOT_FULFILLED);
            request.setSupportItem(Player.ID_NONE);
            player.sendPacket(new ExPutEnchantSupportItemResultPacket(0));
            return ValueTask.CompletedTask;
        }
		
        request.setSupportItem(support.ObjectId);
        request.setTimestamp(DateTime.UtcNow);
        player.sendPacket(new ExPutEnchantSupportItemResultPacket(_supportObjectId));
        player.sendPacket(new ChangedEnchantTargetItemProbabilityListPacket(player, false));
        
        return ValueTask.CompletedTask;
    }
}