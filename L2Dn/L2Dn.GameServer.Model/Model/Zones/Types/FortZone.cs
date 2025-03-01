using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Utilities;

namespace L2Dn.GameServer.Model.Zones.Types;

/**
 * A castle zone
 * @author durgus
 */
public class FortZone(int id, ZoneForm form): ResidenceZone(id, form)
{
    public override void setParameter(string name, string value)
	{
		if (name.equals("fortId"))
		{
			setResidenceId(int.Parse(value));
		}
		else
		{
			base.setParameter(name, value);
		}
	}

	protected override void onEnter(Creature creature)
	{
		creature.setInsideZone(ZoneId.FORT, true);
	}

	protected override void onExit(Creature creature)
	{
		creature.setInsideZone(ZoneId.FORT, false);
	}
}