﻿using L2Dn.GameServer.InstanceManagers;
using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Network.OutgoingPackets;
using L2Dn.GameServer.Utilities;
using L2Dn.Network;
using L2Dn.Packets;
using Config = L2Dn.GameServer.Configuration.Config;

namespace L2Dn.GameServer.Network.IncomingPackets;

public struct RequestSentPostPacket: IIncomingPacket<GameSession>
{
    private int _msgId;

    public void ReadContent(PacketBitReader reader)
    {
        _msgId = reader.ReadInt32();
    }

    public ValueTask ProcessAsync(Connection connection, GameSession session)
    {
        if (!Config.General.ALLOW_MAIL)
            return ValueTask.CompletedTask;

        Player? player = session.Player;
        if (player == null)
            return ValueTask.CompletedTask;

        Message? msg = MailManager.getInstance().getMessage(_msgId);
        if (msg == null)
            return ValueTask.CompletedTask;

        // if (!player.isInsideZone(ZoneId.PEACE) && msg.hasAttachments())
        // {
        // player.sendPacket(SystemMessageId.THE_MAILBOX_FUNCTIONS_CAN_BE_USED_ONLY_IN_PEACE_ZONES_OUTSIDE_OF_THEM_YOU_CAN_ONLY_CHECK_ITS_CONTENTS);
        // return;
        // }

        if (msg.getSenderId() != player.ObjectId)
        {
            Util.handleIllegalPlayerAction(player, player + " tried to read not own post!", Config.General.DEFAULT_PUNISH);
            return ValueTask.CompletedTask;
        }

        if (msg.isDeletedBySender())
            return ValueTask.CompletedTask;

        player.sendPacket(new ExReplySentPostPacket(msg));

        return ValueTask.CompletedTask;
    }
}