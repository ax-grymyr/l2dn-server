using L2Dn.Packages.DatDefinitions.Annotations;

namespace L2Dn.Packages.DatDefinitions.Definitions;

[ChronicleRange(Chronicles.Homunculus2, Chronicles.MasterClass3 - 1)]
public sealed class BlessOption
{
    [ArrayLengthType(ArrayLengthType.Int32)]
    public BlessOptionRecord[] Records { get; set; } = Array.Empty<BlessOptionRecord>();

    public sealed class BlessOptionRecord
    {
        public uint GroupId { get; set; }
        public byte WeaponType { get; set; } // enum grade_type
        public byte Grade { get; set; } // enum item_weapon_v2_type
        public ushort NotInUse { get; set; }
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