using System.Reflection;

namespace L2Dn.Extensions;

public static class EnumExtension
{
    public static TAttribute? GetCustomAttribute<TEnum, TAttribute>(this TEnum value)
        where TEnum: struct, Enum
        where TAttribute: Attribute
    {
        Type enumType = value.GetType();
        string? name = Enum.GetName(value);
        if (name is null)
            return null;

        return enumType.GetField(name)?.GetCustomAttribute<TAttribute>();
    }
}