using L2Dn.Geometry;
using L2Dn.Utilities;

namespace L2Dn.GameServer.Dto.ZoneForms;

/// <summary>
/// A primitive rectangular zone.
/// </summary>
public sealed class ZoneCuboid: ZoneForm
{
    private readonly Rectangle _rectangle;
    private readonly int _z1;
    private readonly int _z2;

    public ZoneCuboid(int x1, int x2, int y1, int y2, int z1, int z2)
    {
        int minX = Math.Min(x1, x2);
        int maxX = Math.Max(x1, x2);
        int minY = Math.Min(y1, y2);
        int maxY = Math.Max(y1, y2);
        _rectangle = new Rectangle(minX, minY, maxX - minX, maxY - minY);
        _z1 = Math.Min(z1, z2);
        _z2 = Math.Max(z1, z2);
    }

    public override bool IsInsideZone(int x, int y, int z) => _rectangle.Contains(x, y) && z >= _z1 && z <= _z2;

    public override bool IntersectsRectangle(int ax1, int ax2, int ay1, int ay2) =>
        _rectangle.Intersects(Math.Min(ax1, ax2), Math.Min(ay1, ay2), Math.Abs(ax2 - ax1), Math.Abs(ay2 - ay1));

    public override double GetDistanceToZone(int x, int y)
    {
        Location2D location = new(x, y);
        return Math.Min(Math.Min(location.DistanceSquare2D(_rectangle.LeftTop),
                location.DistanceSquare2D(_rectangle.LeftBottom)),
            Math.Min(location.DistanceSquare2D(_rectangle.RightTop),
                location.DistanceSquare2D(_rectangle.RightBottom)));
    }

    /*
     * getLowZ() / getHighZ() - These two functions were added to cope with the demand of the new fishing algorithms, which are now able to correctly place the hook in the water, thanks to getHighZ(). getLowZ() was added, considering potential future modifications.
     */
    public override int GetLowZ() => _z1;
    public override int GetHighZ() => _z2;

    public override IEnumerable<Location3D> GetVisualizationPoints(int z)
    {
        int minX = _rectangle.X;
        int maxX = _rectangle.X + _rectangle.Width;
        int minY = _rectangle.Y;
        int maxY = _rectangle.Y + _rectangle.Height;

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
        int x = _rectangle.X + Rnd.get(_rectangle.Width);
        int y = _rectangle.Y + Rnd.get(_rectangle.Height);
        return new Location3D(x, y, (_z1 + _z2) / 2);
    }
}