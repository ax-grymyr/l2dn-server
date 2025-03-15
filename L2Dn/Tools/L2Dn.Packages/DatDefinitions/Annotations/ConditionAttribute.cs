namespace L2Dn.Packages.DatDefinitions.Annotations;

public sealed class ConditionAttribute(string propertyName, object? value): Attribute
{
    public string PropertyName { get; } = propertyName;
    public object? Value { get; } = value;
}