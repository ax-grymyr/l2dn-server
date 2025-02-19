using L2Dn.GameServer.Handlers;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Network.OutgoingPackets;

namespace L2Dn.GameServer.Scripts.Handlers.BypassHandlers;

public class ReleaseAttribute: IBypassHandler
{
	private static readonly string[] COMMANDS =
    [
        "ReleaseAttribute",
    ];

	public bool useBypass(string command, Player player, Creature? target)
	{
		if (target == null || !target.isNpc())
		{
			return false;
		}

		player.sendPacket(new ExShowBaseAttributeCancelWindowPacket(player));
		return true;
	}

	public string[] getBypassList()
	{
		return COMMANDS;
	}
}