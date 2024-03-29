using L2Dn.Packages.DatDefinitions.Annotations;

namespace L2Dn.Packages.DatDefinitions.Definitions;

[ChronicleRange(Chronicles.Interlude, Chronicles.Epilogue - 1)]
public sealed class ZoneNameV2
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
        public string Map { get; set; } = string.Empty;
    } 
}