using L2Dn.Packages.DatDefinitions.Annotations;

namespace L2Dn.Packages.DatDefinitions.Definitions;

[ChronicleRange(Chronicles.Helios, Chronicles.GrandCrusade - 1)]
public sealed class ZoneNameV5
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
        public uint TownBtnLocX { get; set; }
        public uint TownBtnLocY { get; set; }
        public uint TownMapX { get; set; }
        public uint TownMapY { get; set; }
        public uint TownMapWidth { get; set; }
        public uint TownMapHeight { get; set; }
        public float TownMapScale { get; set; }
        public IndexedString TownMapTex { get; set; }
        public uint Continent { get; set; }
        public uint CurrentLayer { get; set; }
        public uint TotalLayers { get; set; }
        public uint TownCenterX { get; set; }
        public uint TownCenterY { get; set; }
    } 
}