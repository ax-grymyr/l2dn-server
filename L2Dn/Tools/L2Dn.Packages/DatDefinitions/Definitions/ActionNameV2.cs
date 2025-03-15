using L2Dn.Packages.DatDefinitions.Annotations;

namespace L2Dn.Packages.DatDefinitions.Definitions;

[ChronicleRange(Chronicles.Awakening, Chronicles.Helios - 1)]
public sealed class ActionNameV2
{
    [ArrayLengthType(ArrayLengthType.Int32)]
    public ActionNameRecord[] Records { get; set; } = Array.Empty<ActionNameRecord>();

    public sealed class ActionNameRecord
    {
        public uint Tag { get; set; }
        public uint Id { get; set; }
        public uint Type { get; set; }
        public uint Category { get; set; }
        public int[] Categories { get; set; } = Array.Empty<int>();
        public string Cmd { get; set; } = string.Empty;
        public string Icon { get; set; } = string.Empty;
        public string IconEx { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public uint ToggleGroupId { get; set; }
        
        [StringType(StringType.Utf16Le)]
        public string Description { get; set; } = string.Empty;
    }
}