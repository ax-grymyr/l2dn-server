using L2Dn.GameServer.Data.Xml;
using L2Dn.GameServer.Handlers;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Utilities;
using NLog;

namespace L2Dn.GameServer.Scripts.Handlers.BypassHandlers;

public class Multisell: IBypassHandler
{
	private static readonly Logger _logger = LogManager.GetLogger(nameof(Multisell));

	private static readonly string[] COMMANDS =
	{
		"multisell",
		"exc_multisell"
	};
	
	public bool useBypass(string command, Player player, Creature target)
	{
		if (!target.isNpc())
		{
			return false;
		}
		
		try
		{
			int listId;
			if (command.toLowerCase().startsWith(COMMANDS[0])) // multisell
			{
				listId = int.Parse(command.Substring(9).Trim());
				MultisellData.getInstance().separateAndSend(listId, player, (Npc) target, false);
				return true;
			}
			else if (command.toLowerCase().startsWith(COMMANDS[1])) // exc_multisell
			{
				listId = int.Parse(command.Substring(13).Trim());
				MultisellData.getInstance().separateAndSend(listId, player, (Npc) target, true);
				return true;
			}
			return false;
		}
		catch (Exception e)
		{
			_logger.Warn("Exception in " + GetType().Name, e);
		}
		
		return false;
	}
	
	public string[] getBypassList()
	{
		return COMMANDS;
	}
}