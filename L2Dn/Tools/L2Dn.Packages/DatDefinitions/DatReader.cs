using System.Collections.Immutable;
using System.Globalization;
using System.Reflection;
using L2Dn.IO;
using L2Dn.Packages.DatDefinitions.Annotations;

namespace L2Dn.Packages.DatDefinitions;

public static class DatReader
{
    private static ImmutableArray<string> _nameData = ImmutableArray<string>.Empty;
    
    public static L2NameData ReadNameData(string filePath)
    {
        L2NameData data = Read<L2NameData>(filePath);
        _nameData = data.Names.ToImmutableArray();
        return data;
    }
    
    public static void SetNameData(IReadOnlyCollection<string> nameData)
    {
        _nameData = nameData.ToImmutableArray();
    }

    public static void ClearNameData()
    {
        _nameData = ImmutableArray<string>.Empty;
    }
    
    public static T[] ReadArray<T>(string filePath, ArrayLengthType lengthType = ArrayLengthType.Int32, int size = -1)
    {
        using EncryptedStream stream = EncryptedStream.OpenRead(filePath);
        DatBinaryReader reader = new DatBinaryReader(stream);
        int length = ReadArrayLength(null, reader, new ArrayLengthTypeAttribute(lengthType, size));

        T[] array = new T[length];
        for (int i = 0; i < length; i++)
            array[i] = (T)ReadValue(null, typeof(T), reader, null);
        
        return array;
    }

    public static object Read(Type objectType, string filePath)
    {
        using EncryptedStream stream = EncryptedStream.OpenRead(filePath);
        DatBinaryReader reader = new DatBinaryReader(stream);
        return ReadValue(null, objectType, reader, null);
    }

    public static object Read(Type objectType, Stream stream)
    {
        DatBinaryReader reader = new DatBinaryReader(stream);
        return ReadValue(null, objectType, reader, null);
    }

    public static T Read<T>(string filePath) => (T)Read(typeof(T), filePath);
    public static T Read<T>(Stream stream) => (T)Read(typeof(T), stream);

    private static object ReadValue(object? obj, Type type, DatBinaryReader reader, ICustomAttributeProvider? attributeProvider)
    {
        if (type == typeof(int))
            return reader.ReadInt32();

        if (type == typeof(uint))
            return reader.ReadUInt32();

        if (type == typeof(short))
            return reader.ReadInt16();

        if (type == typeof(ushort))
            return reader.ReadUInt16();

        if (type == typeof(byte))
            return reader.ReadByte();

        if (type == typeof(long))
            return reader.ReadInt64();

        if (type == typeof(float))
            return reader.ReadFloat();

        if (type == typeof(IndexedString))
        {
            int index = reader.ReadInt32();
            return new IndexedString(_nameData[index], index);
        }
        
        if (type == typeof(string))
        {
            StringType stringType = attributeProvider.GetCustomAttribute<StringTypeAttribute>()?.Type ?? StringType.Ascf;
            return stringType switch
            {
                StringType.Ascf => reader.ReadString(),
                StringType.Utf16Le => reader.ReadUtfString(),
                _ => throw new NotSupportedException()
            };
        }

        if (type.IsEnum)
        {
            EnumValueTypeAttribute? enumValueTypeAttribute =
                attributeProvider.GetCustomAttribute<EnumValueTypeAttribute>();

            object value;
            if (enumValueTypeAttribute != null)
            {
                value = enumValueTypeAttribute.Type switch
                {
                    EnumValueType.Byte => reader.ReadByte(),
                    EnumValueType.Int16 => reader.ReadInt16(),
                    EnumValueType.Int32 => reader.ReadInt32(),
                    EnumValueType.Int64 => reader.ReadInt64(),
                    _ => throw new NotSupportedException()
                };
            }
            else
            {
                value = ReadValue(obj, Enum.GetUnderlyingType(type), reader, attributeProvider);
            }

            return Enum.ToObject(type, value);
        }
        
        if (type.IsArray)
            return ReadArray(obj, type.GetElementType()!, reader, attributeProvider);

        if (type.IsClass)
            return ReadObject(type, reader);

        throw new NotSupportedException();
    }

    private static object ReadObject(Type classType, DatBinaryReader reader)
    {
        object obj = Activator.CreateInstance(classType)!;
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
            
            object value = ReadValue(obj, property.PropertyType, reader, property);
            property.SetValue(obj, value);

            //string len = property.PropertyType.IsArray ? $" (len = {((Array)value).Length})" : "";
            //File.AppendAllText(@"D:\read.txt", $"{type.Name}.{property.Name} = {value}{len}\n\r", Encoding.UTF8);
        }

        return obj;
    }
    
    private static object ReadArray(object? obj, Type elementType, DatBinaryReader reader, ICustomAttributeProvider? attributeProvider)
    {
        ArrayLengthTypeAttribute? attr = attributeProvider.GetCustomAttribute<ArrayLengthTypeAttribute>();
        int length = ReadArrayLength(obj, reader, attr);
        Array array = Array.CreateInstance(elementType, length);
        for (int i = 0; i < length; i++)
        {
            object value = ReadValue(obj, elementType, reader, attributeProvider);
            array.SetValue(value, i);
        }

        return array;
    }

    private static int ReadArrayLength(object? obj, DatBinaryReader reader, ArrayLengthTypeAttribute? attribute)
    {
        if (attribute is null)
            return reader.ReadIndex();

        return attribute.Type switch
        {
            ArrayLengthType.CompactInt => reader.ReadIndex(),
            ArrayLengthType.Int32 => reader.ReadInt32(),
            ArrayLengthType.Int16 => reader.ReadInt16(),
            ArrayLengthType.Byte => reader.ReadByte(),
            ArrayLengthType.Fixed => attribute.Size,
            ArrayLengthType.PropertyRef => ((Array)(obj?.GetType().GetProperty(attribute.ArrayPropertyName!)
                                                        ?.GetValue(obj) ??
                                                    throw new NotSupportedException())).Length,
            
            _ => throw new NotSupportedException()
        };
    }
}