using System.Collections.Immutable;

namespace L2Dn.Geometry;

public readonly struct Polygon
{
    public ImmutableArray<Location2D> Points { get; }

    public Polygon(ImmutableArray<Location2D> points)
    {
        if (points.IsDefaultOrEmpty || points.Length < 3)
            throw new ArgumentException("Polygon must have at least 3 points.");

        Points = points;
    }

    public bool Contains(Location2D location)
    {
        // Check if the point lies on a vertex
        foreach (Location2D point in Points)
        {
            if (point == location)
                return true;
        }

        bool inside = false;
        Location2D last = Points[^1];
        foreach (Location2D point in Points)
        {
            // Check if the point lies on an edge
            if (location.X >= Math.Min(point.X, last.X) && location.X <= Math.Max(point.X, last.X) &&
                location.Y >= Math.Min(point.Y, last.Y) && location.Y <= Math.Max(point.Y, last.Y))
            {
                long crossProduct = (long)(location.X - point.X) * (last.Y - point.Y) -
                    (long)(location.Y - point.Y) * (last.X - point.X);

                if (crossProduct == 0) // Check for collinearity
                    return true;
            }

            bool intersect = (point.Y > location.Y) != (last.Y > location.Y) &&
                (location.X < (last.X - point.X) * (location.Y - point.Y) / (1.0 * (last.Y - point.Y)) + point.X);

            if (intersect)
                inside = !inside;

            last = point;
        }

        return inside;
    }

    public bool Intersects(Rectangle rectangle)
    {
        // Check if any polygon point is inside the rectangle
        foreach (Location2D point in Points)
        {
            if (rectangle.Contains(point))
                return true;
        }

        // Check if any rectangle corner is inside the polygon
        if (Contains(rectangle.LeftTop) || Contains(rectangle.LeftBottom) || Contains(rectangle.RightTop) ||
            Contains(rectangle.RightBottom))
        {
            return true;
        }

        // Check if any polygon edge intersects any rectangle edge
        Location2D last = Points[^1];
        foreach (Location2D point in Points)
        {
            if (LinesIntersect(point, last, rectangle.LeftTop, rectangle.RightTop) ||
                LinesIntersect(point, last, rectangle.RightTop, rectangle.RightBottom) ||
                LinesIntersect(point, last, rectangle.RightBottom, rectangle.LeftBottom) ||
                LinesIntersect(point, last, rectangle.LeftBottom, rectangle.LeftTop))
            {
                return true;
            }

            last = point;
        }

        return false;
    }

    private static bool LinesIntersect(Location2D p1, Location2D p2, Location2D q1, Location2D q2)
    {
        int Orientation(Location2D a, Location2D b, Location2D c)
        {
            long val = (long)(b.Y - a.Y) * (c.X - b.X) - (long)(b.X - a.X) * (c.Y - b.Y);
            return val == 0 ? 0 : (val > 0 ? 1 : -1);
        }

        int o1 = Orientation(p1, p2, q1);
        int o2 = Orientation(p1, p2, q2);
        int o3 = Orientation(q1, q2, p1);
        int o4 = Orientation(q1, q2, p2);
        return o1 != o2 && o3 != o4;
    }
}