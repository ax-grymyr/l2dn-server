﻿using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Network.OutgoingPackets;
using L2Dn.GameServer.StaticData;
using L2Dn.Network;
using L2Dn.Packets;

namespace L2Dn.GameServer.Network.IncomingPackets;

public struct RequestPostItemListPacket: IIncomingPacket<GameSession>
{
    public void ReadContent(PacketBitReader reader)
    {
    }

    public ValueTask ProcessAsync(Connection connection, GameSession session)
    {
        if (!Config.ALLOW_MAIL || !Config.ALLOW_ATTACHMENTS)
            return ValueTask.CompletedTask;

        Player? player = session.Player;
        if (player == null)
            return ValueTask.CompletedTask;

        // if (!player.isInsideZone(ZoneId.PEACE))
        // {
        // player.sendPacket(SystemMessageId.THE_MAILBOX_FUNCTIONS_CAN_BE_USED_ONLY_IN_PEACE_ZONES_OUTSIDE_OF_THEM_YOU_CAN_ONLY_CHECK_ITS_CONTENTS);
        // return;
        // }

        player.sendPacket(new ExReplyPostItemListPacket(1, player));
        player.sendPacket(new ExReplyPostItemListPacket(2, player));

        return ValueTask.CompletedTask;
    }
}