using L2Dn.Packages.DatDefinitions.Annotations;
using L2Dn.Packages.DatDefinitions.Definitions.Shared;

namespace L2Dn.Packages.DatDefinitions.Definitions;

[ChronicleRange(Chronicles.Helios, Chronicles.EtinasFate - 1)]
public sealed class SkillNameV3
{
    public SkillText[] Texts { get; set; } = Array.Empty<SkillText>();
    
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
        public uint Name { get; set; }
        public uint Description { get; set; }
        public uint DescriptionParam { get; set; }
        public uint EnchantName { get; set; }
        public uint EnchantNameParam { get; set; }
        public uint EnchantDesc { get; set; }
        public uint EnchantDescParam { get; set; }
    } 
}