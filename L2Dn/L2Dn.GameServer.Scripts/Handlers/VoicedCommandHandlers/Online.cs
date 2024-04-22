using L2Dn.GameServer.Handlers;
using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Utilities;

namespace L2Dn.GameServer.Scripts.Handlers.VoicedCommandHandlers;

/**
 * @author Mobius
 */
public class Online: IVoicedCommandHandler
{
	private static readonly String[] VOICED_COMMANDS =
	{
		"online"
	};
	
	public bool useVoicedCommand(String command, Player player, String target)
	{
		if (command.equals("online") && Config.ENABLE_ONLINE_COMMAND)
		{
			int count = World.getInstance().getPlayers().Count;
			if (count > 1)
			{
				player.sendMessage("There are " + count + " players online!");
			}
			else
			{
				player.sendMessage("There is 1 player online!");
			}
		}
		return true;
	}
	
	public String[] getVoicedCommandList()
	{
		return VOICED_COMMANDS;
	}
}