using L2Dn.Packages.DatDefinitions.Annotations;

namespace L2Dn.Packages.DatDefinitions;

public class L2NameData
{
    [ArrayLengthType(ArrayLengthType.Int32)]
    [StringType(StringType.Utf16Le)]
    public string[] Names { get; set; } = Array.Empty<string>();
}