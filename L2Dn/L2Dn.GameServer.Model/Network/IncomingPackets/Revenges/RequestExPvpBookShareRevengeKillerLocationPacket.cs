using L2Dn.GameServer.InstanceManagers;
using L2Dn.GameServer.Model.Actor;
using L2Dn.Network;
using L2Dn.Packets;

namespace L2Dn.GameServer.Network.IncomingPackets.Revenges;

public struct RequestExPvpBookShareRevengeKillerLocationPacket: IIncomingPacket<GameSession>
{
    private string _killerName;

    public void ReadContent(PacketBitReader reader)
    {
        reader.ReadSizedString(); // Victim name.
        _killerName = reader.ReadSizedString();
    }

    public ValueTask ProcessAsync(Connection connection, GameSession session)
    {
        Player? player = session.Player;
        if (player == null)
            return ValueTask.CompletedTask;

        RevengeHistoryManager.getInstance().locateKiller(player, _killerName);

        return ValueTask.CompletedTask;
    }
}