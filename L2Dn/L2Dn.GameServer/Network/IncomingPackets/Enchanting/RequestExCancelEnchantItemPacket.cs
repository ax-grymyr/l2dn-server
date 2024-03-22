using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Actor.Request;
using L2Dn.GameServer.Network.OutgoingPackets.Enchanting;
using L2Dn.Network;
using L2Dn.Packets;

namespace L2Dn.GameServer.Network.IncomingPackets.Enchanting;

public struct RequestExCancelEnchantItemPacket: IIncomingPacket<GameSession>
{
    public void ReadContent(PacketBitReader reader)
    {
    }

    public ValueTask ProcessAsync(Connection connection, GameSession session)
    {
        Player? player = session.Player;
        if (player == null)
            return ValueTask.CompletedTask;

        player.sendPacket(new EnchantResultPacket(EnchantResultPacket.ERROR, null, null, 0));
        player.removeRequest<EnchantItemRequest>();
        player.getChallengeInfo().setChallengePointsPendingRecharge(-1, -1);
        
        return ValueTask.CompletedTask;
    }
}