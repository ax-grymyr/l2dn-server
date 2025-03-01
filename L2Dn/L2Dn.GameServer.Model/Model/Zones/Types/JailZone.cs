using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Actor.Tasks.PlayerTasks;
using L2Dn.GameServer.Network.Enums;
using L2Dn.Geometry;
using ThreadPool = L2Dn.GameServer.Utilities.ThreadPool;

namespace L2Dn.GameServer.Model.Zones.Types;

/**
 * A jail zone
 * @author durgus
 */
public class JailZone(int id, ZoneForm form): ZoneType(id, form)
{
	private static readonly Location JAIL_IN_LOC = new(-114356, -249645, -2984, 0);
	private static readonly Location JAIL_OUT_LOC = new(17836, 170178, -3507, 0);

    protected override void onEnter(Creature creature)
	{
		if (creature.isPlayer())
		{
			creature.setInsideZone(ZoneId.JAIL, true);
			creature.setInsideZone(ZoneId.NO_SUMMON_FRIEND, true);
			if (Config.JAIL_IS_PVP)
			{
				creature.setInsideZone(ZoneId.PVP, true);
				creature.sendPacket(SystemMessageId.YOU_HAVE_ENTERED_A_COMBAT_ZONE);
			}
			if (Config.JAIL_DISABLE_TRANSACTION)
			{
				creature.setInsideZone(ZoneId.NO_STORE, true);
			}
		}
	}

	protected override void onExit(Creature creature)
	{
        Player? player = creature.getActingPlayer();
		if (creature.isPlayer() && player != null)
		{
			player.setInsideZone(ZoneId.JAIL, false);
			player.setInsideZone(ZoneId.NO_SUMMON_FRIEND, false);

			if (Config.JAIL_IS_PVP)
			{
				creature.setInsideZone(ZoneId.PVP, false);
				creature.sendPacket(SystemMessageId.YOU_HAVE_LEFT_A_COMBAT_ZONE);
			}

			if (player.isJailed())
			{
				// when a player wants to exit jail even if he is still jailed, teleport him back to jail
				ThreadPool.schedule(new TeleportTask(player, JAIL_IN_LOC), 2000);
				creature.sendMessage("You cannot cheat your way out of here. You must wait until your jail time is over.");
			}
			if (Config.JAIL_DISABLE_TRANSACTION)
			{
				creature.setInsideZone(ZoneId.NO_STORE, false);
			}
		}
	}

	public static Location getLocationIn()
	{
		return JAIL_IN_LOC;
	}

	public static Location getLocationOut()
	{
		return JAIL_OUT_LOC;
	}
}