namespace L2Dn.GameServer.Utilities;

public static class MathUtil
{
    public static double hypot(double x, double y)
    {
        return Math.Sqrt(x * x + y * y);
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