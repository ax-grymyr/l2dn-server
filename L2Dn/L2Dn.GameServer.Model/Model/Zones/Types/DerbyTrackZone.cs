using L2Dn.GameServer.Dto.ZoneForms;
using L2Dn.GameServer.Model.Actor;

namespace L2Dn.GameServer.Model.Zones.Types;

/**
 * The Monster Derby Track Zone
 * @author durgus
 */
public class DerbyTrackZone(int id, ZoneForm form): ZoneType(id, form)
{
    protected override void onEnter(Creature creature)
	{
		if (creature.isPlayable())
		{
			creature.setInsideZone(ZoneId.MONSTER_TRACK, true);
		}
	}

	protected override void onExit(Creature creature)
	{
		if (creature.isPlayable())
		{
			creature.setInsideZone(ZoneId.MONSTER_TRACK, false);
		}
	}
}