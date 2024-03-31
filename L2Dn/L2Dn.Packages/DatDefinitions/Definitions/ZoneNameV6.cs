using L2Dn.Packages.DatDefinitions.Annotations;

namespace L2Dn.Packages.DatDefinitions.Definitions;

[ChronicleRange(Chronicles.GrandCrusade, Chronicles.MasterClass - 1)]
public sealed class ZoneNameV6
{
    [ArrayLengthType(ArrayLengthType.Int32)]
    public ZoneNameRecord[] Records { get; set; } = Array.Empty<ZoneNameRecord>();
    
    public sealed class ZoneNameRecord
    {
        public ushort Id { get; set; }
        public ushort MapX { get; set; }
        public ushort MapY { get; set; }
        public float Top { get; set; }
        public float Bottom { get; set; }
        public string Name { get; set; } = string.Empty;
        public short TownBtnLocX { get; set; }
        public short TownBtnLocY { get; set; }
        public uint TownMapX { get; set; }
        public uint TownMapY { get; set; }
        public short TownMapWidth { get; set; }
        public short TownMapHeight { get; set; }
        public float TownMapScale { get; set; }
        public IndexedString TownMapTex { get; set; }
        public ushort Color { get; set; }
        public ushort Continent { get; set; }
        public ushort CurrentLayer { get; set; }
        public ushort TotalLayers { get; set; }
        public uint TownCenterX { get; set; }
        public uint TownCenterY { get; set; }
    } 
}