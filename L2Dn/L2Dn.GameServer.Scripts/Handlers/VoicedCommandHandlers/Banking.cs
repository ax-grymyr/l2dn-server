using L2Dn.GameServer.Handlers;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Utilities;
using Config = L2Dn.GameServer.Configuration.Config;

namespace L2Dn.GameServer.Scripts.Handlers.VoicedCommandHandlers;

/**
 * This class trades Gold Bars for Adena and vice versa.
 * @author Ahmed
 */
public class Banking: IVoicedCommandHandler
{
	private static readonly string[] VOICED_COMMANDS =
	{
		"bank",
		"withdraw",
		"deposit"
	};

	public bool useVoicedCommand(string command, Player activeChar, string @params)
	{
		if (command.equals("bank"))
		{
			activeChar.sendMessage(".deposit (" + Config.Banking.BANKING_SYSTEM_ADENA + " Adena = " + Config.Banking.BANKING_SYSTEM_GOLDBARS + " Goldbar) / .withdraw (" + Config.Banking.BANKING_SYSTEM_GOLDBARS + " Goldbar = " + Config.Banking.BANKING_SYSTEM_ADENA + " Adena)");
		}
		else if (command.equals("deposit"))
		{
			if (activeChar.getInventory().getInventoryItemCount(57, 0) >= Config.Banking.BANKING_SYSTEM_ADENA)
			{
				if (!activeChar.reduceAdena("Goldbar", Config.Banking.BANKING_SYSTEM_ADENA, activeChar, false))
				{
					return false;
				}
				activeChar.getInventory().addItem("Goldbar", 3470, Config.Banking.BANKING_SYSTEM_GOLDBARS, activeChar, null);
				activeChar.getInventory().updateDatabase();
				activeChar.sendMessage("Thank you, you now have " + Config.Banking.BANKING_SYSTEM_GOLDBARS + " Goldbar(s), and " + Config.Banking.BANKING_SYSTEM_ADENA + " less adena.");
			}
			else
			{
				activeChar.sendMessage("You do not have enough Adena to convert to Goldbar(s), you need " + Config.Banking.BANKING_SYSTEM_ADENA + " Adena.");
			}
		}
		else if (command.equals("withdraw"))
		{
			if (activeChar.getInventory().getInventoryItemCount(3470, 0) >= Config.Banking.BANKING_SYSTEM_GOLDBARS)
			{
				if (!activeChar.destroyItemByItemId("Adena", 3470, Config.Banking.BANKING_SYSTEM_GOLDBARS, activeChar, false))
				{
					return false;
				}
				activeChar.getInventory().addAdena("Adena", Config.Banking.BANKING_SYSTEM_ADENA, activeChar, null);
				activeChar.getInventory().updateDatabase();
				activeChar.sendMessage("Thank you, you now have " + Config.Banking.BANKING_SYSTEM_ADENA + " Adena, and " + Config.Banking.BANKING_SYSTEM_GOLDBARS + " less Goldbar(s).");
			}
			else
			{
				activeChar.sendMessage("You do not have any Goldbars to turn into " + Config.Banking.BANKING_SYSTEM_ADENA + " Adena.");
			}
		}
		return true;
	}

	public string[] getVoicedCommandList()
	{
		return VOICED_COMMANDS;
	}
}