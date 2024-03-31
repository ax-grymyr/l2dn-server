using L2Dn.Packages.DatDefinitions.Annotations;

namespace L2Dn.Packages.DatDefinitions.Definitions;

[ChronicleRange(Chronicles.ReturnOfTheQueenAnt, Chronicles.Latest)]
public sealed class CollectionMain
{
    [ArrayLengthType(ArrayLengthType.Int32)]
    public CollectionMainRecord[] Records { get; set; } = Array.Empty<CollectionMainRecord>();

    public sealed class CollectionMainRecord
    {
        public uint MainId { get; set; }
        public uint BackgroundLevel { get; set; }
        public uint Category { get; set; }
        public uint CollectionId { get; set; }
        public uint KeyItemId { get; set; }
        public uint KeyEffect { get; set; }
    }
}