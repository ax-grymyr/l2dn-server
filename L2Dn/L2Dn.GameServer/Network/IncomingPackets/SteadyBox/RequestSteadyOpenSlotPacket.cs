using L2Dn.GameServer.Model.Actor;
using L2Dn.Network;
using L2Dn.Packets;

namespace L2Dn.GameServer.Network.IncomingPackets.SteadyBox;

public struct RequestSteadyOpenSlotPacket: IIncomingPacket<GameSession>
{
    private int _slotId;

    public void ReadContent(PacketBitReader reader)
    {
        _slotId = reader.ReadInt32();
    }

    public ValueTask ProcessAsync(Connection connection, GameSession session)
    {
        Player? player = session.Player;
        if (player == null)
            return ValueTask.CompletedTask;

        player.getAchievementBox().unlockSlot(_slotId);
        
        return ValueTask.CompletedTask;
    }
}