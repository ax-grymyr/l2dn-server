using L2Dn.Geometry;
using L2Dn.Utilities;

namespace L2Dn.GameServer.Dto.ZoneForms;

/// <summary>
/// A primitive circular zone.
/// </summary>
public sealed class ZoneCylinder(int x, int y, int z1, int z2, int rad): ZoneForm
{
    private readonly int _radS = rad * rad;

    public override bool IsInsideZone(int x1, int y1, int z)
    {
        return Math.Pow(x - x1, 2) + Math.Pow(y - y1, 2) <= _radS && z >= z1 && z <= z2;
    }

    public override bool IntersectsRectangle(int ax1, int ax2, int ay1, int ay2)
    {
        // Circles point inside the rectangle?
        if (x > ax1 && x < ax2 && y > ay1 && y < ay2)
        {
            return true;
        }

        // Any point of the rectangle intersecting the Circle?
        if (Math.Pow(ax1 - x, 2) + Math.Pow(ay1 - y, 2) < _radS)
            return true;

        if (Math.Pow(ax1 - x, 2) + Math.Pow(ay2 - y, 2) < _radS)
            return true;

        if (Math.Pow(ax2 - x, 2) + Math.Pow(ay1 - y, 2) < _radS)
            return true;

        if (Math.Pow(ax2 - x, 2) + Math.Pow(ay2 - y, 2) < _radS)
            return true;

        // Collision on any side of the rectangle?
        if (x > ax1 && x < ax2)
        {
            if (Math.Abs(y - ay2) < rad)
                return true;

            if (Math.Abs(y - ay1) < rad)
                return true;
        }

        if (y > ay1 && y < ay2)
        {
            if (Math.Abs(x - ax2) < rad)
                return true;

            if (Math.Abs(x - ax1) < rad)
                return true;
        }

        return false;
    }

    public override double GetDistanceToZone(int x1, int y1)
    {
        return double.Hypot(x - x1, y - y1) - rad;
    }

    // getLowZ() / getHighZ() - These two functions were added to cope with the demand of the new fishing algorithms, wich are now able to correctly place the hook in the water, thanks to getHighZ(). getLowZ() was added, considering potential future modifications.
    public override int GetLowZ() => z1;

    public override int GetHighZ() => z2;

    public override IEnumerable<Location3D> GetVisualizationPoints(int z)
    {
        int count = (int)(2 * Math.PI * rad / Step);
        double angle = 2 * Math.PI / count;
        for (int i = 0; i < count; i++)
            yield return new Location3D(x + (int)(Math.Cos(angle * i) * rad), y + (int)(Math.Sin(angle * i) * rad), z);
    }

    public override Location3D GetRandomPoint()
    {
        int x1 = 0;
        int y1 = 0;
        int x2 = x - rad;
        int y2 = y - rad;
        int x3 = x + rad;
        int y3 = y + rad;
        while (Math.Pow(x - x1, 2) + Math.Pow(y - y1, 2) > _radS)
        {
            x1 = Rnd.get(x2, x3);
            y1 = Rnd.get(y2, y3);
        }

        return new Location3D(x1, y1, (z1 + z2) / 2);
    }
}