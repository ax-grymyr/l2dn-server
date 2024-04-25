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
        double angleTarget = double.RadiansToDegrees(Math.Atan2((double)to.Y - from.Y, (double)to.X - from.X));
        if (angleTarget < 0)
            angleTarget += 360;

        return (int)(angleTarget * 182.044444444);
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
}