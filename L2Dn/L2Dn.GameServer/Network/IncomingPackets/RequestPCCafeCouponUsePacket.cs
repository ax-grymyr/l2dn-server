using L2Dn.GameServer.Utilities;
using L2Dn.Network;
using L2Dn.Packets;

namespace L2Dn.GameServer.Network.IncomingPackets;

public struct RequestPCCafeCouponUsePacket: IIncomingPacket<GameSession>
{
    private string _str;

    public void ReadContent(PacketBitReader reader)
    {
        // TODO: This packet is wrong in Gracia Final!!
        _str = reader.ReadString();
    }

    public ValueTask ProcessAsync(Connection connection, GameSession session)
    {
        PacketLogger.Instance.Info("C5: RequestPCCafeCouponUse: S: " + _str);
        return ValueTask.CompletedTask;
    }
}