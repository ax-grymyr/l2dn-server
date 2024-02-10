using System.Numerics;
using System.Runtime.CompilerServices;

namespace L2Dn;

public static class BitUtility
{
    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public static int Compare<T>(T a, T b)
        where T: struct, IComparisonOperators<T, T, bool>
    {
        bool c1 = a > b;
        bool c2 = a < b;
        return Unsafe.As<bool, byte>(ref c1) - Unsafe.As<bool, byte>(ref c2); 
    }
}
