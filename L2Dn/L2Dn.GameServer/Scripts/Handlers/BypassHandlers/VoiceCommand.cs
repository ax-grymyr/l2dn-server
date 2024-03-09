using L2Dn.GameServer.Handlers;
using L2Dn.GameServer.Model.Actor;

namespace L2Dn.GameServer.Scripts.Handlers.BypassHandlers;

/**
 * @author DS
 */
public class VoiceCommand: IBypassHandler
{
	private static readonly string[] COMMANDS =
	{
		"voice"
	};
	
	public bool useBypass(String command, Player player, Creature target)
	{
		// only voice commands allowed
		if ((command.Length > 7) && (command[6] == '.'))
		{
			String vc;
			String vparams;
			int endOfCommand = command.IndexOf(' ', 7);
			if (endOfCommand > 0)
			{
				vc = command.Substring(7, endOfCommand).Trim();
				vparams = command.Substring(endOfCommand).Trim();
			}
			else
			{
				vc = command.Substring(7).Trim();
				vparams = null;
			}
			
			if (vc.Length > 0)
			{
				IVoicedCommandHandler vch = VoicedCommandHandler.getInstance().getHandler(vc);
				if (vch != null)
				{
					return vch.useVoicedCommand(vc, player, vparams);
				}
			}
		}
		
		return false;
	}
	
	public String[] getBypassList()
	{
		return COMMANDS;
	}
}