﻿using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Holders;
using L2Dn.GameServer.Network.OutgoingPackets.Subjugation;
using L2Dn.Network;
using L2Dn.Packets;

namespace L2Dn.GameServer.Network.IncomingPackets.Subjugations;

public struct RequestSubjugationGachaUiPacket: IIncomingPacket<GameSession>
{
    private int _category;

    public void ReadContent(PacketBitReader reader)
    {
        _category = reader.ReadByte();
    }

    public ValueTask ProcessAsync(Connection connection, GameSession session)
    {
        Player? player = session.Player;
        if (player == null)
            return ValueTask.CompletedTask;

        PurgePlayerHolder? holder = player.getPurgePoints().get(_category);
        if (holder != null)
        {
            player.sendPacket(new ExSubjugationGachaUiPacket(holder.getKeys()));
        }

        return ValueTask.CompletedTask;
    }
}