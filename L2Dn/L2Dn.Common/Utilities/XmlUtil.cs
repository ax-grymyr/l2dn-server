using System.Globalization;
using System.Xml.Linq;

namespace L2Dn.Utilities;

public static class XmlUtil
{
    public static byte GetByte(this XAttribute? attribute) => GetValue<byte>(attribute);
    public static byte GetByte(this XAttribute? attribute, byte defaultValue) => GetValue(attribute, defaultValue);
    public static int GetInt32(this XAttribute? attribute) => GetValue<int>(attribute);
    public static int GetInt32(this XAttribute? attribute, int defaultValue) => GetValue(attribute, defaultValue);
    public static long GetInt64(this XAttribute? attribute) => GetValue<long>(attribute);
    public static long GetInt64(this XAttribute? attribute, long defaultValue) => GetValue(attribute, defaultValue);
    public static float GetFloat(this XAttribute? attribute) => GetValue<float>(attribute);
    public static float GetFloat(this XAttribute? attribute, float defaultValue) => GetValue(attribute, defaultValue);
    public static double GetDouble(this XAttribute? attribute) => GetValue<double>(attribute);

    public static double GetDouble(this XAttribute? attribute, double defaultValue) =>
        GetValue(attribute, defaultValue);

    public static string GetString(this XAttribute? attribute) => attribute is null
        ? throw new InvalidOperationException("Attribute missing")
        : attribute.Value;

    public static string GetString(this XAttribute? attribute, string defaultValue) => 
        attribute is null ? defaultValue : attribute.Value;

    public static T GetAttributeValue<T>(this XElement element, string attributeName, T defaultValue)
        where T: struct, IParsable<T>
    {
        XAttribute? attribute = element.Attribute(attributeName);
        if (attribute is null)
            return defaultValue;

        string value = attribute.Value;
        if (T.TryParse(value, CultureInfo.InvariantCulture, out T result))
            return result;
        
        throw new InvalidOperationException($"Invalid attribute '{attributeName}' value in '{element}'");
    }

    public static T? GetAttributeValueOrNull<T>(this XElement element, string attributeName)
        where T: struct, IParsable<T>
    {
        XAttribute? attribute = element.Attribute(attributeName);
        if (attribute is null)
            return null;

        string value = attribute.Value;
        if (T.TryParse(value, CultureInfo.InvariantCulture, out T result))
            return result;
        
        throw new InvalidOperationException($"Invalid attribute '{attributeName}' value in '{element}'");
    }

    public static T GetAttributeValue<T>(this XElement element, string attributeName)
        where T: struct, IParsable<T>
    {
        XAttribute? attribute = element.Attribute(attributeName);
        if (attribute is null)
            throw new InvalidOperationException($"Attribute '{attributeName}' missing in '{element}'");

        string value = attribute.Value;
        if (T.TryParse(value, CultureInfo.InvariantCulture, out T result))
            return result;
        
        throw new InvalidOperationException($"Invalid attribute '{attributeName}' value in '{element}'");
    }

    public static int GetAttributeValueAsInt32(this XElement element, string attributeName) => element.GetAttributeValue<int>(attributeName);
    public static int GetAttributeValueAsInt32(this XElement element, string attributeName, int defaultValue) => element.GetAttributeValue(attributeName, defaultValue);
    public static int? GetAttributeValueAsInt32OrNull(this XElement element, string attributeName) => element.GetAttributeValueOrNull<int>(attributeName);
    public static long GetAttributeValueAsInt64(this XElement element, string attributeName) => element.GetAttributeValue<long>(attributeName);
    public static long GetAttributeValueAsInt64(this XElement element, string attributeName, long defaultValue) => element.GetAttributeValue(attributeName, defaultValue);
    public static long? GetAttributeValueAsInt64OrNull(this XElement element, string attributeName) => element.GetAttributeValueOrNull<long>(attributeName);
    public static float GetAttributeValueAsFloat(this XElement element, string attributeName) => element.GetAttributeValue<float>(attributeName);
    public static float GetAttributeValueAsFloat(this XElement element, string attributeName, float defaultValue) => element.GetAttributeValue(attributeName, defaultValue);
    public static float? GetAttributeValueAsFloatOrNull(this XElement element, string attributeName) => element.GetAttributeValueOrNull<float>(attributeName);
    public static double GetAttributeValueAsDouble(this XElement element, string attributeName) => element.GetAttributeValue<double>(attributeName);
    public static double GetAttributeValueAsDouble(this XElement element, string attributeName, double defaultValue) => element.GetAttributeValue(attributeName, defaultValue);
    public static double? GetAttributeValueAsDoubleOrNull(this XElement element, string attributeName) => element.GetAttributeValueOrNull<double>(attributeName);

    public static TEnum GetAttributeValueAsEnum<TEnum>(this XElement element, string attributeName)
        where TEnum: struct, Enum
    {
        XAttribute? attribute = element.Attribute(attributeName);
        if (attribute is null)
            throw new InvalidOperationException($"Attribute '{attributeName}' missing in '{element}'");

        string value = attribute.Value;
        if (Enum.TryParse<TEnum>(value, false, out TEnum result))
            return result;
        
        throw new InvalidOperationException($"Invalid attribute '{attributeName}' value in '{element}'");
    }

