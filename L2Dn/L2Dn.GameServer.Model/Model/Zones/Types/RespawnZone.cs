using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Utilities;
using L2Dn.Model.Enums;

namespace L2Dn.GameServer.Model.Zones.Types;

/**
 * Respawn zone implementation.
 * @author Nyaran
 */
public class RespawnZone(int id, ZoneForm form): ZoneRespawn(id, form)
{
	private readonly Map<Race, string> _raceRespawnPoint = new();

    protected override void onEnter(Creature creature)
	{
	}

	protected override void onExit(Creature creature)
	{
	}

	public void addRaceRespawnPoint(Race race, string point)
	{
		_raceRespawnPoint.put(race, point);
	}

	public Map<Race, string> getAllRespawnPoints()
	{
		return _raceRespawnPoint;
	}

	public string? getRespawnPoint(Player player)
	{
		return _raceRespawnPoint.get(player.getRace());
	}
}