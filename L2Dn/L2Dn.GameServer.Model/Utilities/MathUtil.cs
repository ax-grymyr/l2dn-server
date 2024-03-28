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

    /**
     * @param objectsSize : The overall elements size.
     * @param pageSize : The number of elements per page.
     * @return The number of pages, based on the number of elements and the number of elements we want per page.
     */
    public static int countPagesNumber(int objectsSize, int pageSize)
    {
        return (objectsSize / pageSize) + ((objectsSize % pageSize) == 0 ? 0 : 1);
    }
}