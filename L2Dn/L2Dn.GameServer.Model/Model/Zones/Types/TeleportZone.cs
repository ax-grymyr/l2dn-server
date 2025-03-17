using L2Dn.GameServer.Dto.ZoneForms;
using L2Dn.GameServer.Enums;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.StaticData.Xml.Zones;
using L2Dn.Geometry;

namespace L2Dn.GameServer.Model.Zones.Types;

/**
 * @author Sdw
 */
public class TeleportZone: Zone
{
	private int _x = -1;
	private int _y = -1;
	private int _z = -1;

    public TeleportZone(int id, ZoneForm form): base(id, form)
    {
        setTargetType(InstanceType.Player); // Default only player.
    }

    public override void setParameter(XmlZoneStatName name, string value)
	{
		switch (name)
		{
			case XmlZoneStatName.oustX:
			{
				_x = int.Parse(value);
				break;
			}
			case XmlZoneStatName.oustY:
			{
				_y = int.Parse(value);
				break;
			}
			case XmlZoneStatName.oustZ:
			{
				_z = int.Parse(value);
				break;
			}
			default:
			{
				base.setParameter(name, value);
				break;
			}
		}
	}

	protected override void onEnter(Creature creature)
	{
		if (isEnabled())
		{
			creature.teleToLocation(new Location(_x, _y, _z, 0));
		}
	}

	protected override void onExit(Creature creature)
	{
	}
}