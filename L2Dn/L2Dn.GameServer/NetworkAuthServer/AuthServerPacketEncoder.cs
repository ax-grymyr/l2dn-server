using L2Dn.Cryptography;
using L2Dn.GameServer.Cryptography;

namespace L2Dn.GameServer.NetworkAuthServer;

internal sealed class AuthServerPacketEncoder: IPacketEncoder
{
    private readonly C4GamePacketEncoder _gamePacketEncoder;

    public AuthServerPacketEncoder()
    {
        ReadOnlySpan<byte> blowfishKey = "N-%H\"$*iP{)U&/bK,{{zo4P;"u8;
        _gamePacketEncoder = new C4GamePacketEncoder(blowfishKey);
    }

    public int GetRequiredLength(int length) => _gamePacketEncoder.GetRequiredLength(length);
    public int Encode(Span<byte> buffer, int packetLength) => _gamePacketEncoder.Encode(buffer, packetLength);
    public bool Decode(Span<byte> packet) => _gamePacketEncoder.Decode(packet);
}