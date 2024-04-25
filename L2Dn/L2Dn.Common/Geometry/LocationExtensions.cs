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
}