using L2Dn.GameServer.Configuration;
using L2Dn.Network;

namespace L2Dn.GameServer.Network;

public sealed class GameSessionFactory: ISessionFactory<GameSession>
{
    private static readonly byte[] _cryptKey =
    {
        0x94, 0x35, 0x00, 0x00,
        0xa1, 0x6c, 0x54, 0x87 // these 4 bytes are fixed
    };

    public GameSession Create()
    {
        //byte[] encryptionKey = new byte[16];
        //RandomGenerator.GetNonZeroBytes(encryptionKey);

        byte[]? cryptKey = ServerConfig.Instance.ClientListener.Encryption ? _cryptKey : null;
        return new GameSession(cryptKey);
    }
}