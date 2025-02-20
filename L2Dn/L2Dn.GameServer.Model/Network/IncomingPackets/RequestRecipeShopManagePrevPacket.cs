using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Network.OutgoingPackets;
using L2Dn.Network;
using L2Dn.Packets;

namespace L2Dn.GameServer.Network.IncomingPackets;

public struct RequestRecipeShopManagePrevPacket: IIncomingPacket<GameSession>
{
    public void ReadContent(PacketBitReader reader)
    {
    }

    public ValueTask ProcessAsync(Connection connection, GameSession session)
    {
        Player? player = session.Player;
        if (player == null)
            return ValueTask.CompletedTask;

        WorldObject? target = player.getTarget();
        Player? targetPlayer = target?.getActingPlayer();
        if (player.isAlikeDead() || target == null || !target.isPlayer() || targetPlayer == null)
        {
            player.sendPacket(ActionFailedPacket.STATIC_PACKET);
            return ValueTask.CompletedTask;
        }

        player.sendPacket(new RecipeShopSellListPacket(player, targetPlayer));

        return ValueTask.CompletedTask;
    }
}