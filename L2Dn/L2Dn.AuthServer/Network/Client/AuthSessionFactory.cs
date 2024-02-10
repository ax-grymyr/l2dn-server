using L2Dn.AuthServer.Cryptography;
using L2Dn.Cryptography;
using L2Dn.Network;

namespace L2Dn.AuthServer.Network.Client;

internal sealed class AuthSessionFactory: ISessionFactory<AuthSession>
{
    private readonly RsaKeyPair _rsaKeyPair;
    private readonly byte[] _blowfishKey;
    private readonly OldBlowfishEngine _blowfishEngine;

    internal AuthSessionFactory()
    {
        _rsaKeyPair = new RsaKeyPair();
        _blowfishKey = new byte[16];
        RandomGenerator.GetNonZeroBytes(_blowfishKey);
        _blowfishEngine = new OldBlowfishEngine(_blowfishKey);
    }

    public AuthSession Create() => new(_rsaKeyPair, _blowfishKey, _blowfishEngine);
}
