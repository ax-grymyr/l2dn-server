using L2Dn.GameServer.Data.Xml;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Residences;
using L2Dn.GameServer.Utilities;

namespace L2Dn.GameServer.Model.Zones.Types;

/**
 * A clan hall zone
 * @author durgus
 */
public class ClanHallZone : ResidenceZone
{
	public ClanHallZone(int id): base(id)
	{
	}
	
	public void setParameter(String name, String value)
	{
		if (name.equals("clanHallId"))
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
		if (creature.isPlayer())
		{
			creature.setInsideZone(ZoneId.CLAN_HALL, true);
		}
	}
	
	protected override void onExit(Creature creature)
	{
		if (creature.isPlayer())
		{
			creature.setInsideZone(ZoneId.CLAN_HALL, false);
		}
	}
	
	public Location getBanishSpawnLoc()
	{
		ClanHall clanHall = ClanHallData.getInstance().getClanHallById(getResidenceId());
		if (clanHall == null)
		{
			return null;
		}
		return clanHall.getBanishLocation();
	}
}