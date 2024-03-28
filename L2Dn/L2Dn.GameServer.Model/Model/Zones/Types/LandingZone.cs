using L2Dn.GameServer.Model.Actor;

namespace L2Dn.GameServer.Model.Zones.Types;

/**
 * A landing zone
 * @author Kerberos
 */
public class LandingZone : ZoneType
{
	public LandingZone(int id):base(id)
	{
	}
	
	protected override void onEnter(Creature creature)
	{
		if (creature.isPlayer())
		{
			creature.setInsideZone(ZoneId.LANDING, true);
		}
	}
	
	protected override void onExit(Creature creature)
	{
		if (creature.isPlayer())
		{
			creature.setInsideZone(ZoneId.LANDING, false);
		}
	}
}