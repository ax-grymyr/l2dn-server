using System.Numerics;

namespace L2Dn.Utilities;

public static class Algorithms
{
    public static T Max<T>(T arg1, T arg2)
        where T: IComparable<T>
    {
        return arg1.CompareTo(arg2) > 0 ? arg1 : arg2;
    }
 
    public static TimeSpan Abs(TimeSpan arg)
    {
        if (arg < TimeSpan.Zero)
            return -arg;
        
        return arg;
    }
}