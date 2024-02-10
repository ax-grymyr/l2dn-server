namespace L2Dn.Packages;

public struct UVector: ISerializableObject
{
    public float X { get; set; }
    public float Y { get; set; }
    public float Z { get; set; }

    public void Read(UBinaryReader reader)
    {
        X = reader.ReadFloat();
        Y = reader.ReadFloat();
        Z = reader.ReadFloat();
    }
}
