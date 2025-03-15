using L2Dn.GameServer.Dto.ZoneForms;
using L2Dn.GameServer.Model.Actor;

namespace L2Dn.GameServer.Model.Zones.Types;

/**
 * A simple no summon zone
 * @author JIV
 */
public class NoSummonFriendZone(int id, ZoneForm form): ZoneType(id, form)
{
    protected override void onEnter(Creature creature)
	{
		creature.setInsideZone(ZoneId.NO_SUMMON_FRIEND, true);
	}

	protected override void onExit(Creature creature)
	{
		creature.setInsideZone(ZoneId.NO_SUMMON_FRIEND, false);
	}
}