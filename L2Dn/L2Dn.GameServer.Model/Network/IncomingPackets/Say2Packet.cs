using System.Text;
using L2Dn.GameServer.Enums;
using L2Dn.GameServer.Handlers;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Effects;
using L2Dn.GameServer.Model.Events.Impl.Players;
using L2Dn.GameServer.Model.Items.Instances;
using L2Dn.GameServer.Model.Olympiads;
using L2Dn.GameServer.Network.Enums;
using L2Dn.GameServer.Network.OutgoingPackets;
using L2Dn.GameServer.Utilities;
using L2Dn.Network;
using L2Dn.Packets;
using Config = L2Dn.GameServer.Configuration.Config;

namespace L2Dn.GameServer.Network.IncomingPackets;

public struct Say2Packet: IIncomingPacket<GameSession>
{
    private static readonly string[] WALKER_COMMAND_LIST =
    [
        "USESKILL",
        "USEITEM",
        "BUYITEM",
        "SELLITEM",
        "SAVEITEM",
        "LOADITEM",
        "MSG",
        "DELAY",
        "LABEL",
        "JMP",
        "CALL",
        "RETURN",
        "MOVETO",
        "NPCSEL",
        "NPCDLG",
        "DLGSEL",
        "CHARSTATUS",
        "POSOUTRANGE",
        "POSINRANGE",
        "GOHOME",
        "SAY",
        "EXIT",
        "PAUSE",
        "STRINDLG",
        "STRNOTINDLG",
        "CHANGEWAITTYPE",
        "FORCEATTACK",
        "ISMEMBER",
        "REQUESTJOINPARTY",
        "REQUESTOUTPARTY",
        "QUITPARTY",
        "MEMBERSTATUS",
        "CHARBUFFS",
        "ITEMCOUNT",
        "FOLLOWTELEPORT"
    ];

    private string _text;
    private ChatType _type;
    private string _target;
    private bool _shareLocation;

    public void ReadContent(PacketBitReader reader)
    {
        _text = reader.ReadString();
        _type = (ChatType)reader.ReadInt32();
        _shareLocation = reader.ReadByte() == 1;
        if (_type == ChatType.WHISPER)
        {
            _target = reader.ReadString();
            _shareLocation = false;
        }
    }

