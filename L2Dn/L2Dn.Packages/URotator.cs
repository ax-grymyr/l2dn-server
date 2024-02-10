namespace L2Dn.Packages;

public struct URotator: ISerializableObject
{
    public float Pitch { get; set; }
    public float Yaw { get; set; }
    public float Roll { get; set; }

    public void Read(UBinaryReader reader)
    {
        Pitch = reader.ReadFloat();
        Yaw = reader.ReadFloat();
        Roll = reader.ReadFloat();
    }
}
