using L2Dn.Packages.DatDefinitions.Annotations;
using L2Dn.Packages.DatDefinitions.Definitions.Shared;

namespace L2Dn.Packages.DatDefinitions.Definitions;

[ChronicleRange(Chronicles.ScionsOfDestiny, Chronicles.Interlude - 1)]
public sealed class SystemMsg
{
    [ArrayLengthType(ArrayLengthType.Int32)]
    public SystemMsgRecord[] Records { get; set; } = Array.Empty<SystemMsgRecord>();

    public sealed class SystemMsgRecord
    {
        public uint Id { get; set; }
        public uint Unknown1 { get; set; }
        public string Message { get; set; } = string.Empty;
        public uint Group { get; set; }
        public RgbColor Color { get; set; } = new RgbColor();
        public uint Unknown2 { get; set; }
        public string Sound { get; set; } = string.Empty;
        public string ScreenMessage { get; set; } = string.Empty;
    }
}