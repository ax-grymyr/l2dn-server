using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Network.OutgoingPackets;

namespace L2Dn.GameServer.Handlers.BypassHandlers;

public class ReleaseAttribute: IBypassHandler
{
	private static readonly string[] COMMANDS =
	{
		"ReleaseAttribute"
	};
	
	public bool useBypass(String command, Player player, Creature target)
	{
		if (!target.isNpc())
		{
			return false;
		}
		
		player.sendPacket(new ExShowBaseAttributeCancelWindowPacket(player));
		return true;
	}
	
	public String[] getBypassList()
	{
		return COMMANDS;
	}
}