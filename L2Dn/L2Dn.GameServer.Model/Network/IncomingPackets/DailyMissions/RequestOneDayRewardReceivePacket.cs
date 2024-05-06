using L2Dn.GameServer.Data.Xml;
using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Actor.Request;
using L2Dn.GameServer.Network.OutgoingPackets.DailyMissions;
using L2Dn.Network;
using L2Dn.Packets;
using ThreadPool = L2Dn.GameServer.Utilities.ThreadPool;

namespace L2Dn.GameServer.Network.IncomingPackets.DailyMissions;

public struct RequestOneDayRewardReceivePacket: IIncomingPacket<GameSession>
{
    private int _id;

    public void ReadContent(PacketBitReader reader)
    {
        _id = reader.ReadInt16();
    }

    public ValueTask ProcessAsync(Connection connection, GameSession session)
    {
        // TODO: flood protection
        // if (!client.getFloodProtectors().canPerformPlayerAction())
        // {
        //     return;
        // }

        Player? player = session.Player;
        if (player == null)
            return ValueTask.CompletedTask;

        if (player.hasRequest<RewardRequest>())
            return ValueTask.CompletedTask;

        player.addRequest(new RewardRequest(player));

        DailyMissionDataHolder? holder = DailyMissionData.getInstance().getDailyMissionData(_id);
        if (holder == null)
        {
            player.removeRequest<RewardRequest>();
            return ValueTask.CompletedTask;
        }

        if (holder.isDisplayable(player))
            holder.requestReward(player);

        player.sendPacket(new ExOneDayReceiveRewardListPacket(player, true));
        player.sendPacket(new ExConnectedTimeAndGettableRewardPacket(player));

        ThreadPool.schedule(() => player.removeRequest<RewardRequest>(), 300);
        return ValueTask.CompletedTask;
    }
}