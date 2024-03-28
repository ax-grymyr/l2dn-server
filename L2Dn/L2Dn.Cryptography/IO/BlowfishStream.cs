using L2Dn.Cryptography;

namespace L2Dn.IO;

public sealed class BlowfishStream(Stream stream, ReadOnlySpan<byte> blowfishKey): ProcessingStream(stream)
{
    private readonly BlowfishEngine _blowfishEngine = new(blowfishKey);

    protected override int ProcessInputData(Span<byte> output, ReadOnlySpan<byte> input) =>
        _blowfishEngine.Decode(output, input);

    protected override int ProcessOutputData(Span<byte> output, ReadOnlySpan<byte> input) =>
        _blowfishEngine.Encode(output, input);
}