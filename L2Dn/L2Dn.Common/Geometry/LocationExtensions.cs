using L2Dn.Utilities;

namespace L2Dn.Geometry;

public static class LocationExtensions
{
    public static double Distance2D<TFrom, TTo>(this TFrom from, TTo to)
        where TFrom: ILocation2D
        where TTo: ILocation2D
    {
        double dx = (double)from.X - to.X;
        double dy = (double)from.Y - to.Y;
        return double.Sqrt(dx * dx + dy * dy);
    }

    public static double Distance3D<TFrom, TTo>(this TFrom from, TTo to)
        where TFrom: ILocation3D
        where TTo: ILocation3D
    {
        double dx = (double)from.X - to.X;
        double dy = (double)from.Y - to.Y;
        double dz = (double)from.Z - to.Z;
        return double.Sqrt(dx * dx + dy * dy + dz * dz);
    }

    public static double DistanceSquare2D<TFrom, TTo>(this TFrom from, TTo to)
        where TFrom: ILocation2D
        where TTo: ILocation2D
    {
        double dx = (double)from.X - to.X;
        double dy = (double)from.Y - to.Y;
        return dx * dx + dy * dy;
    }

    public static double DistanceSquare3D<TFrom, TTo>(this TFrom from, TTo to)
        where TFrom: ILocation3D
        where TTo: ILocation3D
    {
        double dx = (double)from.X - to.X;
        double dy = (double)from.Y - to.Y;
        double dz = (double)from.Z - to.Z;
        return dx * dx + dy * dy + dz * dz;
    }

    public static bool IsInsideRadius2D<TFrom, TTo>(this TFrom from, TTo to, int radius)
        where TFrom: ILocation2D
        where TTo: ILocation2D
    {
        return Distance2D(from, to) <= radius;
    }

    public static bool IsInsideRadius3D<TFrom, TTo>(this TFrom from, TTo to, int radius)
        where TFrom: ILocation3D
        where TTo: ILocation3D
    {
        return Distance3D(from, to) <= radius;
    }

    public static int HeadingTo<TFrom, TTo>(this TFrom from, TTo to)
        where TFrom: ILocation2D
        where TTo: ILocation2D
    {
        return (int)(AngleDegreesTo(from, to) * 182.044444444);
    }

    public static double AngleRadiansTo<TFrom, TTo>(this TFrom from, TTo to)
        where TFrom: ILocation2D
        where TTo: ILocation2D
    {
        const double twoPi = double.Pi * 2.0;
        double angle = double.Atan2(to.Y - from.Y, to.X - from.X);
        return angle < 0 ? angle + twoPi : angle;
    }

    public static double AngleDegreesTo<TFrom, TTo>(this TFrom from, TTo to)
        where TFrom: ILocation2D
        where TTo: ILocation2D
    {
        double angle = double.RadiansToDegrees(double.Atan2(to.Y - from.Y, to.X - from.X));
        return angle < 0 ? angle + 360.0 : angle;
    }

    public static Location2D RandomPosition2D<TOrigin>(this TOrigin location, int minRange, int maxRange)
        where TOrigin: ILocation2D
    {
        int randomX = Rnd.get(minRange, maxRange);
        int randomY = Rnd.get(minRange, maxRange);
        double rndAngle = double.DegreesToRadians(Rnd.get(360));
        int newX = (int)(location.X + randomX * Math.Cos(rndAngle));
        int newY = (int)(location.Y + randomY * Math.Sin(rndAngle));
        return new Location2D(newX, newY);
    }

    public static Location3D RandomPosition3D<TOrigin>(this TOrigin location, int minRange, int maxRange)
        where TOrigin: ILocation3D
    {
        Location2D randomLocation = RandomPosition2D(location, minRange, maxRange);
        return new Location3D(randomLocation.X, randomLocation.Y, location.Z);
    }

    /// <summary>
    /// Position calculation based on the retail-like formulas.
    /// </summary>
    /// <param name="attackerLocation"></param>
    /// <param name="targetLocation"></param>
    /// <typeparam name="TFrom"></typeparam>
    /// <typeparam name="TTo"></typeparam>
    /// <returns></returns>
    public static Position PositionTo<TFrom, TTo>(this TFrom attackerLocation, TTo targetLocation)
        where TFrom: ILocation3D
        where TTo: ILocationHeading
    {
        // heading: (unsigned short) abs(heading - (unsigned short)(int)floor(atan2(toY - fromY, toX - fromX) * 65535.0 / 6.283185307179586))
        // side: if (heading >= 0x2000 && heading <= 0x6000 || (unsigned int)(heading - 0xA000) <= 0x4000)
        // front: else if ((unsigned int)(heading - 0x2000) <= 0xC000)
        // back: otherwise.
        int heading = Math.Abs(targetLocation.Heading - attackerLocation.HeadingTo(targetLocation));
        if (heading is >= 0x2000 and <= 0x6000 || (uint)(heading - 0xA000) <= 0x4000)
            return Position.Side;

        return (uint)(heading - 0x2000) <= 0xC000 ? Position.Front : Position.Back;
    }

    public static bool IsInFrontOf<TFrom, TTo>(this TFrom attackerLocation, TTo targetLocation)
        where TFrom: ILocation3D
        where TTo: ILocationHeading => attackerLocation.PositionTo(targetLocation) == Position.Front;

    public static bool IsOnSideOf<TFrom, TTo>(this TFrom attackerLocation, TTo targetLocation)
        where TFrom: ILocation3D
        where TTo: ILocationHeading => attackerLocation.PositionTo(targetLocation) == Position.Side;

    public static bool IsBehindOf<TFrom, TTo>(this TFrom attackerLocation, TTo targetLocation)
        where TFrom: ILocation3D
        where TTo: ILocationHeading => attackerLocation.PositionTo(targetLocation) == Position.Back;
}