using System.Runtime.CompilerServices;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Math;

namespace L2Dn.IO;

public static class EncryptionKeys
{
    internal const int XorKey111 = 0xAC;

    internal static byte GetXorKey121(string fileName)
    {
        ReadOnlySpan<char> input = fileName;
        Span<char> lowerCase = stackalloc char[input.Length + 8];
        int length = input.ToLowerInvariant(lowerCase);
        ref char begin = ref lowerCase[0];
        ref char end = ref Unsafe.Add(ref begin, length);
        int sum = 0;
        while (!Unsafe.AreSame(ref begin, ref end))
        {
            sum += begin;
            begin = ref Unsafe.Add(ref begin, 1);
        }

        return (byte)sum;
    }

    internal static readonly byte[] BlowfishKey211 = "31==-%&@!^+][;'.]94-\0"u8.ToArray();
    internal static readonly byte[] BlowfishKey212 = "[;'.]94-&@%!^+]-31==\0"u8.ToArray();

    public static readonly BigInteger RsaModulus411 = new(
        "8c9d5da87b30f5d7cd9dc88c746eaac5" +
        "bb180267fa11737358c4c95d9adf59dd" +
        "37689f9befb251508759555d6fe0eca8" +
        "7bebe0a10712cf0ec245af84cd22eb4c" +
        "b675e98eaf5799fca62a20a2baa4801d" +
        "5d70718dcd43283b8428f1387aec6600" +
        "f937bfc7bb72404d187d3a9c438f1ffc" +
        "e9ce365dccf754232ff6def038a41385", 16);

    public static readonly BigInteger RsaPrivateExponent411 = new("1d", 16);

    public static readonly BigInteger RsaModulus412 = new(
        "a465134799cf2c45087093e7d0f0f144" +
        "e6d528110c08f674730d436e40827330" +
        "eccea46e70acf10cdda7d8f710e3b44d" +
        "cca931812d76cd7494289bca8b73823f" +
        "57efc0515b97e4a2a02612ccfa719cf7" +
        "885104b06f2e7e2cc967b62e3d3b1aad" +
        "b925db94cbc8cd3070a4bb13f7e202c7" +
        "733a67b1b94c1ebc0afcbe1a63b448cf", 16);

    public static readonly BigInteger RsaPrivateExponent412 = new("25", 16);

    public static readonly BigInteger RsaModulus413 = new(
        "97df398472ddf737ef0a0cd17e8d172f" +
        "0fef1661a38a8ae1d6e829bc1c6e4c3c" +
        "fc19292dda9ef90175e46e7394a18850" +
        "b6417d03be6eea274d3ed1dde5b5d7bd" +
        "e72cc0a0b71d03608655633881793a02" +
        "c9a67d9ef2b45eb7c08d4be329083ce4" +
        "50e68f7867b6749314d40511d09bc574" +
        "4551baa86a89dc38123dc1668fd72d83", 16);

    public static readonly BigInteger RsaPrivateExponent413 = new("35", 16);

    public static readonly BigInteger RsaModulus414 = new(
        "ad70257b2316ce09dfaf2ebc3f63b3d6" +
        "73b0c98a403950e26bb87379b11e17ae" +
        "d0e45af23e7171e5ec1fbc8d1ae32ffb" +
        "7801b31266eef9c334b53469d4b7cbe8" +
        "3284273d35a9aab49b453e7012f37449" +
        "6c65f8089f5d134b0eb3d1e3b22051ed" +
        "5977a6dd68c4f85785dfcc9f4412c816" +
        "81944fc4b8ce27caf0242deaa5762e8d", 16);

    public static readonly BigInteger RsaPrivateExponent414 = new("25", 16);

    public static readonly BigInteger RsaModulusL2EncDec = new(
        "75b4d6de5c016544068a1acf125869f4" +
        "3d2e09fc55b8b1e289556daf9b875763" +
        "5593446288b3653da1ce91c87bb1a5c1" +
        "8f16323495c55d7d72c0890a83f69bfd" +
        "1fd9434eb1c02f3e4679edfa43309319" +
        "070129c267c85604d87bb65bae205de3" +
        "707af1d2108881abb567c3b3d069ae67" +
        "c3a4c6a3aa93d26413d4c66094ae2039", 16);

    public static readonly BigInteger RsaPublicExponentL2EncDec = new(
        "30b4c2d798d47086145c75063c8e841e" +
        "719776e400291d7838d3e6c4405b504c" +
        "6a07f8fca27f32b86643d2649d1d5f12" +
        "4cdd0bf272f0909dd7352fe10a77b34d" +
        "831043d9ae541f8263c6fe3d1c14c2f0" +
        "4e43a7253a6dda9a8c1562cbd493c1b6" +
        "31a1957618ad5dfe5ca28553f746e2fc" +
        "6f2db816c7db223ec91e955081c1de65", 16);

    public static readonly BigInteger RsaPrivateExponentL2EncDec = new("1d", 16);

    public static RsaKeyParameters RsaDecryption411 { get; set; } = new(true, RsaModulus411, RsaPrivateExponent411);
    public static RsaKeyParameters RsaDecryption412 { get; set; } = new(true, RsaModulus412, RsaPrivateExponent411);
    public static RsaKeyParameters RsaDecryption413 { get; set; } = new(true, RsaModulus413, RsaPrivateExponent411);
    public static RsaKeyParameters RsaDecryption414 { get; set; } = new(true, RsaModulus414, RsaPrivateExponent411);

    public static RsaKeyParameters RsaEncryption411 { get; set; } =
        new(false, RsaModulusL2EncDec, RsaPublicExponentL2EncDec);

    public static RsaKeyParameters RsaEncryption412 { get; set; } =
        new(false, RsaModulusL2EncDec, RsaPublicExponentL2EncDec);

    public static RsaKeyParameters RsaEncryption413 { get; set; } =
        new(false, RsaModulusL2EncDec, RsaPublicExponentL2EncDec);

    public static RsaKeyParameters RsaEncryption414 { get; set; } =
        new(false, RsaModulusL2EncDec, RsaPublicExponentL2EncDec);
}
