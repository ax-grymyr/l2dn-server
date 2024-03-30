using L2Dn.Packages.DatDefinitions.Annotations;

namespace L2Dn.Packages.DatDefinitions.Definitions;

[ChronicleRange(Chronicles.ScionsOfDestiny, Chronicles.Lindvior - 1)]
public sealed class ServerName
{
    [ArrayLengthType(ArrayLengthType.Int32)]
    public ServerNameRecord[] Records { get; set; } = Array.Empty<ServerNameRecord>();

    public sealed class ServerNameRecord
    {
        public uint Id { get; set; }
        public uint Tag { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
    }
}