using System.Collections.Immutable;
using L2Dn.AuthServer.Cryptography;
using L2Dn.Cryptography;
using L2Dn.Network;

namespace L2Dn.AuthServer.Network.Client;

internal sealed class AuthSessionFactory: ISessionFactory<AuthSession>
{
    private const int RsaKeyPairCount = 20;
    private readonly ImmutableArray<RsaKeyPair> _rsaKeyPairs; 

    internal AuthSessionFactory()
    {
        // Create RSA key pairs
        _rsaKeyPairs = Enumerable.Range(0, RsaKeyPairCount).Select(x => new RsaKeyPair()).ToImmutableArray();
    }

    public AuthSession Create()
    {
        // Choose random RSA key pair
        RsaKeyPair rsaKeyPair = _rsaKeyPairs[RandomGenerator.GetInt32(RsaKeyPairCount)];
        
        // Generate random blowfish key
        byte[] blowfishKey = new byte[16]; 
        RandomGenerator.GetNonZeroBytes(blowfishKey);
        BlowfishEngine blowfishEngine = new BlowfishEngine(blowfishKey);
        return new AuthSession(rsaKeyPair, blowfishKey, blowfishEngine);
    }
}