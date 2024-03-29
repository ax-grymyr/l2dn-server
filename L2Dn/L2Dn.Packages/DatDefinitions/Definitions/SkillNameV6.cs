using L2Dn.Packages.DatDefinitions.Annotations;
using L2Dn.Packages.DatDefinitions.Definitions.Shared;

namespace L2Dn.Packages.DatDefinitions.Definitions;

[ChronicleRange(Chronicles.AgeOfMagic, Chronicles.Latest)]
public sealed class SkillNameV6
{
    public SkillText[] Texts { get; set; } = Array.Empty<SkillText>();
    
    [ArrayLengthType(ArrayLengthType.Int32)]
    public SkillNameRecord[] Records { get; set; } = Array.Empty<SkillNameRecord>();
    
    public sealed class SkillNameRecord
    {
        public uint SkillId { get; set; }
        public short SkillLevel { get; set; }
        public short SkillSubLevel { get; set; }
        public int Name { get; set; }
        public int Description { get; set; }
        public int DescriptionParam { get; set; }
        public int EnchantName { get; set; }
        public int EnchantNameParam { get; set; }
        public int EnchantDesc { get; set; }
        public int EnchantDescParam { get; set; }
    } 
}