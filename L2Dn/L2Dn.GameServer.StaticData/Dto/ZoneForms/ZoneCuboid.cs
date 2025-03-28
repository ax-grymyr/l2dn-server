﻿using L2Dn.Geometry;
using L2Dn.Utilities;

namespace L2Dn.GameServer.Dto.ZoneForms;

/// <summary>
/// A primitive rectangular zone.
/// </summary>
public sealed class ZoneCuboid(Location2D point1, Location2D point2, int z1, int z2)
    : ZoneForm(GetRectangle(point1, point2), z1, z2)
{
    private static Rectangle GetRectangle(Location2D point1, Location2D point2)
    {
        int minX = Math.Min(point1.X, point2.X);
        int maxX = Math.Max(point1.X, point2.X);
        int minY = Math.Min(point1.Y, point2.Y);
        int maxY = Math.Max(point1.Y, point2.Y);
        return new Rectangle(minX, minY, maxX - minX, maxY - minY);
    }

    public override bool IsInsideZone(Location3D location) => IsInsideBounds(location);

    public override bool IntersectsRectangle(Rectangle rectangle) => Bounds.Intersects(rectangle);

    public override double GetDistanceToZone(Location2D location)
    {
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