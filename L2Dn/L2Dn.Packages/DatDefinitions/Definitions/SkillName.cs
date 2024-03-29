using L2Dn.Packages.DatDefinitions.Annotations;

namespace L2Dn.Packages.DatDefinitions.Definitions;

[ChronicleRange(Chronicles.ScionsOfDestiny, Chronicles.Underground - 1)]
public sealed class SkillName
{
    [ArrayLengthType(ArrayLengthType.Int32)]
    public SkillNameRecord[] Records { get; set; } = Array.Empty<SkillNameRecord>();
    
    public sealed class SkillNameRecord
    {
        public uint SkillId { get; set; }
        public uint SkillLevel { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string EnchantName { get; set; } = string.Empty;
        public string EnchantDesc { get; set; } = string.Empty;
    } 
}