using L2Dn.GameServer.Cache;
using L2Dn.GameServer.Db;
using L2Dn.GameServer.Handlers;
using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Html;
using L2Dn.GameServer.Network.Enums;
using L2Dn.GameServer.Network.OutgoingPackets;
using L2Dn.GameServer.Utilities;

namespace L2Dn.GameServer.Scripts.Handlers.AdminCommandHandlers;

/**
 * Admin Prime Points manage admin commands.
 * @author St3eT
 */
public class AdminPrimePoints: IAdminCommandHandler
{
	private static readonly string[] ADMIN_COMMANDS =
    [
        "admin_primepoints",
    ];
	
	public bool useAdminCommand(string command, Player activeChar)
	{
		StringTokenizer st = new StringTokenizer(command, " ");
		string actualCommand = st.nextToken();
		if (actualCommand.equals("admin_primepoints"))
		{
			if (st.hasMoreTokens())
			{
				string action = st.nextToken();
				Player target = getTarget(activeChar);
				if ((target == null) || !st.hasMoreTokens())
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
					showMenuHtml(activeChar);
					BuilderUtil.sendSysMessage(activeChar, "Invalid Value!");
					return false;
				}
				
				switch (action)
				{
					case "set":
					{
						target.setPrimePoints(value);
						target.sendMessage("Admin set your Prime Point(s) to " + value + "!");
						BuilderUtil.sendSysMessage(activeChar, "You set " + value + " Prime Point(s) to player " + target.getName());
						break;
					}
					case "increase":
					{
						if (target.getPrimePoints() == int.MaxValue)
						{
							showMenuHtml(activeChar);
							activeChar.sendMessage(target.getName() + " already have max count of Prime Points!");
							return false;
						}
						
						int primeCount = Math.Min((target.getPrimePoints() + value), int.MaxValue);
						if (primeCount < 0)
						{
							primeCount = int.MaxValue;
						}
						target.setPrimePoints(primeCount);
						target.sendMessage("Admin increase your Prime Point(s) by " + value + "!");
						BuilderUtil.sendSysMessage(activeChar, "You increased Prime Point(s) of " + target.getName() + " by " + value);
						break;
					}
					case "decrease":
					{
						if (target.getPrimePoints() == 0)
						{
							showMenuHtml(activeChar);
							activeChar.sendMessage(target.getName() + " already have min count of Prime Points!");
							return false;
						}
						
						int primeCount = Math.Max(target.getPrimePoints() - value, 0);
						target.setPrimePoints(primeCount);
						target.sendMessage("Admin decreased your Prime Point(s) by " + value + "!");
						BuilderUtil.sendSysMessage(activeChar, "You decreased Prime Point(s) of " + target.getName() + " by " + value);
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
						}
						
						if (range <= 0)
						{
							int count = increaseForAll(World.getInstance().getPlayers(), value);
							BuilderUtil.sendSysMessage(activeChar, "You increased Prime Point(s) of all online players (" + count + ") by " + value + ".");
						}
						else if (range > 0)
						{
							int count = increaseForAll(World.getInstance().getVisibleObjectsInRange<Player>(activeChar, range), value);
							BuilderUtil.sendSysMessage(activeChar, "You increased Prime Point(s) of all players (" + count + ") in range " + range + " by " + value + ".");
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
			if ((temp != null) && (temp.getOnlineStatus() == CharacterOnlineStatus.Online))
			{
				if (temp.getPrimePoints() == int.MaxValue)
				{
					continue;
				}
				
				int primeCount = Math.Min((temp.getPrimePoints() + value), int.MaxValue);
				if (primeCount < 0)
				{
					primeCount = int.MaxValue;
				}
				temp.setPrimePoints(primeCount);
				temp.sendMessage("Admin increase your Prime Point(s) by " + value + "!");
				counter++;
			}
		}
		return counter;
	}
	
	private Player getTarget(Player activeChar)
	{
		return ((activeChar.getTarget() != null) && (activeChar.getTarget().getActingPlayer() != null)) ? activeChar.getTarget().getActingPlayer() : activeChar;
	}
	
	private void showMenuHtml(Player activeChar)
	{
		HtmlContent htmlContent = HtmlContent.LoadFromFile("html/admin/primepoints.htm", activeChar);
		NpcHtmlMessagePacket html = new NpcHtmlMessagePacket(0, 1, htmlContent);
		
		Player target = getTarget(activeChar);
		int points = target.getPrimePoints();
		
		htmlContent.Replace("%points%", Util.formatAdena(points));
		htmlContent.Replace("%targetName%", target.getName());
		activeChar.sendPacket(html);
	}
	
	public string[] getAdminCommandList()
	{
		return ADMIN_COMMANDS;
	}
}