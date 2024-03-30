using L2Dn.Packages.DatDefinitions.Annotations;

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
        public byte Category { get; set; } // enum action_category
        public int[] Categories { get; set; } = Array.Empty<int>();
        public string Name { get; set; } = string.Empty;

        [StringType(StringType.NameDataIndex)] 
        public string Icon { get; set; } = string.Empty;
        
        [StringType(StringType.NameDataIndex)] 
        public string IconEx { get; set; } = string.Empty;

        public string Description { get; set; } = string.Empty;
        public byte ToggleGroupId { get; set; }
        public byte AutomaticUse { get; set; }

        [StringType(StringType.NameDataIndex)] 
        public string Cmd { get; set; } = string.Empty;
    }
}