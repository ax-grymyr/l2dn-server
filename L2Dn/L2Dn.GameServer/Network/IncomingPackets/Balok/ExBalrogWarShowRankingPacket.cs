using L2Dn.GameServer.InstanceManagers;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Network.OutgoingPackets.Balok;
using L2Dn.Network;
using L2Dn.Packets;

namespace L2Dn.GameServer.Network.IncomingPackets.Balok;

public struct ExBalrogWarShowRankingPacket: IIncomingPacket<GameSession>
{
    public void ReadContent(PacketBitReader reader)
    {
    }

    public ValueTask ProcessAsync(Connection connection, GameSession session)
    {
        Player? player = session.Player;
        if (player == null)
            return ValueTask.CompletedTask;

        if (!BattleWithBalokManager.getInstance().getInBattle())
            return ValueTask.CompletedTask;
		
        player.sendPacket(new BalrogWarShowRankingPacket());
        
        return ValueTask.CompletedTask;
    }
}