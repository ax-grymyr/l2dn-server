using System.Globalization;
using System.Reflection;
using L2Dn.Packages.DatDefinitions.Annotations;

namespace L2Dn.Packages.DatDefinitions;

public static class DatWriter
{
    public static void Write(string filePath, object obj, bool isSafePackage = true)
    {
        // TODO: for now, I write only non-encrypted data, use l2encdec to encrypt
        using FileStream stream = new FileStream(filePath, FileMode.Create, FileAccess.Write, FileShare.None);
        DatBinaryWriter writer = new DatBinaryWriter(stream);
        WriteObject(writer, obj);
        
        if (isSafePackage)
            writer.WriteString("SafePackage");
    }

    private static void WriteObject(DatBinaryWriter writer, object obj)
    {
        Type classType = obj.GetType();
        foreach (PropertyInfo property in classType.GetProperties(BindingFlags.Instance | BindingFlags.Public))
        {
            ConditionAttribute? conditionAttribute = property.GetCustomAttribute<ConditionAttribute>();
            if (conditionAttribute != null)
            {
                PropertyInfo? conditionProperty = classType.GetProperty(conditionAttribute.PropertyName,
                    BindingFlags.Instance | BindingFlags.Public);

                if (conditionProperty is null)
                    throw new InvalidOperationException("Condition property not found");

                object? conditionPropertyValue = conditionProperty.GetValue(obj);
                object? conditionValue = conditionAttribute.Value;
                if (!Utils.ValuesEqual(conditionPropertyValue, conditionValue))
                    continue;
            }

            object? value = property.GetValue(obj);
            WriteValue(obj, writer, property, value);
        }
    }

    private static void WriteValue(object? obj, DatBinaryWriter writer, ICustomAttributeProvider? attributeProvider, object? value)
    {
        switch (value)
        {
            case null:
                throw new NotSupportedException();
            
            case int i:
                writer.WriteInt32(i);
                return;
            case uint i:
                writer.WriteUInt32(i);
                return;
            case short i:
                writer.WriteInt16(i);
                return;
            case ushort i:
                writer.WriteUInt16(i);
                return;
            case byte i:
                writer.WriteByte(i);
                return;
            case long i:
                writer.WriteInt64(i);
                return;
            case float i:
                writer.WriteFloat(i);
                return;
            case IndexedString s:
                writer.WriteInt32(s.Index);
                return;
            case string s:
                StringType stringType = attributeProvider.GetCustomAttribute<StringTypeAttribute>()?.Type ?? StringType.Ascf;
                switch (stringType)
                {
                    case StringType.Ascf:
                        writer.WriteString(s);
                        return;
                    case StringType.Utf16Le:
                        writer.WriteUtfString(s);
                        return;
                    default:
                        throw new NotSupportedException();
                }
                
            case Array a:
                WriteArray(obj, writer, attributeProvider, a);
                return;
                
            default:
                if (value.GetType().IsEnum)
                {
                    WriteEnum(obj, writer, attributeProvider, value);
                    return;
                }
                
                if (value.GetType().IsClass)
                {
                    WriteObject(writer, value);
                    return;
                }

                throw new NotSupportedException();
        }
    }

    private static void WriteEnum(object? obj, DatBinaryWriter writer, ICustomAttributeProvider? attributeProvider, object value)
    {
        EnumValueTypeAttribute? enumValueTypeAttribute =
            attributeProvider.GetCustomAttribute<EnumValueTypeAttribute>();

        if (enumValueTypeAttribute != null)
        {
            long val = Convert.ToInt64(value, CultureInfo.InvariantCulture);
            switch (enumValueTypeAttribute.Type)
            {
                case EnumValueType.Byte:
                    writer.WriteByte((byte)val);
                    break;
                    
                case EnumValueType.Int16:
                    writer.WriteInt16((short)val);
                    break;

                case EnumValueType.Int32:
                    writer.WriteInt32((int)val);
                    break;
                    
                case EnumValueType.Int64:
                    writer.WriteInt64(val);
                    break;
                
                default:
                    throw new NotSupportedException();
            };
        }
        else
        {
            WriteValue(obj, writer, attributeProvider,
                Convert.ChangeType(value, Enum.GetUnderlyingType(value.GetType()), CultureInfo.InvariantCulture));
        }
    }

    private static void WriteArray(object? obj, DatBinaryWriter writer, ICustomAttributeProvider? attributeProvider, Array array)
    {
        ArrayLengthTypeAttribute? attr = attributeProvider.GetCustomAttribute<ArrayLengthTypeAttribute>();
        if (attr is null)
            writer.WriteIndex(array.Length);
        else
        {
            switch (attr.Type)
            {
                case ArrayLengthType.CompactInt:
                    writer.WriteIndex(array.Length);
                    break;
                case ArrayLengthType.Int32:
                    writer.WriteInt32(array.Length);
                    break;
                case ArrayLengthType.Int16:
                    writer.WriteInt16(checked((short)array.Length));
                    break;
                case ArrayLengthType.Byte:
                    writer.WriteByte(checked((byte)array.Length));
                    break;
                case ArrayLengthType.Fixed:
                    if (array.Length != attr.Size)
                        throw new InvalidOperationException("Invalid array size");
                    
                    break;

                case ArrayLengthType.PropertyRef:
                {
                    if (obj is null)
                        throw new InvalidOperationException("Invalid property ref: no object");
                    
                    if (attr.ArrayPropertyName is null)
                        throw new InvalidOperationException("Invalid property ref: no ArrayPropertyName");

                    PropertyInfo? propertyInfo = obj.GetType().GetProperty(attr.ArrayPropertyName);
                    if (propertyInfo is null)
                        throw new InvalidOperationException($"Invalid property ref: no property {attr.ArrayPropertyName}");

                    Array? arr = propertyInfo.GetValue(obj) as Array;
                    if (arr is null)
                        throw new InvalidOperationException($"Invalid property ref: {attr.ArrayPropertyName} is not array");
                    
                    if (array.Length != arr.Length)
                        throw new InvalidOperationException("Invalid array size");

                    break;
                }
                
                default:
                    throw new NotSupportedException();
            }
        }
        
        for (int i = 0; i < array.Length; i++)
        {
            object? value = array.GetValue(i);
            WriteValue(obj, writer, attributeProvider, value);
        }
    }
}