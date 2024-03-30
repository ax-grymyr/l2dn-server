using L2Dn.Packages.DatDefinitions.Annotations;
using L2Dn.Packages.DatDefinitions.Definitions.Shared;

namespace L2Dn.Packages.DatDefinitions.Definitions;

[ChronicleRange(Chronicles.Helios, Chronicles.PreludeOfWar3 - 1)]
public sealed class AbnormalDefaultEffectV3
{
    [ArrayLengthType(ArrayLengthType.Int32)]
    public AbnormalDefaultEffectRecord[] Records { get; set; } = Array.Empty<AbnormalDefaultEffectRecord>();

    public sealed class AbnormalDefaultEffectRecord
    {
        public uint Id { get; set; }
        public string EffectName { get; set; } = string.Empty;
        public uint IsScreenEffect { get; set; }
        public uint IsAttachToGround { get; set; }
        
        [StringType(StringType.NameDataIndex)]
        public string AttachBoneName { get; set; } = string.Empty;
        
        public uint AttachType { get; set; }
        public Location OffsetPosition { get; set; } = new Location();
        public uint IsScalingByCylinder { get; set; }
        public uint IsLocateOnFoot { get; set; }
        public float EffectScale { get; set; }
        public uint PostEffectId { get; set; }
    }
}