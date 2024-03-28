namespace L2Dn.GameServer.Model.Zones.Types;

/**
 * Just dummy zone, needs only for geometry calculations
 * @author UnAfraid
 */
public class BannedSpawnTerritory
{
	private readonly String _name;
	private readonly ZoneForm _territory;
	
	public BannedSpawnTerritory(String name, ZoneForm territory)
	{
		_name = name;
		_territory = territory;
	}
	
	public String getName()
	{
		return _name;
	}
	
	public Location getRandomPoint()
	{
		return _territory.getRandomPoint();
	}
	
	public bool isInsideZone(int x, int y, int z)
	{
		return _territory.isInsideZone(x, y, z);
	}
	
	public void visualizeZone(int z)
	{
		_territory.visualizeZone(z);
	}
}