using System.Runtime.InteropServices;
using SixLabors.ImageSharp.PixelFormats;

namespace L2Dn.Packages.Textures;

public class UBitmapG16: UBitmap
{
    protected override void Decode(ReadOnlySpan<byte> input)
    {
        ReadOnlySpan<L16> source = MemoryMarshal.Cast<byte, L16>(input);
        Image = ImageFromPixels(source);
    }
}
