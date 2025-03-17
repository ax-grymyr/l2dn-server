using System.Collections.Immutable;
using L2Dn.GameServer.Dto.ZoneForms;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.StaticData.Xml.Zones;
using L2Dn.Geometry;

namespace L2Dn.GameServer.Model.Zones.Types;

/**
 * @author BiggBoss
 */
public sealed class SiegableHallZone(int id, ZoneForm form): ClanHallZone(id, form)
{
	private ImmutableArray<Location3D> _challengerLocations = ImmutableArray<Location3D>.Empty;

	public override void parseLoc(Location3D location, XmlZoneSpawnType type)
	{
		if (type == XmlZoneSpawnType.challenger)
		{
			_challengerLocations = _challengerLocations.Add(location);
		}
		else
		{
			base.parseLoc(location, type);
		}
	}

	public ImmutableArray<Location3D> getChallengerSpawns()
	{
		return _challengerLocations;
	}

	public void banishNonSiegeParticipants()
	{
		foreach (Player player in getPlayersInside())
		{
			if (player != null && player.isInHideoutSiege())
			{
				player.teleToLocation(new Location(getBanishSpawnLoc(), 0), true);
			}
		}
	}
}