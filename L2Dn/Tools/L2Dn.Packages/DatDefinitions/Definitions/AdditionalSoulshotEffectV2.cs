using L2Dn.Packages.DatDefinitions.Annotations;

namespace L2Dn.Packages.DatDefinitions.Definitions;

[ChronicleRange(Chronicles.PreludeOfWar3, Chronicles.Latest)]
public sealed class AdditionalSoulshotEffectV2
{
    [ArrayLengthType(ArrayLengthType.Int32)]
    public AdditionalSoulshotEffectRecord[] Records { get; set; } = Array.Empty<AdditionalSoulshotEffectRecord>();

    public sealed class AdditionalSoulshotEffectRecord
    {
        public uint ItemClassId { get; set; }
        public uint Level { get; set; }
        public string AttackEffect { get; set; } = string.Empty;
        public string AttackCriticalEffect { get; set; } = string.Empty;
    }
}