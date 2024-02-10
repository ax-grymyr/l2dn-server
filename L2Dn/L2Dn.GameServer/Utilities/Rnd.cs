namespace L2Dn.GameServer.Utilities;

public static class Rnd
{
    [ThreadStatic]
    private static readonly Random _random = new();
    
    public static int get(int maxValue)
    {
        return _random.Next(maxValue);
    }
    
    public static double get(double maxValue)
    {
        return _random.NextDouble() * maxValue;
    }
    
    public static int get(int minValue, int maxValue)
    {
        return _random.Next(minValue, maxValue);
    }

    public static long get(long minValue, long maxValue)
    {
        return _random.NextInt64(minValue, maxValue);
    }

    public static double nextDouble()
    {
        return _random.NextDouble();
    }

    public static bool nextBoolean()
    {
        return _random.Next(1) != 0;
    }
}