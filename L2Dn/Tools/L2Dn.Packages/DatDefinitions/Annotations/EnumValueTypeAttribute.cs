namespace L2Dn.Packages.DatDefinitions.Annotations;

[AttributeUsage(AttributeTargets.Property)]
public class EnumValueTypeAttribute(EnumValueType valueType): Attribute
{
    public EnumValueType Type { get; } = valueType;
}