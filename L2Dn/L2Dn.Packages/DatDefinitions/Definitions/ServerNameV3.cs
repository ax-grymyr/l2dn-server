using L2Dn.Packages.DatDefinitions.Annotations;

namespace L2Dn.Packages.DatDefinitions.Definitions;

[ChronicleRange(Chronicles.Helios, Chronicles.MasterClass - 1)]
public sealed class ServerNameV3
{
    [ArrayLengthType(ArrayLengthType.Int32)]
    public ServerNameRecord[] Records { get; set; } = Array.Empty<ServerNameRecord>();

    public sealed class ServerNameRecord
    {
        public uint Id { get; set; }

        [ArrayLengthType(ArrayLengthType.Int32)]
        public string[] Name { get; set; } = Array.Empty<string>();
    }
}