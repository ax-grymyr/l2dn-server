using L2Dn.Packages.DatDefinitions.Annotations;

namespace L2Dn.Packages.DatDefinitions.Definitions;

[ChronicleRange(Chronicles.MasterClass, Chronicles.MasterClass2 - 1)]
public sealed class ServerNameV4
{
    [ArrayLengthType(ArrayLengthType.Int32)]
    public ServerNameRecord[] Records { get; set; } = Array.Empty<ServerNameRecord>();

    public sealed class ServerNameRecord
    {
        public uint Id { get; set; }
        public uint ExtId { get; set; }
        public uint Priority { get; set; }
        public string Name { get; set; } = string.Empty;
        public string MarkName { get; set; } = string.Empty;

        [ArrayLengthType(ArrayLengthType.Fixed, 4)]
        public string[] Description { get; set; } = Array.Empty<string>();
    }
}