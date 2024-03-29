namespace L2Dn.Packages.DatDefinitions.Annotations;

[AttributeUsage(AttributeTargets.Class)]
public class ChronicleRange(Chronicles from, Chronicles upTo): Attribute
{
    public Chronicles From { get; } = from;
    public Chronicles UpTo { get; } = upTo;
}