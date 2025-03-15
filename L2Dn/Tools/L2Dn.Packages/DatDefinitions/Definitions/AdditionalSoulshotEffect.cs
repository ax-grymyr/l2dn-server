using L2Dn.Packages.DatDefinitions.Annotations;
using L2Dn.Packages.DatDefinitions.Definitions.Shared;

namespace L2Dn.Packages.DatDefinitions.Definitions;

[ChronicleRange(Chronicles.Valiance, Chronicles.PreludeOfWar3 - 1)]
public sealed class AdditionalSoulshotEffect
{
    [ArrayLengthType(ArrayLengthType.Int32)]
    public AdditionalSoulshotEffectRecord[] Records { get; set; } = Array.Empty<AdditionalSoulshotEffectRecord>();

    public sealed class AdditionalSoulshotEffectRecord
    {
        public uint ItemClassId { get; set; }
        public string AttackEffect { get; set; } = string.Empty;
        public string AttackCriticalEffect { get; set; } = string.Empty;
    }
}