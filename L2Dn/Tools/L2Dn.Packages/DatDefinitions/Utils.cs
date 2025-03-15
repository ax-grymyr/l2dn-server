using System.Globalization;
using System.Reflection;

namespace L2Dn.Packages.DatDefinitions;

internal static class Utils
{
    internal static T? GetCustomAttribute<T>(this ICustomAttributeProvider? attributeProvider)
        where T: Attribute
    {
        if (attributeProvider is null)
            return null;

        object[] attributes = attributeProvider.GetCustomAttributes(typeof(T), false);
        if (attributes.Length != 0)
            return (T)attributes[0];

        return null;
    }
    
    internal static bool ValuesEqual(object? left, object? right)
    {
        if (ReferenceEquals(left, right))
            return true;
        
        if (left is null)
            return right is null;

        if (right is null)
            return false;

        if (left.GetType() == right.GetType())
            return left.Equals(right);

        object right1 = Convert.ChangeType(right, left.GetType(), CultureInfo.InvariantCulture);
        return left.Equals(right1);
    }
}