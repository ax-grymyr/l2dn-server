using SixLabors.ImageSharp.PixelFormats;

namespace L2Dn.Packages.Textures;

internal static class DxtHelper
{
    internal static Rgba32[] DecodeDxt(ReadOnlySpan<byte> input, int width, int height, DxtVersion version)
    {
        Rgba32[] output = new Rgba32[width * height];
        Span<byte> targetRgba = stackalloc byte[16 * 4];
        ReadOnlySpan<byte> sourceBlock = input;
        int bytesPerBlock = version == DxtVersion.Dxt1 ? 8 : 16;
        for (int y = 0; y < height; y += 4)
        {
            for (int x = 0; x < width; x += 4)
            {
                DecompressDxtBlock(targetRgba, sourceBlock, version);

                Span<byte> sourcePixel = targetRgba;

                // exchange R & B
                for (int i = 0; i < 64; i += 4)
                    (sourcePixel[i], sourcePixel[i + 2]) = (sourcePixel[i + 2], sourcePixel[i]);

                for (int py = 0; py < 4; ++py)
                {
                    for (int px = 0; px < 4; ++px)
                    {
                        // get the target location
                        int sx = x + px;
                        int sy = y + py;
                        if (sx < width && sy < height)
                        {
                            // write the decompressed pixels to the correct image locations
                            ref Rgba32 targetPixel = ref output[width * sy + sx];

                            // copy the rgba value
                            targetPixel.R = sourcePixel[0];
                            targetPixel.G = sourcePixel[1];
                            targetPixel.B = sourcePixel[2];
                            targetPixel.A = sourcePixel[3];
                        }

                        sourcePixel = sourcePixel[4..];
                    }
                }

                sourceBlock = sourceBlock[bytesPerBlock..];
            }
        }

        return output;
    }

    private static void DecompressDxtBlock(Span<byte> target, ReadOnlySpan<byte> source, DxtVersion version)
    {
        // get the block locations
        ReadOnlySpan<byte> colourBlock = source;
        ReadOnlySpan<byte> alphaBock = source;
        if (version != DxtVersion.Dxt1)
            colourBlock = colourBlock[8..];

        // decompress colour
        DecompressColour(target, colourBlock, version == DxtVersion.Dxt1);

        // decompress alpha separately if necessary
        if (version == DxtVersion.Dxt3)
            DecompressAlphaDxt3(target, alphaBock);
        else if (version == DxtVersion.Dxt5)
            DecompressAlphaDxt5(target, alphaBock);
    }

    private static void DecompressAlphaDxt3(Span<byte> rgba, ReadOnlySpan<byte> block)
    {
        ReadOnlySpan<byte> bytes = block;

        // unpack the alpha values pairwise
        for (int i = 0; i < 8; ++i)
        {
            // quantise down to 4 bits
            byte quant = bytes[i];

            // unpack the values
            byte lo = (byte)(quant & 0x0f);
            byte hi = (byte)(quant & 0xf0);

            // convert back up to bytes
            rgba[8 * i + 3] = (byte)(lo | (lo << 4));
            rgba[8 * i + 7] = (byte)(hi | (hi >> 4));
        }
    }

    private static void DecompressAlphaDxt5(Span<byte> rgba, ReadOnlySpan<byte> block)
    {
        // get the two alpha values
        ReadOnlySpan<byte> bytes = block;
        int alpha0 = bytes[0];
        int alpha1 = bytes[1];

        // compare the values to build the codebook
        Span<byte> codes = stackalloc byte[16];
        codes[0] = (byte)alpha0;
        codes[1] = (byte)alpha1;
        if (alpha0 <= alpha1)
        {
            // use 5-alpha codebook
            for (int i = 1; i < 5; ++i)
                codes[1 + i] = (byte)(((5 - i) * alpha0 + i * alpha1) / 5);

            codes[6] = 0;
            codes[7] = 255;
        }
        else
        {
            // use 7-alpha codebook
            for (int i = 1; i < 7; ++i)
                codes[1 + i] = (byte)(((7 - i) * alpha0 + i * alpha1) / 7);
        }

        // decode the indices
        Span<byte> indices = stackalloc byte[16];
        ReadOnlySpan<byte> src = bytes[2..];
        Span<byte> dest = indices;
        for (int i = 0; i < 2; ++i)
        {
            // grab 3 bytes
            int value = 0;
            for (int j = 0; j < 3; ++j)
            {
                int b = src[0];
                src = src[1..];
                value |= b << (8 * j);
            }

            // unpack 8 3-bit values from it
            for (int j = 0; j < 8; ++j)
            {
                int index = (value >> (3 * j)) & 0x7;
                dest[0] = (byte)index;
                dest = dest[1..];
            }
        }

        // write out the indexed codebook values
        for (int i = 0; i < 16; ++i)
            rgba[4 * i + 3] = codes[indices[i]];
    }

    private static void DecompressColour(Span<byte> target, ReadOnlySpan<byte> source, bool isDxt1)
    {
        // get the block bytes
        ReadOnlySpan<byte> bytes = source;

        // unpack the endpoints
        Span<byte> codes = stackalloc byte[16];
        int a = Unpack565(bytes, codes);
        int b = Unpack565(bytes[2..], codes[4..]);

        // generate the midpoints
        for (int i = 0; i < 3; ++i)
        {
            int c = codes[i];
            int d = codes[4 + i];

            if (isDxt1 && a <= b)
            {
                codes[8 + i] = (byte)((c + d) / 2);
                codes[12 + i] = 0;
            }
            else
            {
                codes[8 + i] = (byte)((2 * c + d) / 3);
                codes[12 + i] = (byte)((c + 2 * d) / 3);
            }
        }

        // fill in alpha for the intermediate values
        codes[8 + 3] = 255;
        codes[12 + 3] = isDxt1 && a <= b ? (byte)0 : (byte)255;

        // unpack the indices
        Span<byte> indices = stackalloc byte[16];
        for (int i = 0; i < 4; ++i)
        {
            Span<byte> ind = indices[(4 * i)..];
            byte packed = bytes[4 + i];

            ind[0] = (byte)(packed & 0x3);
            ind[1] = (byte)((packed >> 2) & 0x3);
            ind[2] = (byte)((packed >> 4) & 0x3);
            ind[3] = (byte)((packed >> 6) & 0x3);
        }

        // store out the colours
        for (int i = 0; i < 16; ++i)
        {
            int offset = 4 * indices[i];
            for (int j = 0; j < 4; ++j)
                target[4 * i + j] = codes[offset + j];
        }
    }

    private static int Unpack565(ReadOnlySpan<byte> packed, Span<byte> colour)
    {
        // build the packed value
        int value = packed[0] | (packed[1] << 8);

        // get the components in the stored range
        byte red = (byte)((value >> 11) & 0x1f);
        byte green = (byte)((value >> 5) & 0x3f);
        byte blue = (byte)(value & 0x1f);

        // scale up to 8 bits
        colour[0] = (byte)((red << 3) | (red >> 2));
        colour[1] = (byte)((green << 2) | (green >> 4));
        colour[2] = (byte)((blue << 3) | (blue >> 2));
        colour[3] = 255;

        // return the value
        return value;
    }
}
