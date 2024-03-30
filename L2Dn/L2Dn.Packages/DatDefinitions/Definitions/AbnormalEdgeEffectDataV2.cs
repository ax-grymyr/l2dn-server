using L2Dn.Packages.DatDefinitions.Annotations;
using L2Dn.Packages.DatDefinitions.Definitions.Shared;

namespace L2Dn.Packages.DatDefinitions.Definitions;

[ChronicleRange(Chronicles.MasterClass2, Chronicles.Latest)]
public sealed class AbnormalEdgeEffectDataV2
{
    [ArrayLengthType(ArrayLengthType.Int32)]
    public AbnormalAgathionRecord[] Records { get; set; } = Array.Empty<AbnormalAgathionRecord>();

    public sealed class AbnormalAgathionRecord
    {
        public uint Id { get; set; }
        public uint AlphaFactor { get; set; }
        public uint GrayFactor { get; set; }
        public uint Unknown1 { get; set; }
        public uint Unknown2 { get; set; }
        public uint Unknown3 { get; set; }
        public float ExtrudeScale { get; set; }
        public float EdgePeak { get; set; }
        public float EdgeSharp { get; set; }
        public float NoiseScale { get; set; }
        public float NoisePanSpeed { get; set; }
        public float NoiseRate { get; set; }
        public RgbaColor MaxColor { get; set; } = new RgbaColor();
        public RgbaColor MinColor { get; set; } = new RgbaColor();
        public uint Unknown4 { get; set; }
        public int BodyPart { get; set; }
    }
}