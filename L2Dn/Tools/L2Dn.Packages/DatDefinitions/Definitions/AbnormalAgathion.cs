using L2Dn.Packages.DatDefinitions.Annotations;
using L2Dn.Packages.DatDefinitions.Definitions.Shared;

namespace L2Dn.Packages.DatDefinitions.Definitions;

[ChronicleRange(Chronicles.PreludeOfWar, Chronicles.Latest)]
public sealed class AbnormalAgathion
{
    [ArrayLengthType(ArrayLengthType.Int32)]
    public AbnormalAgathionRecord[] Records { get; set; } = Array.Empty<AbnormalAgathionRecord>();

    public sealed class AbnormalAgathionRecord
    {
        public uint Id { get; set; }
        public uint NpcId { get; set; }
        public Location OffsetPosition { get; set; } = new Location();
    }
}