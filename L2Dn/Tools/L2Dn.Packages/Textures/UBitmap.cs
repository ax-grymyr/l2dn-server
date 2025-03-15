using L2Dn.Packages.Unreal;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Advanced;
using SixLabors.ImageSharp.Memory;
using SixLabors.ImageSharp.PixelFormats;

namespace L2Dn.Packages.Textures;

public abstract class UBitmap: ISerializableObject
{
    public Image? Image { get; set; }
    public int Width { get; set; }
    public int Height { get; set; }
    public int UBits { get; set; }
    public int VBits { get; set; }

    public void Read(UBinaryReader reader)
    {
        if (reader.PackageVersion >= 63)
        {
            int positionAfterData = reader.ReadInt32();
        }

        int blockSize = reader.ReadIndex();
        byte[] data = new byte[blockSize];
        reader.ReadBytes(data);

        Width = reader.ReadInt32();
        Height = reader.ReadInt32();
        UBits = reader.ReadByte();
        VBits = reader.ReadByte();

        Decode(data);
    }

    protected abstract void Decode(ReadOnlySpan<byte> input);

    protected Image<T> ImageFromPixels<T>(ReadOnlySpan<T> pixels)
        where T: unmanaged, IPixel<T>
    {
        Image<T> image = new Image<T>(Width, Height);
        IMemoryGroup<T> memoryGroup = image.GetPixelMemoryGroup();
        ReadOnlySpan<T> source = pixels;
        foreach (Memory<T> memory in memoryGroup)
        {
            source[..memory.Length].CopyTo(memory.Span);
            source = source[memory.Length..];
        }

        return image;
    }
}
