using L2Dn.GameServer.Dto.ZoneForms;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.StaticData.Xml.Zones;
using L2Dn.GameServer.Utilities;

namespace L2Dn.GameServer.Model.Zones.Types;

/**
 * A castle zone
 * @author durgus
 */
public class CastleZone(int id, ZoneForm form): ResidenceZone(id, form)
{
    public override void setParameter(XmlZoneStatName name, string value)
	{
		if (name == XmlZoneStatName.castleId)
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
		creature.setInsideZone(ZoneId.CASTLE, true);
	}

	protected override void onExit(Creature creature)
	{
		creature.setInsideZone(ZoneId.CASTLE, false);
	}
}