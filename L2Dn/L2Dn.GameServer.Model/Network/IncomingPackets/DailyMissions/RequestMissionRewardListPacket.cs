﻿using L2Dn.GameServer.Data.Xml;
using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Network.OutgoingPackets.DailyMissions;
using L2Dn.Network;
using L2Dn.Packets;

namespace L2Dn.GameServer.Network.IncomingPackets.DailyMissions;

public struct RequestMissionRewardListPacket: IIncomingPacket<GameSession>
{
    public void ReadContent(PacketBitReader reader)
    {
    }

    public ValueTask ProcessAsync(Connection connection, GameSession session)
    {
        Player? player = session.Player;
        if (player == null)
            return ValueTask.CompletedTask;

        MissionLevelHolder? holder = MissionLevel.getInstance()
            .getMissionBySeason(MissionLevel.getInstance().getCurrentSeason());

        if (holder == null)
            return ValueTask.CompletedTask;

        ExMissionLevelRewardListPacket packet = new ExMissionLevelRewardListPacket(player, holder);
        connection.Send(ref packet);

        return ValueTask.CompletedTask;
    }
}