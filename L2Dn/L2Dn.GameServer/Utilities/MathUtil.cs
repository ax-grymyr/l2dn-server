namespace L2Dn.GameServer.Utilities;

public static class MathUtil
{
    public static double toRadians(double degrees)
    {
        return degrees * Math.PI / 180.0;
    }

    public static double hypot(double x, double y)
    {
        return Math.Sqrt(x * x + y * y);
    }

    public static double toDegrees(double radians)
    {
        return radians * 180.0 / Math.PI;
    }
}