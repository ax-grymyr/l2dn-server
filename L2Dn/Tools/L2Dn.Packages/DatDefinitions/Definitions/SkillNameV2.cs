using L2Dn.Packages.DatDefinitions.Annotations;

namespace L2Dn.Packages.DatDefinitions.Definitions;

[ChronicleRange(Chronicles.Underground, Chronicles.Helios - 1)]
public sealed class SkillNameV2
{
    [ArrayLengthType(ArrayLengthType.Int32)]
    public SkillNameRecord[] Records { get; set; } = Array.Empty<SkillNameRecord>();
    
    public sealed class SkillNameRecord
    {
        public uint SkillId { get; set; }
        public short SkillLevel { get; set; }
        public short SkillSubLevel { get; set; }
        public uint PrevSkillId { get; set; }
        public short PrevSkillLevel { get; set; }
        public short PrevSkillSubLevel { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string DescriptionParam { get; set; } = string.Empty;
        public string EnchantName { get; set; } = string.Empty;
        public string EnchantNameParam { get; set; } = string.Empty;
        public string EnchantDesc { get; set; } = string.Empty;
        public string EnchantDescParam { get; set; } = string.Empty;
    } 
}