using System.Collections.Immutable;
using System.Runtime.InteropServices;
using L2Dn.Geometry;
using L2Dn.Utilities;

namespace L2Dn.GameServer.Dto.ZoneForms;

/// <summary>
/// A not so primitive N-polygon zone.
/// </summary>
public sealed class ZoneNPoly(ImmutableArray<Location2D> points, int z1, int z2):
    ZoneForm(CalculateBounds(points), z1, z2)
{
    private readonly Polygon _polygon = new(points);

    public ImmutableArray<Location2D> Points => _polygon.Points;

    public override bool IsInsideZone(Location3D location) =>
        IsInsideBounds(location) && _polygon.Contains(location.Location2D);

    public override bool IntersectsRectangle(Rectangle rectangle) =>
        Bounds.Intersects(rectangle) && _polygon.Intersects(rectangle);

    public override double GetDistanceToZone(Location2D location)
    {
        // TODO: should also the distance to an edge considered? not only the distance to points
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
        int x = Bounds.X + Rnd.get(Bounds.Width);
        int y = Bounds.Y + Rnd.get(Bounds.Height);

        int antiBlocker = 0;
        while (!_polygon.Contains(new Location2D(x, y)) && antiBlocker++ < 1000)
        {
            x = Bounds.X + Rnd.get(Bounds.Width);
            y = Bounds.Y + Rnd.get(Bounds.Height);
        }

        return new Location3D(x, y, (LowZ + HighZ) / 2);
    }

    private static Rectangle CalculateBounds(ImmutableArray<Location2D> points)
    {
        Location2D[] array = ImmutableCollectionsMarshal.AsArray(points)!;
        int boundsMinX = array.Min(p => p.X);
        int boundsMaxX = array.Max(p => p.X);

        int boundsMinY = array.Min(p => p.Y);
        int boundsMaxY = array.Max(p => p.Y);

        return new Rectangle(boundsMinX, boundsMinY, boundsMaxX - boundsMinX, boundsMaxY - boundsMinY);
    }
}