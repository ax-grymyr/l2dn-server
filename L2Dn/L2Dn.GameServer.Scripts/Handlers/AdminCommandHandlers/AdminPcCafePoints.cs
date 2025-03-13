using L2Dn.GameServer.Enums;
using L2Dn.GameServer.Handlers;
using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Html;
using L2Dn.GameServer.Network.OutgoingPackets;
using L2Dn.GameServer.Utilities;
using NLog;

namespace L2Dn.GameServer.Scripts.Handlers.AdminCommandHandlers;

/**
 * Admin PC Points manage admin commands.
 */
public class AdminPcCafePoints: IAdminCommandHandler
{
    private static readonly Logger _logger = LogManager.GetLogger(nameof(AdminPcCafePoints));
	private static readonly string[] ADMIN_COMMANDS =
    [
        "admin_pccafepoints",
    ];

	public bool useAdminCommand(string command, Player activeChar)
	{
		StringTokenizer st = new StringTokenizer(command, " ");
		string actualCommand = st.nextToken();
		if (actualCommand.equals("admin_pccafepoints"))
		{
			if (st.hasMoreTokens())
			{
				string action = st.nextToken();
				Player target = getTarget(activeChar);
				if (target == null || !st.hasMoreTokens())
				{
					return false;
				}

				int value = 0;
				try
				{
					value = int.Parse(st.nextToken());
				}
				catch (Exception e)
				{
                    _logger.Error(e);
					showMenuHtml(activeChar);
					BuilderUtil.sendSysMessage(activeChar, "Invalid Value!");
					return false;
				}

				switch (action)
				{
					case "set":
					{
						if (value > Config.PC_CAFE_MAX_POINTS)
						{
							showMenuHtml(activeChar);
							BuilderUtil.sendSysMessage(activeChar, "You cannot set more than " + Config.PC_CAFE_MAX_POINTS + " PC points!");
							return false;
						}
						if (value < 0)
						{
							value = 0;
						}

						target.setPcCafePoints(value);
						target.sendMessage("Admin set your PC Cafe point(s) to " + value + "!");
						BuilderUtil.sendSysMessage(activeChar, "You set " + value + " PC Cafe point(s) to player " + target.getName());
						target.sendPacket(new ExPcCafePointInfoPacket(value, value, 1));
						break;
					}
					case "increase":
					{
						if (target.getPcCafePoints() == Config.PC_CAFE_MAX_POINTS)
						{
							showMenuHtml(activeChar);
							activeChar.sendMessage(target.getName() + " already have max count of PC points!");
							return false;
						}

						int pcCafeCount = Math.Min(target.getPcCafePoints() + value, Config.PC_CAFE_MAX_POINTS);
						if (pcCafeCount < 0)
						{
							pcCafeCount = Config.PC_CAFE_MAX_POINTS;
						}
						target.setPcCafePoints(pcCafeCount);
						target.sendMessage("Admin increased your PC Cafe point(s) by " + value + "!");
						BuilderUtil.sendSysMessage(activeChar, "You increased PC Cafe point(s) of " + target.getName() + " by " + value);
						target.sendPacket(new ExPcCafePointInfoPacket(pcCafeCount, value, 1));
						break;
					}
					case "decrease":
					{
						if (target.getPcCafePoints() == 0)
						{
							showMenuHtml(activeChar);
							activeChar.sendMessage(target.getName() + " already have min count of PC points!");
							return false;
						}

						int pcCafeCount = Math.Max(target.getPcCafePoints() - value, 0);
						target.setPcCafePoints(pcCafeCount);
						target.sendMessage("Admin decreased your PC Cafe point(s) by " + value + "!");
						BuilderUtil.sendSysMessage(activeChar, "You decreased PC Cafe point(s) of " + target.getName() + " by " + value);
						target.sendPacket(new ExPcCafePointInfoPacket(target.getPcCafePoints(), -value, 1));
						break;
					}
					case "rewardOnline":
					{
						int range = 0;
						try
						{
							range = int.Parse(st.nextToken());
						}
						catch (Exception e)
						{
                            _logger.Error(e);
						}

						if (range <= 0)
						{
							int count = increaseForAll(World.getInstance().getPlayers(), value);
							BuilderUtil.sendSysMessage(activeChar, "You increased PC Cafe point(s) of all online players (" + count + ") by " + value + ".");
						}
						else if (range > 0)
						{
							int count = increaseForAll(World.getInstance().getVisibleObjectsInRange<Player>(activeChar, range), value);
							BuilderUtil.sendSysMessage(activeChar, "You increased PC Cafe point(s) of all players (" + count + ") in range " + range + " by " + value + ".");
						}
						break;
					}
				}
			}
			showMenuHtml(activeChar);
		}
		return true;
	}

	private int increaseForAll(ICollection<Player> playerList, int value)
	{
		int counter = 0;
		foreach (Player temp in playerList)
		{
			if (temp != null && temp.getOnlineStatus() == CharacterOnlineStatus.Online)
			{
				if (temp.getPcCafePoints() == int.MaxValue)
				{
					continue;
				}

				int pcCafeCount = Math.Min(temp.getPcCafePoints() + value, int.MaxValue);
				if (pcCafeCount < 0)
				{
					pcCafeCount = int.MaxValue;
				}
				temp.setPcCafePoints(pcCafeCount);
				temp.sendMessage("Admin increased your PC Cafe point(s) by " + value + "!");
				temp.sendPacket(new ExPcCafePointInfoPacket(pcCafeCount, value, 1));
				counter++;
			}
		}
		return counter;
	}

	private Player getTarget(Player activeChar)
    {
        WorldObject? activeCharTarget = activeChar.getTarget();
		return activeCharTarget?.getActingPlayer() ?? activeChar;
	}

	private void showMenuHtml(Player activeChar)
	{
		HtmlContent htmlContent = HtmlContent.LoadFromFile("html/admin/pccafe.htm", activeChar);

		NpcHtmlMessagePacket html = new NpcHtmlMessagePacket(null, 1, htmlContent);
		Player target = getTarget(activeChar);
		int points = target.getPcCafePoints();
		htmlContent.Replace("%points%", Util.formatAdena(points));
		htmlContent.Replace("%targetName%", target.getName());
		activeChar.sendPacket(html);
	}

	public string[] getAdminCommandList()
	{
		return ADMIN_COMMANDS;
	}
}