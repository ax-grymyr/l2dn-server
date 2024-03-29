using L2Dn.Packages.DatDefinitions.Annotations;
using L2Dn.Packages.DatDefinitions.Definitions.Shared;

namespace L2Dn.Packages.DatDefinitions.Definitions;

[ChronicleRange(Chronicles.Homunculus2, Chronicles.AgeOfMagic - 1)]
public sealed class SkillNameV5
{
    public SkillText[] Texts { get; set; } = Array.Empty<SkillText>();
    
    [ArrayLengthType(ArrayLengthType.Int32)]
    public SkillNameRecord[] Records { get; set; } = Array.Empty<SkillNameRecord>();
    
    public sealed class SkillNameRecord
    {
        public uint SkillId { get; set; }
        public byte SkillLevel { get; set; }
        public ushort SkillSubLevel { get; set; }
        public uint Name { get; set; }
        public uint Description { get; set; }
        public uint DescriptionParam { get; set; }
        public uint EnchantName { get; set; }
        public uint EnchantNameParam { get; set; }
        public uint EnchantDesc { get; set; }
        public uint EnchantDescParam { get; set; }
    } 
}