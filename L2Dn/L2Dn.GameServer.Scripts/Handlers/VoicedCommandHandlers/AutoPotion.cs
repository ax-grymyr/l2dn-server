using L2Dn.GameServer.Handlers;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.TaskManagers;

namespace L2Dn.GameServer.Scripts.Handlers.VoicedCommandHandlers;

/**
 * @author Mobius, Gigi
 */
public class AutoPotion: IVoicedCommandHandler
{
	private static readonly String[] VOICED_COMMANDS =
	{
		"apon",
		"apoff",
		"potionon",
		"potionoff"
	};
	
	public bool useVoicedCommand(String command, Player activeChar, String target)
	{
		if (!Config.AUTO_POTIONS_ENABLED || (activeChar == null))
		{
			return false;
		}
		if (activeChar.getLevel() < Config.AUTO_POTION_MIN_LEVEL)
		{
			activeChar.sendMessage("You need to be at least " + Config.AUTO_POTION_MIN_LEVEL + " to use auto potions.");
			return false;
		}
		
		switch (command)
		{
			case "apon":
			case "potionon":
			{
				AutoPotionTaskManager.getInstance().add(activeChar);
				activeChar.sendMessage("Auto potions is enabled.");
				break;
			}
			case "apoff":
			case "potionoff":
			{
				AutoPotionTaskManager.getInstance().remove(activeChar);
				activeChar.sendMessage("Auto potions is disabled.");
				break;
			}
		}
		
		return true;
	}
	
	public String[] getVoicedCommandList()
	{
		return VOICED_COMMANDS;
	}
}