using L2Dn.Packages.DatDefinitions.Annotations;
using L2Dn.Packages.DatDefinitions.Definitions.Shared;

namespace L2Dn.Packages.DatDefinitions.Definitions;

[ChronicleRange(Chronicles.MasterClass2, Chronicles.Latest)]
public sealed class AbnormalDefaultEffectV5
{
    [ArrayLengthType(ArrayLengthType.Int32)]
    public AbnormalDefaultEffectRecord[] Records { get; set; } = Array.Empty<AbnormalDefaultEffectRecord>();

    public sealed class AbnormalDefaultEffectRecord
    {
        public uint Id { get; set; }
        public string EffectName { get; set; } = string.Empty;
        public uint IsScreenEffect { get; set; }
        
        [StringType(StringType.NameDataIndex)]
        public string AttachBoneName { get; set; } = string.Empty;
        
        public uint AttachType { get; set; }
        public Location OffsetPosition { get; set; } = new Location();
        public Location OffsetRotation { get; set; } = new Location();
        public uint IsScalingByCylinder { get; set; }
        public float EffectScale { get; set; }
        public uint PostEffectId { get; set; }
        public int BodyPart { get; set; }
    }
}