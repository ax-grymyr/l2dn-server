using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Utilities;

namespace L2Dn.GameServer.Model.Zones.Types;

/**
 * @author BiggBoss
 */
public class SiegableHallZone : ClanHallZone
{
	private List<Location> _challengerLocations;
	
	public SiegableHallZone(int id): base(id)
	{
	}
	
	public override void parseLoc(int x, int y, int z, String type)
	{
		if ((type != null) && type.equals("challenger"))
		{
			if (_challengerLocations == null)
			{
				_challengerLocations = new();
			}
			_challengerLocations.add(new Location(x, y, z));
		}
		else
		{
			base.parseLoc(x, y, z, type);
		}
	}
	
	public List<Location> getChallengerSpawns()
	{
		return _challengerLocations;
	}
	
	public void banishNonSiegeParticipants()
	{
		foreach (Player player in getPlayersInside())
		{
			if ((player != null) && player.isInHideoutSiege())
			{
				player.teleToLocation(getBanishSpawnLoc(), true);
			}
		}
	}
}