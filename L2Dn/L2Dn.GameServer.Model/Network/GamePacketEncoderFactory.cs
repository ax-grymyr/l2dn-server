using L2Dn.Cryptography;
using L2Dn.GameServer.Configuration;
using L2Dn.GameServer.Cryptography;
using L2Dn.Packets;

namespace L2Dn.GameServer.Network;

public sealed class GamePacketEncoderFactory: IPacketEncoderFactory<GameSession>
{
    private static readonly byte[] _cryptKey =
    {
        0x94, 0x35, 0x00, 0x00,
        0xa1, 0x6c, 0x54, 0x87 // these 4 bytes are fixed
    };

    private static readonly byte[] _cryptKey2 =
    {
        0x94, 0x35, 0x00, 0x00, 0xa1, 0x6c, 0x54, 0x87,
        0xC8, 0x27, 0x93, 0x01, 0xA1, 0x6C, 0x31, 0x97 // constant part
    };

    public PacketEncoder Create(GameSession session) =>
        ServerConfig.Instance.ClientListener.Encryption ? new C4GamePacketEncoder(_cryptKey) : new NullPacketEncoder();
}