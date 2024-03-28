using System.Reflection;
using System.Text;
using L2Dn.IO;
using L2Dn.Packages.DatDefinitions.Annotations;

namespace L2Dn.Packages.DatDefinitions;

public static class DatReader
{
    public static T Read<T>(string filePath)
    {
        using EncryptedStream stream = EncryptedStream.OpenRead(filePath);
        DatBinaryReader reader = new DatBinaryReader(stream);
        return (T)ReadValue(typeof(T), reader);
    }

    private static object ReadValue(Type type, DatBinaryReader reader, ArrayLengthTypeAttribute? attribute = null)
    {
        if (type == typeof(int))
            return reader.ReadInt32();

        if (type == typeof(long))
            return reader.ReadInt64();

        if (type == typeof(float))
            return reader.ReadFloat();

        if (type == typeof(string))
            return reader.ReadDatString();

        if (type.IsArray)
        {
            int length = attribute != null && attribute.Type == ArrayLengthType.Int32
                ? reader.ReadInt32()
                : reader.ReadIndex();

            Type elementType = type.GetElementType()!;
            Array array = Array.CreateInstance(elementType, length);
            for (int i = 0; i < length; i++)
            {
                object value = ReadValue(elementType, reader);
                array.SetValue(value, i);
            }

            return array;
        }

        if (type.IsClass)
        {
            object obj = Activator.CreateInstance(type);
            foreach (PropertyInfo property in obj.GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public))
            {
                ArrayLengthTypeAttribute? attr = property.GetCustomAttribute<ArrayLengthTypeAttribute>();
                object value = ReadValue(property.PropertyType, reader, attr);

                string len = property.PropertyType.IsArray ? $" (len = {((Array)value).Length})" : "";
                File.AppendAllText(@"D:\read.txt", $"{type.Name}.{property.Name} = {value}{len}\n\r", Encoding.UTF8);
                
                property.SetValue(obj, value);
            }

            return obj;
        }

        throw new NotSupportedException();
    }
}