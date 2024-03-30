using L2Dn.Packages.DatDefinitions.Annotations;
using L2Dn.Packages.DatDefinitions.Definitions.Shared;

namespace L2Dn.Packages.DatDefinitions.Definitions;

[ChronicleRange(Chronicles.Valiance, Chronicles.Latest)]
public sealed class AdditionalJewelEquipEffect
{
    [ArrayLengthType(ArrayLengthType.Int32)]
    public AdditionalJewelEquipEffectRecord[] Records { get; set; } = Array.Empty<AdditionalJewelEquipEffectRecord>();

    public sealed class AdditionalJewelEquipEffectRecord
    {
        public uint SkillId { get; set; }
        public uint SkillLevel { get; set; }
        public string OnStick { get; set; } = string.Empty;
        public string OnBook { get; set; } = string.Empty;
    }
}