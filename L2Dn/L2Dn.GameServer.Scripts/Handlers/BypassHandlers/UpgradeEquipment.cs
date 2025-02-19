using L2Dn.GameServer.Handlers;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Network.OutgoingPackets.EquipmentUpgrade;

namespace L2Dn.GameServer.Scripts.Handlers.BypassHandlers;

/**
 * @author Mobius
 */
public class UpgradeEquipment: IBypassHandler
{
	private static readonly int FERRIS = 30847;

    private static readonly string[] COMMANDS = ["UpgradeEquipment"];

	public bool useBypass(string command, Player player, Creature? target)
	{
		if (target == null || !target.isNpc() || ((Npc) target).getId() != FERRIS)
		{
			return false;
		}
		player.sendPacket(new ExShowUpgradeSystemPacket());
		return true;
	}

	public string[] getBypassList()
	{
		return COMMANDS;
	}
}