    public ValueTask ProcessAsync(Connection connection, GameSession session)
    {
		Player? player = session.Player;
		if (player == null)
			return ValueTask.CompletedTask;

		ChatType chatType = _type;
		string text = _text;
		if (!Enum.IsDefined(chatType))
		{
			PacketLogger.Instance.Warn($"Say2: Invalid type: {chatType} Player : {player.getName()} text: {text}");
			connection.Send(ActionFailedPacket.STATIC_PACKET);
			Disconnection.of(player).defaultSequence(LeaveWorldPacket.STATIC_PACKET);
			return ValueTask.CompletedTask;
		}

		if (string.IsNullOrEmpty(text))
		{
			PacketLogger.Instance.Warn(player.getName() + ": sending empty text. Possible packet hack!");
			connection.Send(ActionFailedPacket.STATIC_PACKET);
			Disconnection.of(player).defaultSequence(LeaveWorldPacket.STATIC_PACKET);
			return ValueTask.CompletedTask;
		}

		// Even though the client can handle more characters than it's current limit allows, an overflow (critical error) happens if you pass a huge (1000+) message.
		// July 11, 2011 - Verified on High Five 4 official client as 105.
		// Allow higher limit if player shift some item (text is longer then).
		bool hasItem = text.Contains('\x8');
		if (!player.isGM() && ((hasItem && text.Length > 500) || (!hasItem && text.Length > 105)))
		{
			connection.Send(new SystemMessagePacket(SystemMessageId.WHEN_A_USER_S_KEYBOARD_INPUT_EXCEEDS_A_CERTAIN_CUMULATIVE_SCORE_A_CHAT_BAN_WILL_BE_APPLIED_THIS_IS_DONE_TO_DISCOURAGE_SPAMMING_PLEASE_AVOID_POSTING_THE_SAME_MESSAGE_MULTIPLE_TIMES_DURING_A_SHORT_PERIOD));
			return ValueTask.CompletedTask;
		}

		if (Config.L2WALKER_PROTECTION && chatType == ChatType.WHISPER && checkBot(text))
		{
			Util.handleIllegalPlayerAction(player, "Client Emulator Detect: " + player + " using L2Walker.", Config.DEFAULT_PUNISH);
			return ValueTask.CompletedTask;
		}

		if (player.isCursedWeaponEquipped() && (chatType == ChatType.TRADE || chatType == ChatType.SHOUT))
		{
			connection.Send(new SystemMessagePacket(SystemMessageId.SHOUT_AND_TRADE_CHATTING_CANNOT_BE_USED_WHILE_POSSESSING_A_CURSED_WEAPON));
			return ValueTask.CompletedTask;
		}

		if (player.isChatBanned() && text[0] != '.')
		{
			if (player.isAffected(EffectFlag.CHAT_BLOCK))
			{
				connection.Send(new SystemMessagePacket(SystemMessageId.YOU_HAVE_BEEN_REPORTED_AS_AN_ILLEGAL_PROGRAM_USER_SO_CHATTING_IS_NOT_ALLOWED));
			}
			else if (Config.BAN_CHAT_CHANNELS.Contains(chatType))
			{
				connection.Send(new SystemMessagePacket(SystemMessageId.IF_YOU_TRY_TO_CHAT_BEFORE_THE_PROHIBITION_IS_REMOVED_THE_PROHIBITION_TIME_WILL_INCREASE_EVEN_FURTHER_S1_SEC_OF_PROHIBITION_IS_LEFT));
			}

			return ValueTask.CompletedTask;
		}

		if (player.isInOlympiadMode() || OlympiadManager.getInstance().isRegistered(player))
		{
			connection.Send(new SystemMessagePacket(SystemMessageId.YOU_CANNOT_CHAT_WHILE_PARTICIPATING_IN_THE_OLYMPIAD));
			return ValueTask.CompletedTask;
		}

		if (player.isJailed() && Config.JAIL_DISABLE_CHAT && (chatType == ChatType.WHISPER || chatType == ChatType.SHOUT ||
		                                                      chatType == ChatType.TRADE || chatType == ChatType.HERO_VOICE))
		{
			player.sendMessage("You can not chat with players outside of the jail.");
			return ValueTask.CompletedTask;
		}

		if (chatType == ChatType.PETITION_PLAYER && player.isGM())
			chatType = ChatType.PETITION_GM;

		if (Config.LOG_CHAT)
		{
			StringBuilder sb = new StringBuilder();
			sb.Append(chatType);
			sb.Append(" [");
			sb.Append(player);
			if (chatType == ChatType.WHISPER)
			{
				sb.Append(" to ");
				sb.Append(_target);
				sb.Append("] ");
				sb.Append(text);
			}
			else
			{
				sb.Append("] ");
				sb.Append(text);
			}

			PacketLogger.Instance.Info(sb.ToString());
		}

		if (hasItem && !parseAndPublishItem(player, text))
			return ValueTask.CompletedTask;

		if (player.Events.HasSubscribers<OnPlayerChat>())
		{
			OnPlayerChat onPlayerChat = new OnPlayerChat(player, _target, text, chatType);
			if (player.Events.Notify(onPlayerChat))
			{
				text = onPlayerChat.FilteredText;
				chatType = onPlayerChat.FilteredType;
			}
		}

		// Say Filter implementation
		if (Config.USE_SAY_FILTER)
			text = checkText(text);

		IChatHandler? handler = ChatHandler.getInstance().getHandler(chatType);
		if (handler != null)
		{
			handler.handleChat(chatType, player, _target, text, _shareLocation);
		}
		else
		{
			PacketLogger.Instance.Info("No handler registered for ChatType: " + chatType + " Player: " + player);
		}

		return ValueTask.CompletedTask;
	}

	private static bool checkBot(string text) => WALKER_COMMAND_LIST.Any(text.StartsWith);

	private static string checkText(string text)
	{
		string filteredText = text;
		foreach (string pattern in Config.FILTER_LIST)
		{
			filteredText = filteredText.replaceAll("(?i)" + pattern, Config.CHAT_FILTER_CHARS);
		}

		return filteredText;
	}

	private static bool parseAndPublishItem(Player owner, string text)
	{
		int pos1 = -1;
		while ((pos1 = text.IndexOf('\x8', pos1)) > -1)
		{
			int pos = text.IndexOf("ID=", pos1, StringComparison.Ordinal);
			if (pos == -1)
			{
				return false;
			}

			StringBuilder result = new StringBuilder(9);
			pos += 3;
			while (pos < text.Length && char.IsDigit(text[pos]))
			{
				result.Append(text[pos]);
				pos++;
			}

			int id = int.Parse(result.ToString());
			Item? item = owner.getInventory().getItemByObjectId(id);
			if (item == null)
			{
				PacketLogger.Instance.Info(owner + " trying publish item which does not own! ID:" + id);
				return false;
			}

			item.publish();

			pos1 = text.IndexOf('\x8', pos) + 1;
			if (pos1 == 0) // missing ending tag
			{
				PacketLogger.Instance.Info(owner + " sent invalid publish item msg! ID:" + id);
				return false;
			}
		}

		return true;
	}
}