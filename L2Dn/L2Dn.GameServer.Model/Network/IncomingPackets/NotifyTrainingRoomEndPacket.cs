﻿using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Holders;
using L2Dn.GameServer.Network.OutgoingPackets.Training;
using L2Dn.Network;
using L2Dn.Packets;

namespace L2Dn.GameServer.Network.IncomingPackets;

public struct NotifyTrainingRoomEndPacket: IIncomingPacket<GameSession>
{
    public void ReadContent(PacketBitReader reader)
    {
    }

    public ValueTask ProcessAsync(Connection connection, GameSession session)
    {
        Player? player = session.Player;
        if (player == null)
            return ValueTask.CompletedTask;

        TrainingHolder? holder = player.getTraingCampInfo();
        if (holder == null)
            return ValueTask.CompletedTask;

        if (holder.isTraining())
        {
            holder.setEndTime(DateTime.UtcNow);
            player.setTraingCampInfo(holder);
            player.enableAllSkills();
            player.setInvul(false);
            player.setInvisible(false);
            player.setImmobilized(false);
            player.teleToLocation(player.getLastLocation() ?? player.Location.Location3D);
            player.sendPacket(ExTrainingZoneLeavingPacket.STATIC_PACKET);
            holder.setEndTime(DateTime.UtcNow);
            player.setTraingCampInfo(holder);
        }

        return ValueTask.CompletedTask;
    }
}