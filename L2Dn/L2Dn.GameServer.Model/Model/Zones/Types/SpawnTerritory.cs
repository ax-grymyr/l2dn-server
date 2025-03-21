using L2Dn.GameServer.Dto.ZoneForms;
using L2Dn.GameServer.Geo;
using L2Dn.GameServer.InstanceManagers;
using L2Dn.GameServer.Model.ItemContainers;
using L2Dn.GameServer.StaticData.Xml.Common;
using L2Dn.GameServer.StaticData.Xml.Zones;
using L2Dn.GameServer.Utilities;
using L2Dn.Geometry;

namespace L2Dn.GameServer.Model.Zones.Types;

/// <summary>
/// Just dummy zone, needs only for geometry calculations.
/// </summary>
public sealed class SpawnTerritory(string name, ZoneForm territory)
{
    private readonly string _name = string.IsNullOrEmpty(name)
        ? throw new ArgumentException("Spawn territory name cannot be empty") : name;

    public SpawnTerritory(XmlZone xmlZone): this(xmlZone.Name, ZoneForm.Create(xmlZone))
    {
    }

    public string getName() => _name;

    public Location3D getRandomPoint()
    {
        Location3D point = territory.GetRandomPoint();
        return point with { Z = GeoEngine.getInstance().getHeight(point) };
    }

    public bool isInsideZone(int x, int y, int z)
    {
        return territory.IsInsideZone(new Location3D(x, y, z));
    }

    public void visualizeZone(int z)
    {
        foreach (Location3D point in territory.GetVisualizationPoints(z))
            ZoneManager.Instance.DropDebugItem(Inventory.AdenaId, 1, point.X, point.Y, point.Z);
    }
}