using L2Dn.GameServer.Data.Xml;
using L2Dn.GameServer.Handlers;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.BuyList;
using L2Dn.GameServer.Network.OutgoingPackets;
using L2Dn.GameServer.Utilities;
using NLog;

namespace L2Dn.GameServer.Scripts.Handlers.AdminCommandHandlers;

/**
 * This class handles following admin commands:
 * <ul>
 * <li>gmshop = shows menu</li>
 * <li>buy id = shows shop with respective id</li>
 * </ul>
 */
public class AdminShop: IAdminCommandHandler
{
	private static readonly Logger LOGGER = LogManager.GetLogger(nameof(AdminShop));
	
	private static readonly string[] ADMIN_COMMANDS =
	{
		"admin_buy",
		"admin_gmshop",
		"admin_multisell",
		"admin_exc_multisell"
	};
	
	public bool useAdminCommand(string command, Player activeChar)
	{
		if (command.startsWith("admin_buy"))
		{
			try
			{
				handleBuyRequest(activeChar, command.Substring(10));
			}
			catch (Exception e)
			{
				BuilderUtil.sendSysMessage(activeChar, "Please specify buylist.");
			}
		}
		else if (command.equals("admin_gmshop"))
		{
			AdminHtml.showAdminHtml(activeChar, "gmshops.htm");
		}
		else if (command.startsWith("admin_multisell"))
		{
			try
			{
				int listId = int.Parse(command.Substring(16).Trim());
				MultisellData.getInstance().separateAndSend(listId, activeChar, null, false);
			}
			catch (Exception e)
			{
				BuilderUtil.sendSysMessage(activeChar, "Please specify multisell list ID.");
			}
		}
		else if (command.toLowerCase().startsWith("admin_exc_multisell"))
		{
			try
			{
				int listId = int.Parse(command.Substring(20).Trim());
				MultisellData.getInstance().separateAndSend(listId, activeChar, null, true);
			}
			catch (Exception e)
			{
				BuilderUtil.sendSysMessage(activeChar, "Please specify multisell list ID.");
			}
		}
		return true;
	}
	
	public string[] getAdminCommandList()
	{
		return ADMIN_COMMANDS;
	}
	
	private void handleBuyRequest(Player activeChar, string command)
	{
		int val = -1;
		try
		{
			val = int.Parse(command);
		}
		catch (Exception e)
		{
			LOGGER.Warn("admin buylist failed:" + command);
		}
		
		ProductList buyList = BuyListData.getInstance().getBuyList(val);
		if (buyList != null)
		{
			activeChar.sendPacket(new ExBuySellListPacket(buyList, activeChar, 0));
			activeChar.sendPacket(new ExBuySellListPacket(activeChar, false));
			activeChar.sendPacket(new ExBuySellListPacket(false));
		}
		else
		{
			LOGGER.Warn("no buylist with id:" + val);
			activeChar.sendPacket(ActionFailedPacket.STATIC_PACKET);
		}
	}
}
