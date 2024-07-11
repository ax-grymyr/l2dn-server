using L2Dn.GameServer.Handlers;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Utilities;

namespace L2Dn.GameServer.Scripts.Handlers.UserCommandHandlers;

/**
 * @author xban1x
 */
public class ExperienceGain: IVoicedCommandHandler
{
	private static readonly string[] COMMANDS =
	[
		"expoff",
		"expon"
	];
	
	public bool useVoicedCommand(string command, Player player, string @params)
	{
		if (command.equals("expoff"))
		{
			if (!player.getVariables().getBoolean("EXPOFF", false))
			{
				player.disableExpGain();
				player.getVariables().set("EXPOFF", true);
				player.sendMessage("Experience gain is disabled.");
			}
		}
		else if (command.equals("expon"))
		{
			if (player.getVariables().getBoolean("EXPOFF", false))
			{
				player.enableExpGain();
				player.getVariables().set("EXPOFF", false);
				player.sendMessage("Experience gain is enabled.");
			}
		}
		
		return true;
	}
	
	public string[] getVoicedCommandList()
	{
		return COMMANDS;
	}
}