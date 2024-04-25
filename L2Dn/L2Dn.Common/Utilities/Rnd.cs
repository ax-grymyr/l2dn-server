namespace L2Dn.Utilities;

public static class Rnd
{
    public static int get(int maxValue)
    {
        return Random.Shared.Next(maxValue);
    }
    
    public static long get(long maxValue)
    {
        return Random.Shared.NextInt64(maxValue);
    }
    
    public static double get(double maxValue)
    {
        return Random.Shared.NextDouble() * maxValue;
    }
    
    public static int get(int minValue, int maxValue)
    {
        return Random.Shared.Next(minValue, maxValue);
    }

    public static double get(double minValue, double maxValue)
    {
        return minValue + Random.Shared.NextDouble() * (maxValue - minValue);
    }

    public static long get(long minValue, long maxValue)
    {
        return Random.Shared.NextInt64(minValue, maxValue);
    }

    public static TimeSpan get(TimeSpan minValue, TimeSpan maxValue)
    {
        return TimeSpan.FromTicks(get(minValue.Ticks, maxValue.Ticks));
    }

    public static double nextDouble()
    {
        return Random.Shared.NextDouble();
    }

    public static bool nextBoolean()
    {
        return Random.Shared.Next(1) != 0;
    }
}