﻿using L2Dn.Packages.DatDefinitions.Annotations;

namespace L2Dn.Packages.DatDefinitions.Definitions;

[ChronicleRange(Chronicles.MasterClass2, Chronicles.Latest)]
public sealed class ServerNameV5
{
    [ArrayLengthType(ArrayLengthType.Int32)]
    public ServerNameRecord[] Records { get; set; } = Array.Empty<ServerNameRecord>();

    public sealed class ServerNameRecord
    {
        public uint Id { get; set; }
        public uint ExtId { get; set; }
        public uint Priority { get; set; }
        public string Name { get; set; } = string.Empty;
        public string MarkName { get; set; } = string.Empty;
        public uint Brackets { get; set; }
    }
}