namespace L2Dn.Packages.Unreal;

public struct UColor: ISerializableObject
{
    public int Value { get; set; }

    public int R => Value & 0xFF;
    public int G => (Value >> 8) & 0xFF;
    public int B => (Value >> 16) & 0xFF;
    public int A => Value >> 24;

    public void Read(UBinaryReader reader)
    {
        Value = reader.ReadInt32();
    }
}
