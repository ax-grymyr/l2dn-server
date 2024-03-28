using L2Dn.Network;
using L2Dn.Packets;

namespace L2Dn.GameServer.Network.IncomingPackets;

public struct GameGuardReplyPacket: IIncomingPacket<GameSession>
{
    private static readonly byte[] _valid =
    [
        0x88, 0x40, 0x1c, 0xa7, 0x83, 0x42, 0xe9, 0x15, 0xde, 0xc3, 0x68, 0xf6, 0x2d, 0x23, 0xf1, 0x3f, 0xee, 0x68,
        0x5b, 0xc5
    ];

    private int _reply1;
    private int _reply2;
    
    public void ReadContent(PacketBitReader reader)
    {
        _reply1 = reader.ReadInt32();
        reader.ReadInt32();
        _reply2 = reader.ReadInt32();
    }

    public ValueTask ProcessAsync(Connection connection, GameSession session)
    {
        // try
        // {
        //     final MessageDigest md = MessageDigest.getInstance("SHA");
        //     final byte[] result = md.digest(_reply);
        //     if (Arrays.equals(result, VALID))
        //     {
        //         client.setGameGuardOk(true);
        //     }
        // }
        // catch (NoSuchAlgorithmException e)
        // {
        //     PacketLogger.warning(getClass().getSimpleName() + ": " + e.getMessage());
        // }
        return ValueTask.CompletedTask;
    }
}