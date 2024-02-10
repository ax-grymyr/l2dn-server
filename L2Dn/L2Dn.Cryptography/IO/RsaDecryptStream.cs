using System.Buffers;
using Org.BouncyCastle.Crypto.Engines;
using Org.BouncyCastle.Crypto.Parameters;

namespace L2Dn.IO;

internal sealed class RsaDecryptStream: ProcessingStream
{
    private readonly RsaEngine _engine;
    //private readonly RSA _rsa;

    internal RsaDecryptStream(Stream stream, RsaKeyParameters parameters): base(stream)
    {
        // RSAParameters p = new RSAParameters();
        // byte[] modulus = parameters.Modulus.ToByteArray();
        // if (modulus.Length == 129)
        // {
        //     if (modulus[0] != 0)
        //         throw new InvalidOperationException();
        //
        //     modulus = modulus[1..];
        // }
        //
        // p.Modulus = modulus;
        // p.Exponent = parameters.Exponent.ToByteArray();
        // _rsa = RSA.Create(p);

        _engine = new RsaEngine();
        _engine.Init(false, parameters);
    }

    protected override void ProcessInputData(Span<byte> output, ReadOnlySpan<byte> input)
    {
        //_rsa.TryDecrypt(input, output, RSAEncryptionPadding.Pkcs1, out int bytesWritten);
        //_rsa.Decrypt(input, output, RSAEncryptionPadding.Pkcs1);

        int inputBlockSize = _engine.GetInputBlockSize();
        //int inputBlockSize = _rsa.KeySize / 8;
        byte[] buf = ArrayPool<byte>.Shared.Rent(inputBlockSize);
         try
         {
            ReadOnlySpan<byte> inputTemp = input;
            Span<byte> outputTemp = output;

            while (inputTemp.Length != 0)
            {
                // _rsa.Decrypt(inputTemp[..inputBlockSize], outputTemp[..inputBlockSize], RSAEncryptionPadding.Pkcs1);
                // inputTemp = inputTemp[inputBlockSize..];
                // outputTemp = outputTemp[inputBlockSize..];

                inputTemp[..inputBlockSize].CopyTo(buf);
                inputTemp = inputTemp[inputBlockSize..];

                 byte[]? result = _engine.ProcessBlock(buf, 0, inputBlockSize);
                 if (result is null)
                     throw new InvalidOperationException("Invalid data");

                result.CopyTo(outputTemp);
                outputTemp = outputTemp[result.Length..];
            }
        }
        finally
        {
            ArrayPool<byte>.Shared.Return(buf);
        }
    }

    protected override void ProcessOutputData(Span<byte> output, ReadOnlySpan<byte> input) =>
        throw new NotSupportedException();
}
