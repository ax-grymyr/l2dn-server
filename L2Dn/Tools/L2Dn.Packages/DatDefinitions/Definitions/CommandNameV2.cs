using L2Dn.Packages.DatDefinitions.Annotations;

namespace L2Dn.Packages.DatDefinitions.Definitions;

[ChronicleRange(Chronicles.HighFive, Chronicles.Latest)]
public sealed class CommandNameV2
{
    [ArrayLengthType(ArrayLengthType.Int32)]
    public CommandNameRecord[] Records { get; set; } = Array.Empty<CommandNameRecord>();

    public sealed class CommandNameRecord
    {
        public uint Id { get; set; }
        public uint Action { get; set; }
        public string Cmd { get; set; } = string.Empty;
    }
}