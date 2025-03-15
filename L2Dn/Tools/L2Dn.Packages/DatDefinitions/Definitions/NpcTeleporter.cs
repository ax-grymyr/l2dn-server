using L2Dn.Packages.DatDefinitions.Annotations;
using L2Dn.Packages.DatDefinitions.Definitions.Shared;

namespace L2Dn.Packages.DatDefinitions.Definitions;

[ChronicleRange(Chronicles.Ertheia, Chronicles.Latest)]
public sealed class NpcTeleporter
{
    [ArrayLengthType(ArrayLengthType.Int32)]
    public NpcTeleporterRecord[] Records { get; set; } = Array.Empty<NpcTeleporterRecord>();

    public sealed class NpcTeleporterRecord
    {
        public uint NpcId { get; set; }
        public Location Location { get; set; } = new Location();
        public uint TeleportZoneId { get; set; }
        public uint[] Classes { get; set; } = Array.Empty<uint>();
    }
}