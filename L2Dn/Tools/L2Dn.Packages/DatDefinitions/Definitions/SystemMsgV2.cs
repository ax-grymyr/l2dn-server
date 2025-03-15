using L2Dn.Packages.DatDefinitions.Annotations;
using L2Dn.Packages.DatDefinitions.Definitions.Shared;

namespace L2Dn.Packages.DatDefinitions.Definitions;

[ChronicleRange(Chronicles.Interlude, Chronicles.Helios - 1)]
public sealed class SystemMsgV2
{
    [ArrayLengthType(ArrayLengthType.Int32)]
    public SystemMsgRecord[] Records { get; set; } = Array.Empty<SystemMsgRecord>();

    public sealed class SystemMsgRecord
    {
        public uint Id { get; set; }
        public uint Unknown1 { get; set; }
        public string Message { get; set; } = string.Empty;
        public uint Group { get; set; }
        public RgbaColor Color { get; set; } = new RgbaColor();
        public string Sound { get; set; } = string.Empty;
        public string Voice { get; set; } = string.Empty;
        public uint Window { get; set; }
        public uint Font { get; set; }
        public uint LfTime { get; set; }
        public uint Background { get; set; }
        public uint Animation { get; set; }
        public string ScreenMessage { get; set; } = string.Empty;
        public string Type { get; set; } = string.Empty;
    }
}