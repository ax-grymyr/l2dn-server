using L2Dn.GameServer.Model.Actor;

namespace L2Dn.GameServer.Model.Zones.Types;

/**
 * A Peace Zone
 * @author durgus
 */
public class PeaceZone : ZoneType
{
	public PeaceZone(int id): base(id)
	{
	}

	protected override void onEnter(Creature creature)
	{
		if (!isEnabled())
		{
			return;
		}

		if (creature.isPlayer())
		{
			Player player = creature.getActingPlayer();
			// PVP possible during siege, now for siege participants only
			// Could also check if this town is in siege, or if any siege is going on
			if ((player.getSiegeState() != 0) && (Config.PEACE_ZONE_MODE == 1))
			{
				return;
			}
		}

		if (Config.PEACE_ZONE_MODE != 2)
		{
			creature.setInsideZone(ZoneId.PEACE, true);
		}

		if (!getAllowStore())
		{
			creature.setInsideZone(ZoneId.NO_STORE, true);
		}

		// Send player info to nearby players.
		if (creature.isPlayer())
		{
			creature.broadcastInfo();
		}
	}

	protected override void onExit(Creature creature)
	{
		if (Config.PEACE_ZONE_MODE != 2)
		{
			creature.setInsideZone(ZoneId.PEACE, false);
		}

		if (!getAllowStore())
		{
			creature.setInsideZone(ZoneId.NO_STORE, false);
		}

		// Send player info to nearby players.
		if (creature.isPlayer() && !creature.isTeleporting())
		{
			creature.broadcastInfo();
		}
	}

	public override void setEnabled(bool value)
	{
		base.setEnabled(value);
		if (value)
		{
			foreach (Player player in World.getInstance().getPlayers())
			{
				if ((player != null) && isInsideZone(player))
				{
					revalidateInZone(player);

					if (player.getPet() != null)
					{
						revalidateInZone(player.getPet());
					}

					foreach (Summon summon in player.getServitors().Values)
					{
						revalidateInZone(summon);
					}
				}
			}
		}
		else
		{
			foreach (Creature creature in getCharactersInside())
			{
				if (creature != null)
				{
					removeCharacter(creature);
				}
			}
		}
	}
}