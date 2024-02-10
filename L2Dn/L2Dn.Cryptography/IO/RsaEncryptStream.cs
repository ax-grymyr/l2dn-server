using System.Buffers;
using Org.BouncyCastle.Crypto.Engines;
using Org.BouncyCastle.Crypto.Parameters;

namespace L2Dn.IO;

internal sealed class RsaEncryptStream: ProcessingStream
{
    private readonly RsaEngine _engine;

    internal RsaEncryptStream(Stream stream, RsaKeyParameters parameters): base(stream)
    {
        _engine = new RsaEngine();
        _engine.Init(true, parameters);
    }

    protected override void ProcessInputData(Span<byte> output, ReadOnlySpan<byte> input) =>
        throw new NotSupportedException();

    protected override void ProcessOutputData(Span<byte> output, ReadOnlySpan<byte> input)
    {
        byte[] buf = ArrayPool<byte>.Shared.Rent(input.Length);
        try
        {
            input.CopyTo(buf);
            byte[]? result = _engine.ProcessBlock(buf, 0, input.Length);
            if (result is null)
                throw new InvalidOperationException("Invalid data");

            result.CopyTo(output);
        }
        finally
        {
            ArrayPool<byte>.Shared.Return(buf);
        }
    }
}