    public static TEnum GetAttributeValueAsEnum<TEnum>(this XElement element, string attributeName, TEnum defaultValue)
        where TEnum: struct, Enum, IParsable<TEnum>
    {
        XAttribute? attribute = element.Attribute(attributeName);
        if (attribute is null)
            return defaultValue;

        string value = attribute.Value;
        if (Enum.TryParse<TEnum>(value, false, out TEnum result))
            return result;
        
        throw new InvalidOperationException($"Invalid attribute '{attributeName}' value in '{element}'");
    }

    public static TEnum? GetAttributeValueAsEnumOrNull<TEnum>(this XElement element, string attributeName)
        where TEnum: struct, Enum, IParsable<TEnum>
    {
        XAttribute? attribute = element.Attribute(attributeName);
        if (attribute is null)
            return null;

        string value = attribute.Value;
        if (Enum.TryParse<TEnum>(value, false, out TEnum result))
            return result;
        
        throw new InvalidOperationException($"Invalid attribute '{attributeName}' value in '{element}'");
    }
    
    public static string GetAttributeValueAsString(this XElement element, string attributeName)
    {
        XAttribute? attribute = element.Attribute(attributeName);
        if (attribute is null)
            throw new InvalidOperationException($"Attribute '{attributeName}' missing in '{element}'");

        return attribute.Value;
    }

    public static string GetAttributeValueAsString(this XElement element, string attributeName, string defaultValue) =>
        element.Attribute(attributeName)?.Value ?? defaultValue;

    public static bool GetAttributeValueAsBoolean(this XElement element, string attributeName)
    {
        XAttribute? attribute = element.Attribute(attributeName);
        if (attribute is null)
            throw new InvalidOperationException($"Attribute '{attributeName}' missing in '{element}'");

        string value = attribute.Value;
        if (bool.TryParse(value, out bool result))
            return result;
        
        throw new InvalidOperationException($"Invalid attribute '{attributeName}' value in '{element}'");
    }

    public static bool? GetAttributeValueAsBooleanOrNull(this XElement element, string attributeName)
    {
        XAttribute? attribute = element.Attribute(attributeName);
        if (attribute is null)
            return null;

        string value = attribute.Value;
        if (bool.TryParse(value, out bool result))
            return result;
        
        throw new InvalidOperationException($"Invalid attribute '{attributeName}' value in '{element}'");
    }
    
    public static T GetValue<T>(this XAttribute? attribute)
        where T: struct, IParsable<T>
    {
        if (attribute is null)
            throw new InvalidOperationException("Attribute missing");

        string value = attribute.Value;
        if (T.TryParse(value, CultureInfo.InvariantCulture, out T result))
            return result;
        
        throw new InvalidOperationException("Invalid attribute value");
    }

    public static T GetValue<T>(this XAttribute? attribute, T defaultValue)
        where T: IParsable<T>
    {
        if (attribute is null)
            return defaultValue;

        string value = attribute.Value;
        if (T.TryParse(value, CultureInfo.InvariantCulture, out T result))
            return result;
        
        throw new InvalidOperationException("Invalid attribute value");
    }

    public static Color GetColor(this XAttribute? attribute) => GetValue<Color>(attribute);
    public static Color GetColor(this XAttribute? attribute, Color defaultValue) => GetValue(attribute, defaultValue);

    public static bool GetBoolean(this XAttribute? attribute)
    {
        if (attribute is null)
            throw new InvalidOperationException("Attribute missing");

        string value = attribute.Value;
        if (bool.TryParse(value, out bool result))
            return result;
        
        throw new InvalidOperationException("Invalid attribute value");
    }

    public static bool GetBoolean(this XAttribute? attribute, bool defaultValue)
    {
        if (attribute is null)
            return defaultValue;

        string value = attribute.Value;
        if (bool.TryParse(value, out bool result))
            return result;
        
        throw new InvalidOperationException("Invalid attribute value");
    }

    public static T GetEnum<T>(this XAttribute? attribute)
        where T: struct, Enum
    {
        if (attribute is null)
            throw new InvalidOperationException("Attribute missing");

        string value = attribute.Value;
        if (Enum.TryParse(value, out T result))
            return result;
        
        throw new InvalidOperationException("Invalid attribute value");
    }

    public static T GetEnum<T>(this XAttribute? attribute, T defaultValue)
        where T: struct, Enum
    {
        if (attribute is null)
            return defaultValue;

        string value = attribute.Value;
        if (Enum.TryParse(value, out T result))
            return result;
        
        throw new InvalidOperationException("Invalid attribute value");
    }

    public static TimeSpan GetTimeSpan(this XAttribute? attribute)
    {
        if (attribute is null)
            throw new InvalidOperationException("Attribute missing");

        string value = attribute.Value;
        // TODO
        //if (bool.TryParse(value, out bool result))
        //    return result;
        
        throw new InvalidOperationException("Invalid attribute value");
    }

    public static TimeSpan GetTimeSpan(this XAttribute? attribute, TimeSpan defaultValue)
    {
        if (attribute is null)
            return defaultValue;

        string value = attribute.Value;
        // TODO
        //if (bool.TryParse(value, out bool result))
        //    return result;
        
        throw new InvalidOperationException("Invalid attribute value");
    }
}