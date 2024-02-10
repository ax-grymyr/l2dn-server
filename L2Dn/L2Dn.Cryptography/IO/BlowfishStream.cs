using L2Dn.Cryptography;

namespace L2Dn.IO;

public sealed class BlowfishStream(Stream stream, byte[] blowfishKey): ProcessingStream(stream)
{
    private readonly OldBlowfishEngine _blowfishEngine = new(blowfishKey);

    protected override void ProcessInputData(Span<byte> output, ReadOnlySpan<byte> input) =>
        _blowfishEngine.Decode(output, input);

    protected override void ProcessOutputData(Span<byte> output, ReadOnlySpan<byte> input) =>
        _blowfishEngine.Encode(output, input);
}
