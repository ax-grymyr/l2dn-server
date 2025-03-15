using System.Collections.Immutable;
using L2Dn.Geometry;
using L2Dn.Utilities;

namespace L2Dn.GameServer.Dto.ZoneForms;

/// <summary>
/// A not so primitive N-polygon zone.
/// </summary>
public sealed class ZoneNPoly(ImmutableArray<Location2D> points, int z1, int z2): ZoneForm
{
    private readonly Polygon _polygon = new(points);
    private readonly int _z1 = Math.Min(z1, z2);
    private readonly int _z2 = Math.Max(z1, z2);

    public ImmutableArray<Location2D> Points => _polygon.Points;

    public override bool IsInsideZone(int x, int y, int z)
    {
        return _polygon.Contains(x, y) && z >= _z1 && z <= _z2;
    }

    public override bool IntersectsRectangle(int ax1, int ax2, int ay1, int ay2)
    {
        return _polygon.Intersects(Math.Min(ax1, ax2), Math.Min(ay1, ay2), Math.Abs(ax2 - ax1), Math.Abs(ay2 - ay1));
    }

    public override double GetDistanceToZone(int x, int y)
    {
        // TODO: should also the distance to an edge considered? not only the distance to points
        Location2D location = new(x, y);
        ImmutableArray<Location2D> points = _polygon.Points;
        double shortestDist = points[0].DistanceSquare2D(location);
        for (int i = 1; i < points.Length; i++)
        {
            double test = points[i].DistanceSquare2D(location);
            if (test < shortestDist)
                shortestDist = test;
        }

        return Math.Sqrt(shortestDist);
    }

    // getLowZ() / getHighZ() - These two functions were added to cope with the demand of the new fishing algorithms, which are now able to correctly place the hook in the water, thanks to getHighZ(). getLowZ() was added, considering potential future modifications.
    public override int GetLowZ() => _z1;

    public override int GetHighZ() => _z2;

    public override IEnumerable<Location3D> GetVisualizationPoints(int z)
    {
        ImmutableArray<Location2D> points = _polygon.Points;
        for (int i = 0; i < points.Length; i++)
        {
            Location2D point = points[i];
            int nextIndex = i + 1 == points.Length ? 0 : i + 1;
            int vx = points[nextIndex].X - point.X;
            int vy = points[nextIndex].X - point.X;
            float length = (float)Math.Sqrt(vx * vx + vy * vy) / Step;

            for (int o = 1; o <= length; o++)
                yield return new Location3D((int)(point.X + o / length * vx), (int)(point.Y + o / length * vy), z);
        }
    }

    public override Location3D GetRandomPoint()
    {
        Rectangle bounds = _polygon.Bounds;
        int x = bounds.X + Rnd.get(bounds.Width);
        int y = bounds.Y + Rnd.get(bounds.Height);

        int antiBlocker = 0;
        while (!_polygon.Contains(x, y) && antiBlocker++ < 1000)
        {
            x = bounds.X + Rnd.get(bounds.Width);
            y = bounds.Y + Rnd.get(bounds.Height);
        }

        return new Location3D(x, y, (_z1 + _z2) / 2);
    }
}