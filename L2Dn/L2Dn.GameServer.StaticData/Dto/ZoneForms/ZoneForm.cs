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

    protected bool IsInsideBounds(int x, int y, int z) => Bounds.Contains(x, y) && z >= LowZ && z <= HighZ;

    public abstract bool IsInsideZone(int x, int y, int z);

    public abstract bool IntersectsRectangle(int x1, int x2, int y1, int y2);

    public abstract double GetDistanceToZone(int x, int y);

    public abstract IEnumerable<Location3D> GetVisualizationPoints(int z);

    public abstract Location3D GetRandomPoint();
}