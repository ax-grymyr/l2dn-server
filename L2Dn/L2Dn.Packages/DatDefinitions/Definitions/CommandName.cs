using L2Dn.Packages.DatDefinitions.Annotations;

namespace L2Dn.Packages.DatDefinitions.Definitions;

[ChronicleRange(Chronicles.ScionsOfDestiny, Chronicles.HighFive - 1)]
public sealed class CommandName
{
    [ArrayLengthType(ArrayLengthType.Int32)]
    public CommandNameRecord[] Records { get; set; } = Array.Empty<CommandNameRecord>();

    public sealed class CommandNameRecord
    {
        public uint Number { get; set; }
        public uint Id { get; set; }
        public string Cmd { get; set; } = string.Empty;
    }
}