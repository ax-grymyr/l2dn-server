using L2Dn.AuthServer.Cryptography;
using L2Dn.AuthServer.Model;
using L2Dn.Cryptography;
using L2Dn.Network;

namespace L2Dn.AuthServer.Network;

internal sealed class AuthSession(
    RsaKeyPair rsaKeyPair,
    ReadOnlyMemory<byte> blowfishKey,
    BlowfishEngine blowfishEngine)
    : Session
{
    public AuthSessionState State { get; set; } = AuthSessionState.Authorization;
    public ReadOnlyMemory<byte> BlowfishKey => blowfishKey;
    public BlowfishEngine BlowfishEngine => blowfishEngine;
    public RsaKeyPair RsaKeyPair => rsaKeyPair;
    public int LoginKey1 { get; } = RandomGenerator.GetInt32();
    public int LoginKey2 { get; } = RandomGenerator.GetInt32();
    public int PlayKey1 { get; } = RandomGenerator.GetInt32();
    public int PlayKey2 { get; } = RandomGenerator.GetInt32();

    public AccountInfo? AccountInfo { get; set; }
    
    protected override long GetState() => (long)State;
}