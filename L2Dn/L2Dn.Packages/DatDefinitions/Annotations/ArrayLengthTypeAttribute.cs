namespace L2Dn.Packages.DatDefinitions.Annotations;

public sealed class ArrayLengthTypeAttribute(ArrayLengthType type): Attribute
{
    public ArrayLengthType Type { get; } = type;
}