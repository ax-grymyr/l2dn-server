using System.Text.Json;
using System.Text.Json.Serialization;

namespace L2Dn.Packages.DatDefinitions;

public sealed class IndexedStringConverter: JsonConverter<IndexedString>
{
    public override IndexedString Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        throw new NotSupportedException();
    }

    public override void Write(Utf8JsonWriter writer, IndexedString value, JsonSerializerOptions options)
    {
        writer.WriteStartObject();
        writer.WritePropertyName("NameDataIndex");
        writer.WriteNumberValue(value.Index);
        writer.WritePropertyName("NameDataString");
        writer.WriteStringValue(value.ToString());
        writer.WriteEndObject();
    }
}