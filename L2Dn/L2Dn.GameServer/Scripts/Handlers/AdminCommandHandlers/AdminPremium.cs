// using L2Dn.GameServer.Cache;
// using L2Dn.GameServer.Handlers;
// using L2Dn.GameServer.InstanceManagers;
// using L2Dn.GameServer.Model;
// using L2Dn.GameServer.Model.Actor;
// using L2Dn.GameServer.Network.OutgoingPackets;
// using L2Dn.GameServer.Utilities;
//
// namespace L2Dn.GameServer.Scripts.Handlers.AdminCommandHandlers;
//
// /**
//  * @author Mobius
//  */
// public class AdminPremium: IAdminCommandHandler
// {
// 	private static readonly string[] ADMIN_COMMANDS =
// 	{
// 		"admin_premium_menu",
// 		"admin_premium_add1",
// 		"admin_premium_add2",
// 		"admin_premium_add3",
// 		"admin_premium_info",
// 		"admin_premium_remove"
// 	};
// 	
// 	public bool useAdminCommand(String command, Player activeChar)
// 	{
// 		if (command.equals("admin_premium_menu"))
// 		{
// 			AdminHtml.showAdminHtml(activeChar, "premium_menu.htm");
// 		}
// 		else if (command.startsWith("admin_premium_add1"))
// 		{
// 			try
// 			{
// 				addPremiumStatus(activeChar, 1, command.Substring(19));
// 			}
// 			catch (IndexOutOfRangeException e)
// 			{
// 				BuilderUtil.sendSysMessage(activeChar, "Please enter a valid account name.");
// 			}
// 		}
// 		else if (command.startsWith("admin_premium_add2"))
// 		{
// 			try
// 			{
// 				addPremiumStatus(activeChar, 2, command.Substring(19));
// 			}
// 			catch (IndexOutOfRangeException e)
// 			{
// 				BuilderUtil.sendSysMessage(activeChar, "Please enter a valid account name.");
// 			}
// 		}
// 		else if (command.startsWith("admin_premium_add3"))
// 		{
// 			try
// 			{
// 				addPremiumStatus(activeChar, 3, command.Substring(19));
// 			}
// 			catch (IndexOutOfRangeException e)
// 			{
// 				BuilderUtil.sendSysMessage(activeChar, "Please enter a valid account name.");
// 			}
// 		}
// 		else if (command.startsWith("admin_premium_info"))
// 		{
// 			try
// 			{
// 				viewPremiumInfo(activeChar, command.Substring(19));
// 			}
// 			catch (IndexOutOfRangeException e)
// 			{
// 				BuilderUtil.sendSysMessage(activeChar, "Please enter a valid account name.");
// 			}
// 		}
// 		else if (command.startsWith("admin_premium_remove"))
// 		{
// 			try
// 			{
// 				removePremium(activeChar, command.Substring(21));
// 			}
// 			catch (IndexOutOfRangeException e)
// 			{
// 				BuilderUtil.sendSysMessage(activeChar, "Please enter a valid account name.");
// 			}
// 		}
//
// 		NpcHtmlMessagePacket html = new NpcHtmlMessagePacket(0, 1,
// 			HtmCache.getInstance().getHtm(activeChar, "html/admin/premium_menu.htm"));
//
// 		activeChar.sendPacket(html);
// 		return true;
// 	}
// 	
// 	private void addPremiumStatus(Player admin, int months, String accountName)
// 	{
// 		if (!Config.PREMIUM_SYSTEM_ENABLED)
// 		{
// 			admin.sendMessage("Premium system is disabled.");
// 			return;
// 		}
// 		
// 		// TODO: Add check if account exists XD
// 		PremiumManager.getInstance().addPremiumTime(accountName, TimeSpan.FromDays(months * 30));
// 		admin.sendMessage("Account " + accountName + " will now have premium status until " +
// 		                  PremiumManager.getInstance().getPremiumExpiration(accountName).ToString("dd.MM.yyyy HH:mm") +
// 		                  ".");
// 		
// 		if (Config.PC_CAFE_RETAIL_LIKE)
// 		{
// 			foreach (Player player in World.getInstance().getPlayers())
// 			{
// 				if (player.getAccountName() == accountName)
// 				{
// 					PcCafePointsManager.getInstance().run(player);
// 					break;
// 				}
// 			}
// 		}
// 	}
// 	
// 	private void viewPremiumInfo(Player admin, String accountName)
// 	{
// 		if (!Config.PREMIUM_SYSTEM_ENABLED)
// 		{
// 			admin.sendMessage("Premium system is disabled.");
// 			return;
// 		}
// 		
// 		if (PremiumManager.getInstance().getPremiumExpiration(accountName) > 0)
// 		{
// 			admin.sendMessage("Account " + accountName + " has premium status until " + PremiumManager.getInstance()
// 				.getPremiumExpiration(accountName).ToString("dd.MM.yyyy HH:mm") + ".");
// 		}
// 		else
// 		{
// 			admin.sendMessage("Account " + accountName + " has no premium status.");
// 		}
// 	}
// 	
// 	private void removePremium(Player admin, String accountName)
// 	{
// 		if (!Config.PREMIUM_SYSTEM_ENABLED)
// 		{
// 			admin.sendMessage("Premium system is disabled.");
// 			return;
// 		}
// 		
// 		if (PremiumManager.getInstance().getPremiumExpiration(accountName) > 0)
// 		{
// 			PremiumManager.getInstance().removePremiumStatus(accountName, true);
// 			admin.sendMessage("Account " + accountName + " has no longer premium status.");
// 		}
// 		else
// 		{
// 			admin.sendMessage("Account " + accountName + " has no premium status.");
// 		}
// 	}
// 	
// 	public String[] getAdminCommandList()
// 	{
// 		return ADMIN_COMMANDS;
// 	}
// }