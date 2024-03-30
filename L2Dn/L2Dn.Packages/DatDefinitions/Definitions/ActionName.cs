using L2Dn.Packages.DatDefinitions.Annotations;
using L2Dn.Packages.DatDefinitions.Definitions.Shared;

namespace L2Dn.Packages.DatDefinitions.Definitions;

[ChronicleRange(Chronicles.ScionsOfDestiny, Chronicles.Awakening - 1)]
public sealed class ActionName
{
    [ArrayLengthType(ArrayLengthType.Int32)]
    public ActionNameRecord[] Records { get; set; } = Array.Empty<ActionNameRecord>();

    public sealed class ActionNameRecord
    {
        public uint Tag { get; set; }
        public uint Id { get; set; }
        public int Type { get; set; }
        public uint Category { get; set; }
        public int[] Categories { get; set; } = Array.Empty<int>();
        public string Cmd { get; set; } = string.Empty;
        public string Icon { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        
        [StringType(StringType.Utf16)]
        public string Description { get; set; } = string.Empty;
    }
}