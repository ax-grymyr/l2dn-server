using System.Globalization;
using System.Xml.Linq;

namespace L2Dn.Utilities;

public static class XmlUtil
{
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