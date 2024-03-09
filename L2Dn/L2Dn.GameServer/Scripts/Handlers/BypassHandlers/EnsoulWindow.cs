using L2Dn.GameServer.Handlers;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Network.OutgoingPackets.Ensoul;
using L2Dn.GameServer.Utilities;

namespace L2Dn.GameServer.Scripts.Handlers.BypassHandlers;

/**
 * @author St3eT
 */
public class EnsoulWindow: IBypassHandler
{
	private static readonly string[] COMMANDS =
	{
		"show_ensoul_window",
		"show_extract_ensoul_window"
	};
	
	public bool useBypass(String command, Player player, Creature target)
	{
		if (!target.isNpc())
		{
			return false;
		}
		
		if (command.toLowerCase().startsWith(COMMANDS[0])) // show_ensoul_window
		{
			player.sendPacket(ExShowEnsoulWindowPacket.STATIC_PACKET);
			return true;
		}
		else if (command.toLowerCase().startsWith(COMMANDS[1])) // show_extract_ensoul_window
		{
			player.sendPacket(ExShowEnsoulExtractionWindowPacket.STATIC_PACKET);
			return true;
		}
		return false;
	}
	
	public String[] getBypassList()
	{
		return COMMANDS;
	}
}