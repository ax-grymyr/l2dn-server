using L2Dn.Packages.DatDefinitions.Annotations;

namespace L2Dn.Packages.DatDefinitions.Definitions;

[ChronicleRange(Chronicles.Lindvior, Chronicles.Helios - 1)]
public sealed class ServerNameV2
{
    [ArrayLengthType(ArrayLengthType.Int32)]
    public ServerNameRecord[] Records { get; set; } = Array.Empty<ServerNameRecord>();

    public sealed class ServerNameRecord
    {
        public uint Id { get; set; }

        [ArrayLengthType(ArrayLengthType.Int32)]
        public string[] Name { get; set; } = Array.Empty<string>();

        public string Description { get; set; } = string.Empty;
    }
}