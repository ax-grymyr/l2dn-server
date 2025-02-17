using System.Numerics;
using System.Runtime.CompilerServices;

namespace L2Dn;

public static class ComparableExtensions
{
    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public static int CompareFast<T>(this T value1, T value2)
        where T: IComparisonOperators<T, T, bool> =>
        (value1 > value2 ? 1 : 0) - (value1 < value2 ? 1 : 0);
}