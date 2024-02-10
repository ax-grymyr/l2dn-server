namespace L2Dn.Packages;

public struct UScale: ISerializableObject
{
    public float X { get; set; }
    public float Y { get; set; }
    public float Z { get; set; }
    public int SheerRate { get; set; }
    public byte SheerAxis { get; set; }

    public void Read(UBinaryReader reader)
    {
        X = reader.ReadFloat();
        Y = reader.ReadFloat();
        Z = reader.ReadFloat();
        SheerRate = reader.ReadInt32();
        SheerAxis = reader.ReadByte();
    }
}
