using System.Text;
using L2Dn.GameServer.AI;
using L2Dn.GameServer.Data.Xml;
using L2Dn.GameServer.Handlers;
using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Events.Impl.Npcs;
using L2Dn.GameServer.Model.Events.Impl.Players;
using L2Dn.GameServer.Model.Items.Instances;
using L2Dn.GameServer.Model.Olympiads;
using L2Dn.GameServer.Network.OutgoingPackets;
using L2Dn.GameServer.Utilities;
using L2Dn.Network;
using L2Dn.Packets;

namespace L2Dn.GameServer.Network.IncomingPackets;

public struct RequestBypassToServerPacket: IIncomingPacket<GameSession>
{
    // FIXME: This is for compatibility, will be changed when bypass functionality got an overhaul by NosBit
    private static readonly string[] _possibleNonHtmlCommands =
    {
        "_bbs",
        "bbs",
        "_mail",
        "_friend",
        "_match",
        "_diary",
        "_olympiad?command",
        "menu_select",
        "manor_menu_select",
        "pccafe"
    };

    private string _command;

    public void ReadContent(PacketBitReader reader)
    {
        _command = reader.ReadString();
    }

    public ValueTask ProcessAsync(Connection connection, GameSession session)
    {
		Player? player = session.Player;
		if (player == null)
			return ValueTask.CompletedTask;
		
		if (_command.isEmpty())
		{
			PacketLogger.Instance.Warn(player + " sent empty bypass!");
			Disconnection.of(session, player).defaultSequence(LeaveWorldPacket.STATIC_PACKET);
			return ValueTask.CompletedTask;
		}
		
		bool requiresBypassValidation = true;
		foreach (string possibleNonHtmlCommand in _possibleNonHtmlCommands)
		{
			if (_command.startsWith(possibleNonHtmlCommand))
			{
				requiresBypassValidation = false;
				break;
			}
		}
		
		int bypassOriginId = 0;
		if (requiresBypassValidation)
		{
			bypassOriginId = player.validateHtmlAction(_command);
			if (bypassOriginId == -1)
				return ValueTask.CompletedTask;
			
			if (bypassOriginId > 0 && !Util.isInsideRangeOfObjectId(player, bypassOriginId, Npc.INTERACTION_DISTANCE))
			{
				// No logging here, this could be a common case where the player has the html still open and run
				// too far away and then clicks a html action
				return ValueTask.CompletedTask;
			}
		}

		// TODO: flood protection
		// if (!client.getFloodProtectors().canUseServerBypass())
		// {
		// 	return ValueTask.CompletedTask;
		// }
		
		if (player.Events.HasSubscribers<OnPlayerBypass>())
		{
			OnPlayerBypass onPlayerBypass = new OnPlayerBypass(player, _command);
			if (player.Events.Notify(onPlayerBypass) && onPlayerBypass.Terminate)
				return ValueTask.CompletedTask;
		}
		
		try
		{
			if (_command.startsWith("admin_"))
			{
				AdminCommandHandler.getInstance().useAdminCommand(player, _command, true);
			}
			else if (CommunityBoardHandler.getInstance().isCommunityBoardCommand(_command))
			{
				CommunityBoardHandler.getInstance().handleParseCommand(_command, player);
			}
			else if (_command.equals("come_here") && player.isGM())
			{
				comeHere(player);
			}
			else if (_command.startsWith("npc_"))
			{
				int endOfId = _command.IndexOf('_', 5);
				string id;
				if (endOfId > 0)
				{
					id = _command.Substring(4, endOfId - 4);
				}
				else
				{
					id = _command.Substring(4);
				}
				
				if (Util.isDigit(id))
				{
					WorldObject obj = World.getInstance().findObject(int.Parse(id));
					if (obj != null && obj.isNpc() && endOfId > 0 && player.isInsideRadius2D(obj, Npc.INTERACTION_DISTANCE))
					{
						((Npc)obj).onBypassFeedback(player, _command.Substring(endOfId + 1));
					}
				}
				
				player.sendPacket(ActionFailedPacket.STATIC_PACKET);
			}
			else if (_command.startsWith("item_"))
			{
				int endOfId = _command.IndexOf('_', 5);
				string id;
				if (endOfId > 0)
				{
					id = _command.Substring(5, endOfId - 5);
				}
				else
				{
					id = _command.Substring(5);
				}
				try
				{
					Item item = player.getInventory().getItemByObjectId(int.Parse(id));
					if (item != null && endOfId > 0)
					{
						item.onBypassFeedback(player, _command.Substring(endOfId + 1));
					}
					
					player.sendPacket(ActionFailedPacket.STATIC_PACKET);
				}
				catch (Exception nfe)
				{
					PacketLogger.Instance.Warn("Exception for command [" + _command + "] " + nfe);
				}
			}
			else if (_command.startsWith("_match"))
			{
				string parameters = _command.Substring(_command.IndexOf('?') + 1);
				StringTokenizer st = new StringTokenizer(parameters, "&");
				int heroclass = int.Parse(st.nextToken().Split("=")[1]);
				int heropage = int.Parse(st.nextToken().Split("=")[1]);
				int heroid = Hero.getInstance().getHeroByClass(heroclass);
				if (heroid > 0)
				{
					Hero.getInstance().showHeroFights(player, heroclass, heroid, heropage);
				}
			}
			else if (_command.startsWith("_diary"))
			{
				string parameters = _command.Substring(_command.IndexOf('?') + 1);
				StringTokenizer st = new StringTokenizer(parameters, "&");
				int heroclass = int.Parse(st.nextToken().Split("=")[1]);
				int heropage = int.Parse(st.nextToken().Split("=")[1]);
				int heroid = Hero.getInstance().getHeroByClass(heroclass);
				if (heroid > 0)
				{
					Hero.getInstance().showHeroDiary(player, heroclass, heroid, heropage);
				}
			}
			else if (_command.startsWith("_olympiad?command"))
			{
				int arenaId = int.Parse(_command.Split("=")[2]);
				IBypassHandler handler = BypassHandler.getInstance().getHandler("arenachange");
				if (handler != null)
				{
					handler.useBypass("arenachange " + (arenaId - 1), player, null);
				}
			}
			else if (_command.startsWith("menu_select"))
			{
				Npc lastNpc = player.getLastFolkNPC();
				if (lastNpc != null && lastNpc.canInteract(player) && lastNpc.Events.HasSubscribers<OnNpcMenuSelect>())
				{
					string[] split = _command.Substring(_command.IndexOf('?') + 1).Split("&");
					int ask = int.Parse(split[0].Split("=")[1]);
					int reply = int.Parse(split[1].Split("=")[1]);
					lastNpc.Events.NotifyAsync(new OnNpcMenuSelect(player, lastNpc, ask, reply));
				}
			}
			else if (_command.startsWith("manor_menu_select"))
			{
				Npc lastNpc = player.getLastFolkNPC();
				if (Config.ALLOW_MANOR && lastNpc != null && lastNpc.canInteract(player) && lastNpc.Events.HasSubscribers<OnNpcManorBypass>())
				{
					string[] split = _command.Substring(_command.IndexOf('?') + 1).Split("&");
					int ask = int.Parse(split[0].Split("=")[1]);
					int state = int.Parse(split[1].Split("=")[1]);
					bool time = split[2].Split("=")[1].equals("1");
					lastNpc.Events.NotifyAsync(new OnNpcManorBypass(player, lastNpc, ask, state, time));
				}
			}
			else if (_command.startsWith("pccafe"))
			{
				if (!Config.PC_CAFE_ENABLED)
					return ValueTask.CompletedTask;

				int multisellId = int.Parse(_command.Substring(10).Trim());
				MultisellData.getInstance().separateAndSend(multisellId, player, null, false);
			}
			else
			{
				IBypassHandler handler = BypassHandler.getInstance().getHandler(_command);
				if (handler != null)
				{
					if (bypassOriginId > 0)
					{
						WorldObject bypassOrigin = World.getInstance().findObject(bypassOriginId);
						if (bypassOrigin != null && bypassOrigin.isCreature())
						{
							handler.useBypass(_command, player, (Creature) bypassOrigin);
						}
						else
						{
							handler.useBypass(_command, player, null);
						}
					}
					else
					{
						handler.useBypass(_command, player, null);
					}
				}
				else
				{
					PacketLogger.Instance.Warn(player + " sent not handled RequestBypassToServer: [" + _command + "]");
				}
			}
		}
		catch (Exception e)
		{
			PacketLogger.Instance.Warn("Exception processing bypass from " + player + ": " + _command + " " + e);
			if (player.isGM())
			{
				StringBuilder sb = new StringBuilder(200);
				sb.Append("<html><body>");
				sb.Append("Bypass error: " + e + "<br1>");
				sb.Append("Bypass command: " + _command + "<br1>");
				sb.Append("StackTrace:<br1>");
				sb.Append("</body></html>");
				
				// item html
				NpcHtmlMessagePacket msg = new NpcHtmlMessagePacket(0, 1, sb.ToString());
				//msg.disableValidation();
				
				player.sendPacket(msg);
			}
		}

		return ValueTask.CompletedTask;
	}
	
	/**
	 * @param player
	 */
	private static void comeHere(Player player)
	{
		WorldObject obj = player.getTarget();
		if (obj == null)
		{
			return;
		}
		if (obj.isNpc())
		{
			Npc temp = (Npc) obj;
			temp.setTarget(player);
			temp.getAI().setIntention(CtrlIntention.AI_INTENTION_MOVE_TO, player.getLocation());
		}
	}
}