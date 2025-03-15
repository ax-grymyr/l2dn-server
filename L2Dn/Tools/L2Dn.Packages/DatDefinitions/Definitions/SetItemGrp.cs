using L2Dn.Packages.DatDefinitions.Annotations;

namespace L2Dn.Packages.DatDefinitions.Definitions;

[ChronicleRange(Chronicles.Valiance, Chronicles.Latest)]
public sealed class SetItemGrp
{
    [ArrayLengthType(ArrayLengthType.Int32)]
    public SetItemGrpRecord[] Records { get; set; } = [];

    public sealed class SetItemGrpRecord
    {
        public uint Number { get; set; }
        
        [ArrayLengthType(ArrayLengthType.Int32)]
        public SetItem[] EssentialSetItems { get; set; } = [];
        
        [ArrayLengthType(ArrayLengthType.Int32)]
        public string[] EssentialSetItemDescriptions { get; set; } = [];
        
        [ArrayLengthType(ArrayLengthType.Int32)]
        public SetItem[] AdditionalSetItems { get; set; } = [];
        
        [ArrayLengthType(ArrayLengthType.Int32)]
        public string[] AdditionalSetItemDescriptions { get; set; } = [];

        public uint Unknown1 { get; set; }
        public uint Unknown2 { get; set; }

        [ArrayLengthType(ArrayLengthType.Int32)]
        public EnchantSetItemCondition[] EnchantSetItemConditions { get; set; } = [];
    }

    public class EnchantSetItemCondition
    {
	    public uint EnchantLevel { get; set; }
	    public string Description { get; set; } = string.Empty;
    }

    public class SetItem
    {
	    [ArrayLengthType(ArrayLengthType.Int32)]
	    public uint[] ItemIds { get; set; } = [];
    }
}