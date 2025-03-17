using L2Dn.GameServer.Dto.ZoneForms;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.StaticData.Xml.Zones;
using L2Dn.GameServer.Utilities;
using L2Dn.Geometry;

namespace L2Dn.GameServer.Model.Zones.Types;

/**
 * based on Kerberos work for custom CastleTeleportZone
 * @author Nyaran
 */
public class ResidenceTeleportZone(int id, ZoneForm form): ZoneRespawn(id, form)
{
	private int _residenceId;

    public override void setParameter(XmlZoneStatName name, string value)
	{
		if (name == XmlZoneStatName.residenceId)
		{
			_residenceId = int.Parse(value);
		}
		else
		{
			base.setParameter(name, value);
		}
	}

	protected override void onEnter(Creature creature)
	{
		creature.setInsideZone(ZoneId.NO_SUMMON_FRIEND, true); // FIXME: Custom ?
	}

	protected override void onExit(Creature creature)
	{
		creature.setInsideZone(ZoneId.NO_SUMMON_FRIEND, false); // FIXME: Custom ?
	}

	public override void oustAllPlayers()
	{
		foreach (Player player in getPlayersInside())
		{
			if (player != null && player.isOnline())
			{
				player.teleToLocation(new Location(getSpawnLoc(), 0), 200);
			}
		}
	}

	public int getResidenceId()
	{
		return _residenceId;
	}
}