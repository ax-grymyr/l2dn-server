using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Utilities;

namespace L2Dn.GameServer.Model.Zones.Types;

/**
 * Zone where 'Build Headquarters' is allowed.
 * @author Gnacik
 */
public class HqZone : ZoneType
{
	public HqZone(int id):base(id)
	{
	}
	
	public override void setParameter(String name, String value)
	{
		if ("castleId".equals(name))
		{
			// TODO
		}
		else if ("fortId".equals(name))
		{
			// TODO
		}
		else if ("clanHallId".equals(name))
		{
			// TODO
		}
		else if ("territoryId".equals(name))
		{
			// TODO
		}
		else
		{
			base.setParameter(name, value);
		}
	}
	
	protected override void onEnter(Creature creature)
	{
		if (creature.isPlayer())
		{
			creature.setInsideZone(ZoneId.HQ, true);
		}
	}
	
	protected override void onExit(Creature creature)
	{
		if (creature.isPlayer())
		{
			creature.setInsideZone(ZoneId.HQ, false);
		}
	}
}