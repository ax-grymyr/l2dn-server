using L2Dn.Geometry;

namespace L2Dn.GameServer.Model.Zones.Types;

/**
 * Just dummy zone, needs only for geometry calculations
 * @author GKR
 */
public class SpawnTerritory
{
	private readonly string _name;
	private readonly ZoneForm _territory;
	
	public SpawnTerritory(string name, ZoneForm territory)
	{
		_name = name;
		_territory = territory;
	}
	
	public string getName()
	{
		return _name;
	}
	
	public Location3D getRandomPoint()
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