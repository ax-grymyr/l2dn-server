using SixLabors.ImageSharp.PixelFormats;

namespace L2Dn.Packages.Textures;

public class UBitmapDxt5: UBitmap
{
    protected override void Decode(ReadOnlySpan<byte> input)
    {
        Rgba32[] pixels = DxtHelper.DecodeDxt(input, Width, Height, DxtVersion.Dxt5);
        Image = ImageFromPixels<Rgba32>(pixels);
    }
}
