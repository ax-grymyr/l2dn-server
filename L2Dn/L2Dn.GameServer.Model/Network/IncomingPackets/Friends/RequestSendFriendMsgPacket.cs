﻿using System.Text;
using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Network.Enums;
using L2Dn.GameServer.Network.OutgoingPackets;
using L2Dn.GameServer.Utilities;
using L2Dn.Network;
using L2Dn.Packets;
using NLog;
using Config = L2Dn.GameServer.Configuration.Config;

namespace L2Dn.GameServer.Network.IncomingPackets.Friends;

public struct RequestSendFriendMsgPacket: IIncomingPacket<GameSession>
{
    private static readonly Logger LOGGER_CHAT = LogManager.GetLogger("chat");

    private string _message;
    private string _reciever;

    public void ReadContent(PacketBitReader reader)
    {
        _message = reader.ReadString();
        _reciever = reader.ReadString();
    }

    public ValueTask ProcessAsync(Connection connection, GameSession session)
    {
        Player? player = session.Player;
        if (player == null)
            return ValueTask.CompletedTask;

        if (string.IsNullOrEmpty(_message) || _message.Length > 300)
            return ValueTask.CompletedTask;

        Player? targetPlayer = World.getInstance().getPlayer(_reciever);
        if (targetPlayer == null || !targetPlayer.getFriendList().Contains(player.ObjectId))
        {
            player.sendPacket(SystemMessageId.THAT_PLAYER_IS_NOT_ONLINE);
            return ValueTask.CompletedTask;
        }

        if (Config.General.LOG_CHAT)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("PRIV_MSG [");
            sb.Append(player);
            sb.Append(" to ");
            sb.Append(targetPlayer);
            sb.Append("] ");
            sb.Append(_message);
            LOGGER_CHAT.Info(sb.ToString());
        }

        targetPlayer.sendPacket(new L2FriendSayPacket(player.getName(), _reciever, _message));
        return ValueTask.CompletedTask;
    }
}