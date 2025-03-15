using L2Dn.GameServer.Dto.ZoneForms;
using L2Dn.GameServer.Geo;
using L2Dn.GameServer.InstanceManagers;
using L2Dn.GameServer.Model.ItemContainers;
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
        Location3D point = _territory.GetRandomPoint();
        return point with { Z = GeoEngine.getInstance().getHeight(point) };
	}

	public bool isInsideZone(int x, int y, int z)
	{
		return _territory.IsInsideZone(x, y, z);
	}

	public void visualizeZone(int z)
	{
        foreach (Location3D point in _territory.GetVisualizationPoints(z))
            ZoneManager.getInstance().DropDebugItem(Inventory.ADENA_ID, 1, point.X, point.Y, point.Z);
	}
}