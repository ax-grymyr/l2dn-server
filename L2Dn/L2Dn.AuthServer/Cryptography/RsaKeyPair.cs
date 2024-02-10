using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Generators;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Math;
using Org.BouncyCastle.Security;

namespace L2Dn.AuthServer.Cryptography
{
    public sealed class RsaKeyPair
    {
        private readonly AsymmetricCipherKeyPair _parameters;
        private readonly byte[] _scrambledModulus;

        public RsaKeyPair()
            : this(GenerateKeyPair())
        {
        }

        public RsaKeyPair(AsymmetricCipherKeyPair parameters)
        {
            _parameters = parameters ?? throw new ArgumentNullException(nameof(parameters));
            RsaKeyParameters publicKey = (RsaKeyParameters)parameters.Public;
            _scrambledModulus = ScrambleModulus(publicKey.Modulus);
        }

        public byte[] ScrambledModulus => _scrambledModulus;

        public AsymmetricKeyParameter PrivateKey => _parameters.Private;

        private static AsymmetricCipherKeyPair GenerateKeyPair()
        {
             // RsaKeyGenerationParameters generationParameters = new RsaKeyGenerationParameters(
             //     BigInteger.ValueOf(65537), new SecureRandom(), 1024, 10);

            //RsaKeyGenerationParameters generationParameters = new RsaKeyGenerationParameters();

            RsaKeyPairGenerator keyPairGenerator = new RsaKeyPairGenerator();
            keyPairGenerator.Init(new KeyGenerationParameters(new SecureRandom(), 1024));
            //keyPairGenerator.Init(generationParameters);
            return keyPairGenerator.GenerateKeyPair();
        }

        private static byte[] ScrambleModulus(BigInteger modulus)
        {
            byte[] result = modulus.ToByteArray();

            if (result.Length == 129 && result[0] == 0)
            {
                byte[] temp = new byte[128];
                result.AsSpan(1).CopyTo(temp);
                result = temp;
            }

            // step 1 : swap 0x4d-0x50 <-> 0x00-0x04
            for (int i = 0; i < 4; i++)
                (result[i], result[0x4d + i]) = (result[0x4d + i], result[i]);

            // step 2 : xor first 0x40 bytes with last 0x40 bytes
            for (int i = 0; i < 0x40; i++)
                result[i] = (byte)(result[i] ^ result[0x40 + i]);

            // step 3 : xor bytes 0x0d-0x10 with bytes 0x34-0x38
            for (int i = 0; i < 4; i++)
                result[0x0d + i] = (byte)(result[0x0d + i] ^ result[0x34 + i]);

            // step 4 : xor last 0x40 bytes with first 0x40 bytes
            for (int i = 0; i < 0x40; i++)
                result[0x40 + i] = (byte)(result[0x40 + i] ^ result[i]);

            return result;
        }
    }
}