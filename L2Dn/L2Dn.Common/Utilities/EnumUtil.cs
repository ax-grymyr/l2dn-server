using System.Collections.Immutable;

namespace L2Dn.Utilities;

public static class EnumUtil
{
    public static ImmutableArray<TEnum> GetValues<TEnum>()
        where TEnum: unmanaged, Enum => EnumInfo<TEnum>.Values;

    public static TEnum GetMaxValue<TEnum>()
        where TEnum: unmanaged, Enum => EnumInfo<TEnum>.MaxValue;

    private static class EnumInfo<TEnum>
        where TEnum: unmanaged, Enum
    {
        public static ImmutableArray<TEnum> Values { get; }
        public static TEnum MaxValue { get; }

        static EnumInfo()
        {
            Values = Enum.GetValues<TEnum>().ToImmutableArray();
            MaxValue = Values.Length > 0 ? Values.Max() : default;
        }
    }
}