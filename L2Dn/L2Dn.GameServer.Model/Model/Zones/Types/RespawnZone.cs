using L2Dn.GameServer.Enums;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Utilities;

namespace L2Dn.GameServer.Model.Zones.Types;

/**
 * Respawn zone implementation.
 * @author Nyaran
 */
public class RespawnZone : ZoneRespawn
{
	private readonly Map<Race, String> _raceRespawnPoint = new();
	
	public RespawnZone(int id):base(id)
	{
	}
	
	protected override void onEnter(Creature creature)
	{
	}
	
	protected override void onExit(Creature creature)
	{
	}
	
	public void addRaceRespawnPoint(String race, String point)
	{
		_raceRespawnPoint.put(Enum.Parse<Race>(race), point);
	}
	
	public Map<Race, String> getAllRespawnPoints()
	{
		return _raceRespawnPoint;
	}
	
	public String getRespawnPoint(Player player)
	{
		return _raceRespawnPoint.get(player.getRace());
	}
}