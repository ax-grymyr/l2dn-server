using L2Dn.Packages.DatDefinitions.Annotations;
using L2Dn.Packages.DatDefinitions.Definitions.Enums;

namespace L2Dn.Packages.DatDefinitions.Definitions;

[ChronicleRange(Chronicles.PreludeOfWar3, Chronicles.Latest)]
public sealed class ActionNameV4
{
    [ArrayLengthType(ArrayLengthType.Int32)]
    public ActionNameRecord[] Records { get; set; } = Array.Empty<ActionNameRecord>();

    public sealed class ActionNameRecord
    {
        public uint Id { get; set; }
        public int Type { get; set; }
        public ActionCategory Category { get; set; }
        public int[] Categories { get; set; } = Array.Empty<int>();
        public string Name { get; set; } = string.Empty;
        public IndexedString Icon { get; set; }
        public IndexedString IconEx { get; set; }
        public string Description { get; set; } = string.Empty;
        public byte ToggleGroupId { get; set; }
        public byte AutomaticUse { get; set; }
        public IndexedString Cmd { get; set; }
    }
}