using L2Dn.Geometry;
using L2Dn.Utilities;

namespace L2Dn.GameServer.Dto.ZoneForms;

/// <summary>
/// A primitive rectangular zone.
/// </summary>
public sealed class ZoneCuboid(int x1, int x2, int y1, int y2, int z1, int z2)
    : ZoneForm(GetRectangle(x1, x2, y1, y2), z1, z2)
{
    private static Rectangle GetRectangle(int x1, int y1, int x2, int y2)
    {
        int minX = Math.Min(x1, x2);
        int maxX = Math.Max(x1, x2);
        int minY = Math.Min(y1, y2);
        int maxY = Math.Max(y1, y2);
        return new Rectangle(minX, minY, maxX - minX, maxY - minY);
    }

    public override bool IsInsideZone(int x, int y, int z) => IsInsideBounds(x, y, z);

    public override bool IntersectsRectangle(int ax1, int ax2, int ay1, int ay2) =>
        Bounds.Intersects(Math.Min(ax1, ax2), Math.Min(ay1, ay2), Math.Abs(ax2 - ax1), Math.Abs(ay2 - ay1));

    public override double GetDistanceToZone(int x, int y)
    {
        Location2D location = new(x, y);
        return Math.Min(Math.Min(location.DistanceSquare2D(Bounds.LeftTop),
                location.DistanceSquare2D(Bounds.LeftBottom)),
            Math.Min(location.DistanceSquare2D(Bounds.RightTop),
                location.DistanceSquare2D(Bounds.RightBottom)));
    }

    public override IEnumerable<Location3D> GetVisualizationPoints(int z)
    {
        (int minX, int minY, int width, int height) = Bounds;
        int maxX = minX + width;
        int maxY = minY + height;

        // x1->x2
        for (int x = minX; x < maxX; x += Step)
        {
            yield return new Location3D(x, minY, z);
            yield return new Location3D(x, maxY, z);
        }

        // y1->y2
        for (int y = minY; y < maxY; y += Step)
        {
            yield return new Location3D(minX, y, z);
            yield return new Location3D(maxX, y, z);
        }
    }

    public override Location3D GetRandomPoint()
    {
        (int minX, int minY, int width, int height) = Bounds;
        int x = minX + Rnd.get(width);
        int y = minY + Rnd.get(height);
        return new Location3D(x, y, (LowZ + HighZ) / 2);
    }
}