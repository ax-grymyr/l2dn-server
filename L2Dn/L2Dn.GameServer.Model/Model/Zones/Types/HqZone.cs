using L2Dn.GameServer.Dto.ZoneForms;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.StaticData.Xml.Zones;
using L2Dn.GameServer.Utilities;

namespace L2Dn.GameServer.Model.Zones.Types;

/**
 * Zone where 'Build Headquarters' is allowed.
 * @author Gnacik
 */
public class HqZone(int id, ZoneForm form): Zone(id, form)
{
    public override void setParameter(XmlZoneStatName name, string value)
	{
		if (XmlZoneStatName.castleId == name)
		{
			// TODO
		}
		else if (XmlZoneStatName.fortId == name)
		{
			// TODO
		}
		else if (XmlZoneStatName.clanHallId == name)
		{
			// TODO
		}
		else if (XmlZoneStatName.territoryId == name)
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