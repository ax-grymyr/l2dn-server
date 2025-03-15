using L2Dn.Packages.DatDefinitions.Annotations;
using L2Dn.Packages.DatDefinitions.Definitions.Shared;

namespace L2Dn.Packages.DatDefinitions.Definitions;

[ChronicleRange(Chronicles.PreludeOfWar, Chronicles.MasterClass - 1)]
public sealed class SystemMsgV4
{
    [ArrayLengthType(ArrayLengthType.Int32)]
    public SystemMsgRecord[] Records { get; set; } = Array.Empty<SystemMsgRecord>();

    public sealed class SystemMsgRecord
    {
        public uint Id { get; set; }
        public uint Unknown1 { get; set; }
        public string Message { get; set; } = string.Empty;
        public uint Group { get; set; } // enum system_message_group_type
        public RgbaColor Color { get; set; } = new RgbaColor();
        public IndexedString Sound { get; set; }
        public IndexedString Voice { get; set; }
        public uint Window { get; set; }
        public uint Font { get; set; }
        public uint LfTime { get; set; }
        public uint Background { get; set; }
        public uint Animation { get; set; }
        public string ScreenMessage { get; set; } = string.Empty;
        public string ScreenParam { get; set; } = string.Empty;
        public string GfxScreenMessage { get; set; } = string.Empty;
        public string GfxScreenParam { get; set; } = string.Empty;
        public string Type { get; set; } = string.Empty;
    }
}