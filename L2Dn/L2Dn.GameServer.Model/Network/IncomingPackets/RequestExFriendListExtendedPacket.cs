﻿using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Network.OutgoingPackets.Friends;
using L2Dn.Network;
using L2Dn.Packets;
using Config = L2Dn.GameServer.Configuration.Config;

namespace L2Dn.GameServer.Network.IncomingPackets;

public struct RequestExFriendListExtendedPacket: IIncomingPacket<GameSession>
{
    public void ReadContent(PacketBitReader reader)
    {
    }

    public ValueTask ProcessAsync(Connection connection, GameSession session)
    {
        if (!Config.General.ALLOW_MAIL)
            return ValueTask.CompletedTask;

        Player? player = session.Player;
        if (player == null)
            return ValueTask.CompletedTask;

        player.sendPacket(new FriendListPacket(player));

        return ValueTask.CompletedTask;
    }
}