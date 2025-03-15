using L2Dn.Packages.DatDefinitions.Annotations;
using L2Dn.Packages.DatDefinitions.Definitions.Enums;

namespace L2Dn.Packages.DatDefinitions.Definitions;

[ChronicleRange(Chronicles.MasterClass3, Chronicles.Latest)]
public sealed class BlessOptionV2
{
    [ArrayLengthType(ArrayLengthType.Int32)]
    public BlessOptionRecord[] Records { get; set; } = Array.Empty<BlessOptionRecord>();

    public sealed class BlessOptionRecord
    {
        public BlessItemType ItemType { get; set; }
        public GradeType Grade { get; set; }
        public byte Unknown1 { get; set; }
        public BodyPartV2Type BodyPart { get; set; }
        public ItemWeaponV2Type WeaponType { get; set; }
        public BlessOptionEffect[] Effects { get; set; } = Array.Empty<BlessOptionEffect>();
    }

    public sealed class BlessOptionEffect
    {
        public BlessOptionType OptionType { get; set; }
        public uint EnchantValue { get; set; }
        public uint OptionDescSkillId { get; set; }
        public uint OptionDescSkillLevel { get; set; }
    } 
}