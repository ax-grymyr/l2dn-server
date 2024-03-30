using L2Dn.Packages.DatDefinitions.Annotations;

namespace L2Dn.Packages.DatDefinitions.Definitions;

[ChronicleRange(Chronicles.MasterClass3, Chronicles.Latest)]
public sealed class BlessOptionV2
{
    [ArrayLengthType(ArrayLengthType.Int32)]
    public BlessOptionRecord[] Records { get; set; } = Array.Empty<BlessOptionRecord>();

    public sealed class BlessOptionRecord
    {
        public uint ItemType { get; set; } // enum bless_item_type
        public byte Grade { get; set; } // enum grade_type
        public byte Unknown1 { get; set; }
        public byte BodyPart { get; set; } // enum bodypart_v2_type
        public byte WeaponType { get; set; } // enum item_weapon_v2_type
        public BlessOptionEffect[] Effects { get; set; } = Array.Empty<BlessOptionEffect>();
    }

    public sealed class BlessOptionEffect
    {
        public uint OptionType { get; set; } // enum blessoption_type
        public uint EnchantValue { get; set; }
        public uint OptionDescSkillId { get; set; }
        public uint OptionDescSkillLevel { get; set; }
    } 
}