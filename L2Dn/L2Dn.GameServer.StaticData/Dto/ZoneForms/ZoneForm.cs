using System.Collections.Immutable;
using L2Dn.GameServer.Enums;
using L2Dn.GameServer.StaticData.Xml.Zones;
using L2Dn.Geometry;

namespace L2Dn.GameServer.Dto.ZoneForms;

/// <summary>
/// Abstract base class for any zone form.
/// </summary>
public abstract class ZoneForm(Rectangle bounds, int z1, int z2)
{
    protected const int Step = 10;

    public Rectangle Bounds { get; } = bounds;
    public int LowZ { get; } = Math.Min(z1, z2);
    public int HighZ { get; } = Math.Max(z1, z2);

    protected bool IsInsideBounds(Location3D location) =>
        Bounds.Contains(location.Location2D) && location.Z >= LowZ && location.Z <= HighZ;

    public abstract bool IsInsideZone(Location3D location);

    public abstract bool IntersectsRectangle(Rectangle rectangle);

    public abstract double GetDistanceToZone(Location2D location);

    public abstract IEnumerable<Location3D> GetVisualizationPoints(int z);

    public abstract Location3D GetRandomPoint();

    public static ZoneForm Create(XmlZoneArea zoneArea)
    {
        ImmutableArray<Location2D> nodes = zoneArea.Nodes.Select(node => new Location2D(node.X, node.Y)).
            ToImmutableArray();

        if (nodes.IsDefaultOrEmpty)
            throw new ArgumentException("Invalid zone form, missing nodes");

        // Create this zone. Parsing for cuboids is a bit different than for other polygons cuboids need
        // exactly 2 points to be defined. Other polygons need at least 3 (one per vertex).
        switch (zoneArea.Shape)
        {
            case ZoneShape.Cuboid:
            {
                if (nodes.Length != 2)
                    throw new ArgumentException("Invalid zone form, cuboid must have exactly 2 nodes.");

                return new ZoneCuboid(nodes[0], nodes[1], zoneArea.MinZ, zoneArea.MaxZ);
            }

            case ZoneShape.Cylinder:
            {
                if (nodes.Length != 1)
                    throw new ArgumentException("Invalid zone form, cylinder must have exactly one node.");

                return new ZoneCylinder(nodes[0], zoneArea.MinZ, zoneArea.MaxZ, zoneArea.Radius);
            }

            case ZoneShape.NPoly:
            {
                if (nodes.Length < 3)
                    throw new ArgumentException("Invalid zone form, npoly must have at least 3 nodes.");

                return new ZoneNPoly(nodes, zoneArea.MinZ, zoneArea.MaxZ);
            }

            default:
                throw new ArgumentException($"Unknown zone shape: {zoneArea.Shape}.");
        }
    }
}