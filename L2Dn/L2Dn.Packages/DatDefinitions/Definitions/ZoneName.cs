using L2Dn.Packages.DatDefinitions.Annotations;

namespace L2Dn.Packages.DatDefinitions.Definitions;

[ChronicleRange(Chronicles.ScionsOfDestiny, Chronicles.Interlude - 1)]
public sealed class ZoneName
{
    [ArrayLengthType(ArrayLengthType.Int32)]
    public ZoneNameRecord[] Records { get; set; } = Array.Empty<ZoneNameRecord>();
    
    public sealed class ZoneNameRecord
    {
        public uint Id { get; set; }
        public uint Color { get; set; }
        public uint MapX { get; set; }
        public uint MapY { get; set; }
        public float Top { get; set; }
        public float Bottom { get; set; }
        public string Name { get; set; } = string.Empty;
    } 
}