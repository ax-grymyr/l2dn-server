using L2Dn.GameServer.Model.Actor;
using L2Dn.Network;
using L2Dn.Packets;

namespace L2Dn.GameServer.Network.IncomingPackets.SteadyBox;

public struct RequestSteadyOpenBoxPacket: IIncomingPacket<GameSession>
{
    private int _slotId;
    private long _feeBoxPrice;

    public void ReadContent(PacketBitReader reader)
    {
        _slotId = reader.ReadInt32();
        _feeBoxPrice = reader.ReadInt64();
    }

    public ValueTask ProcessAsync(Connection connection, GameSession session)
    {
        Player? player = session.Player;
        if (player == null)
            return ValueTask.CompletedTask;

        if (_feeBoxPrice > 0)
        {
            player.getAchievementBox().skipBoxOpenTime(_slotId, _feeBoxPrice);
        }
        else
        {
            player.getAchievementBox().openBox(_slotId);
        }
        
        return ValueTask.CompletedTask;
    }
}