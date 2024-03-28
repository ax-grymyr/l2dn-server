using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Actor.Request;
using L2Dn.Network;
using L2Dn.Packets;

namespace L2Dn.GameServer.Network.IncomingPackets.Enchanting.MultiEnchanting;

public struct ExRequestFinishMultiEnchantScrollPacket: IIncomingPacket<GameSession>
{
    public void ReadContent(PacketBitReader reader)
    {
    }

    public ValueTask ProcessAsync(Connection connection, GameSession session)
    {
        Player? player = session.Player;
        if (player == null)
            return ValueTask.CompletedTask;

        if (player.getRequest<EnchantItemRequest>() == null)
            return ValueTask.CompletedTask;
		
        player.removeRequest<EnchantItemRequest>();
        player.getChallengeInfo().setChallengePointsPendingRecharge(-1, -1);
        
        return ValueTask.CompletedTask;
    }
}