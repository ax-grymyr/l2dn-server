using L2Dn.GameServer.Handlers;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Network.OutgoingPackets.Pets;
using L2Dn.GameServer.Utilities;

namespace L2Dn.GameServer.Scripts.Handlers.BypassHandlers;

/**
 * @author Geremy
 */
public class PetExtractWindow: IBypassHandler
{
	private static readonly string[] COMMANDS =
	{
		"pet_extract_window",
	};
	
	public bool useBypass(String command, Player player, Creature target)
	{
		if (!target.isNpc())
		{
			return false;
		}
		
		if (command.toLowerCase().startsWith(COMMANDS[0]))
		{
			player.sendPacket(ShowPetExtractSystemPacket.STATIC_PACKET);
			return true;
		}
		
		return false;
	}
	
	public String[] getBypassList()
	{
		return COMMANDS;
	}
}