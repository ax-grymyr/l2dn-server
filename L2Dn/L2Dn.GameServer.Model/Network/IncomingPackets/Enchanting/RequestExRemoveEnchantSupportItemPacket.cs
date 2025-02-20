using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Actor.Request;
using L2Dn.GameServer.Model.Items.Instances;
using L2Dn.GameServer.Network.OutgoingPackets.Enchanting;
using L2Dn.Network;
using L2Dn.Packets;

namespace L2Dn.GameServer.Network.IncomingPackets.Enchanting;

public struct RequestExRemoveEnchantSupportItemPacket: IIncomingPacket<GameSession>
{
    public void ReadContent(PacketBitReader reader)
    {
    }

    public ValueTask ProcessAsync(Connection connection, GameSession session)
    {
        Player? player = session.Player;
        if (player == null)
            return ValueTask.CompletedTask;

        EnchantItemRequest? request = player.getRequest<EnchantItemRequest>();
        if (request == null || request.isProcessing())
            return ValueTask.CompletedTask;

        Item supportItem = request.getSupportItem();
        if (supportItem == null || supportItem.getCount() >= 0)
        {
            request.setSupportItem(Player.ID_NONE);
        }

        request.setTimestamp(DateTime.UtcNow);

        player.sendPacket(ExRemoveEnchantSupportItemResultPacket.STATIC_PACKET);
        player.sendPacket(new ChangedEnchantTargetItemProbabilityListPacket(player, false));

        return ValueTask.CompletedTask;
    }
}