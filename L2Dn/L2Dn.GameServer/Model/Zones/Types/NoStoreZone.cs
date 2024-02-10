using L2Dn.GameServer.Model.Actor;

namespace L2Dn.GameServer.Model.Zones.Types;

/**
 * Zone where store is not allowed.
 * @author fordfrog
 */
public class NoStoreZone : ZoneType
{
	public NoStoreZone(int id):base(id)
	{
	}
	
	protected override void onEnter(Creature creature)
	{
		if (creature.isPlayer())
		{
			creature.setInsideZone(ZoneId.NO_STORE, true);
		}
	}
	
	protected override void onExit(Creature creature)
	{
		if (creature.isPlayer())
		{
			creature.setInsideZone(ZoneId.NO_STORE, false);
		}
	}
}
