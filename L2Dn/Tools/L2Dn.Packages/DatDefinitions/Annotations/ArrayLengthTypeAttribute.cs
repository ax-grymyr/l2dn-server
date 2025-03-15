namespace L2Dn.Packages.DatDefinitions.Annotations;

[AttributeUsage(AttributeTargets.Property)]
public sealed class ArrayLengthTypeAttribute(ArrayLengthType type, int size = -1): Attribute
{
    public ArrayLengthType Type { get; } = type;
    public int Size { get; } = size;
    public string? ArrayPropertyName { get; set; }
}