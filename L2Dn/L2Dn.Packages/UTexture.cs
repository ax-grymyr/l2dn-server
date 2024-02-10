using L2Dn.Packages.Textures;

namespace L2Dn.Packages;

public class UTexture: UMaterial, ISerializableObject
{
    public int Width { get; set; }
    public int Height { get; set; }
    public UTextureFormat Format { get; set; } = UTextureFormat.Rgba8;
    public int UClamp { get; set; }
    public int VClamp { get; set; }
    public string TextureName { get; set; } = string.Empty;
    public List<UBitmap> Bitmaps { get; set; } = new();

    static UTexture()
    {
        RegisterProperty<UTexture>("Format", (obj, value) => obj.Format = (UTextureFormat)(byte)value);
        RegisterProperty<UTexture>("USize", (obj, value) => obj.Width = (int)value);
        RegisterProperty<UTexture>("VSize", (obj, value) => obj.Height = (int)value);
        RegisterProperty<UTexture>("UClamp", (obj, value) => obj.UClamp = (int)value);
        RegisterProperty<UTexture>("VClamp", (obj, value) => obj.VClamp = (int)value);
        //RegisterProperty<UTexture>("Palette", (obj, value) => obj.Palette = (int)value);
        // bAlphaTexture
        // bMasked
    }

    public new void Read(UBinaryReader reader)
    {
        base.Read(reader);

        reader.ReadInt32(); // Unknown int in Lineage 2

        switch (Format)
        {
            case UTextureFormat.G16:
                Bitmaps = ReadBitmaps<UBitmapG16>(reader);
                break;

            case UTextureFormat.Dxt1:
                Bitmaps = ReadBitmaps<UBitmapDxt1>(reader);
                break;

            case UTextureFormat.Dxt3:
                Bitmaps = ReadBitmaps<UBitmapDxt3>(reader);
                break;

            case UTextureFormat.Dxt5:
                Bitmaps = ReadBitmaps<UBitmapDxt5>(reader);
                break;

            case UTextureFormat.Rgba8:
                Bitmaps = ReadBitmaps<UBitmapRgba8>(reader);
                break;

            default:
                throw new NotSupportedException($"Texture format '{Format}' is not supported");
        }
    }

    private static List<UBitmap> ReadBitmaps<T>(UBinaryReader reader)
        where T: UBitmap, new()
    {
        int count = reader.ReadIndex();
        return reader.ReadObjects<T>(count).Cast<UBitmap>().ToList();
    }
}
