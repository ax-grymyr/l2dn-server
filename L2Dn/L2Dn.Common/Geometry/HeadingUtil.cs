namespace L2Dn.Geometry;

public static class HeadingUtil
{
    public static double ConvertHeadingToDegrees(int heading) => heading / 182.044444444;

    public static double ConvertHeadingToRadians(int heading) => double.DegreesToRadians(heading / 182.044444444);

    public static int CalculateHeading(double dx, double dy)
    {
        double angle = double.RadiansToDegrees(Math.Atan2(dy, dx));
        return (int)((angle < 0 ? angle + 360.0 : angle) * 182.044444444);
    }
}