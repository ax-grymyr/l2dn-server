using L2Dn.GameServer.InstanceManagers;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Variables;
using L2Dn.GameServer.Network.OutgoingPackets.Balok;
using L2Dn.Network;
using L2Dn.Packets;

namespace L2Dn.GameServer.Network.IncomingPackets.Balok;

public struct ExBalrogWarGetRewardPacket: IIncomingPacket<GameSession>
{
    public void ReadContent(PacketBitReader reader)
    {
    }

    public ValueTask ProcessAsync(Connection connection, GameSession session)
    {
        Player? player = session.Player;
        if (player == null)
            return ValueTask.CompletedTask;

        int availableReward = player.getVariables().Get(PlayerVariables.BALOK_AVAILABLE_REWARD, 0);
        if (availableReward != 1)
            return ValueTask.CompletedTask;

        int count = 1;
        int globalStage = BattleWithBalokManager.getInstance().getGlobalStage();
        if (globalStage < 4)
        {
            count = 30; // sayha potion sealed
        }

        int reward = BattleWithBalokManager.getInstance().getReward();
        player.addItem("Battle with Balok", reward, count, player, true);
        player.getVariables().Set(PlayerVariables.BALOK_AVAILABLE_REWARD, -1);
        player.sendPacket(new BalrogWarGetRewardPacket(true));

        return ValueTask.CompletedTask;
    }
}