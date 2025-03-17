using L2Dn.Geometry;
using L2Dn.Utilities;

namespace L2Dn.GameServer.Dto.ZoneForms;

/// <summary>
/// A primitive circular zone.
/// </summary>
public sealed class ZoneCylinder(Location2D center, int z1, int z2, int rad):
    ZoneForm(new Rectangle(center.X - rad, center.Y - rad, 2 * rad, 2 * rad), z1, z2)
{
    private readonly int _radS = rad * rad;

    public override bool IsInsideZone(Location3D location)
    {
        return IsInsideBounds(location) &&
            location.DistanceSquare2D(new Location2D(Bounds.X + Bounds.Width / 2, Bounds.Y + Bounds.Height / 2)) <=
            _radS;
    }

    public override bool IntersectsRectangle(Rectangle rectangle)
    {
        // Circles point inside the rectangle?
        if (!Bounds.Intersects(rectangle))
            return false;

        int ax1 = rectangle.X;
        int ax2 = rectangle.X + rectangle.Width - 1;
        int ay1 = rectangle.Y;
        int ay2 = rectangle.Y + rectangle.Height - 1;

        // Any point of the rectangle intersecting the Circle?
        if (center.DistanceSquare2D(new Location2D(ax1, ay1)) <= _radS)
            return true;

        if (center.DistanceSquare2D(new Location2D(ax1, ay2)) <= _radS)
            return true;

        if (center.DistanceSquare2D(new Location2D(ax2, ay1)) <= _radS)
            return true;

        if (center.DistanceSquare2D(new Location2D(ax2, ay2)) <= _radS)
            return true;

        // Collision on any side of the rectangle?
        if (center.X > ax1 && center.X < ax2)
        {
            if (Math.Abs(center.Y - ay2) < rad)
                return true;

            if (Math.Abs(center.Y - ay1) < rad)
                return true;
        }

        if (center.Y > ay1 && center.Y < ay2)
        {
            if (Math.Abs(center.X - ax2) < rad)
                return true;

            if (Math.Abs(center.X - ax1) < rad)
                return true;
        }

        return false;
    }

    public override double GetDistanceToZone(Location2D location) => center.Distance2D(location) - rad;

    public override IEnumerable<Location3D> GetVisualizationPoints(int z)
    {
        int count = (int)(2 * Math.PI * rad / Step);
        double angle = 2 * Math.PI / count;
        for (int i = 0; i < count; i++)
            yield return new Location3D(center.X + (int)(Math.Cos(angle * i) * rad),
                center.Y + (int)(Math.Sin(angle * i) * rad), z);
    }

    public override Location3D GetRandomPoint()
    {
        Location2D p1 = Bounds.LeftTop;
        Location2D p2 = Bounds.RightBottom;
        Location2D p = new Location2D(Rnd.get(p1.X, p2.X), Rnd.get(p1.Y, p2.Y));
        while (center.DistanceSquare2D(p) > _radS)
            p = new Location2D(Rnd.get(p1.X, p2.X), Rnd.get(p1.Y, p2.Y));

        return new Location3D(p, (LowZ + HighZ) / 2);
    }
}