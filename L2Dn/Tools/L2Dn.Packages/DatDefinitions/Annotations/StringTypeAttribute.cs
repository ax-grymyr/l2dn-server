namespace L2Dn.Packages.DatDefinitions.Annotations;

[AttributeUsage(AttributeTargets.Property)]
public class StringTypeAttribute(StringType stringType): Attribute
{
    public StringType Type { get; } = stringType;
}