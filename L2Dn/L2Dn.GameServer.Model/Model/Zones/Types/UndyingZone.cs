using L2Dn.GameServer.Dto.ZoneForms;
using L2Dn.GameServer.Model.Actor;

namespace L2Dn.GameServer.Model.Zones.Types;

/**
 * An Undying Zone
 * @author UnAfraid
 */
public class UndyingZone(int id, ZoneForm form): Zone(id, form)
{
    protected override void onEnter(Creature creature)
	{
		creature.setInsideZone(ZoneId.UNDYING, true);
	}

	protected override void onExit(Creature creature)
	{
		creature.setInsideZone(ZoneId.UNDYING, false);
	}
}