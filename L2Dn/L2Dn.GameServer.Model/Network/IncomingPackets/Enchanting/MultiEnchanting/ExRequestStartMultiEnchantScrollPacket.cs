using L2Dn.GameServer.Data.Xml;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Actor.Request;
using L2Dn.GameServer.Model.Items.Enchant;
using L2Dn.GameServer.Model.Items.Instances;
using L2Dn.GameServer.Network.OutgoingPackets.Enchanting;
using L2Dn.Network;
using L2Dn.Packets;

namespace L2Dn.GameServer.Network.IncomingPackets.Enchanting.MultiEnchanting;

public struct ExRequestStartMultiEnchantScrollPacket: IIncomingPacket<GameSession>
{
    private int _scrollObjectId;

    public void ReadContent(PacketBitReader reader)
    {
        _scrollObjectId = reader.ReadInt32();
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

        Item? scroll = player.getInventory().getItemByObjectId(_scrollObjectId);
        EnchantScroll? scrollTemplate = EnchantItemData.getInstance().getEnchantScroll(scroll.getId());
        if (scrollTemplate == null || scrollTemplate.isBlessed() || scrollTemplate.isBlessedDown() ||
            scrollTemplate.isSafe() || scrollTemplate.isGiant())
        {
            player.sendPacket(new ExResetSelectMultiEnchantScrollPacket(player, _scrollObjectId, 1));
            return ValueTask.CompletedTask;
        }

        request.setEnchantingScroll(_scrollObjectId);

        player.sendPacket(new ExResetSelectMultiEnchantScrollPacket(player, _scrollObjectId, 0));

        return ValueTask.CompletedTask;
    }
}