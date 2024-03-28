using L2Dn.GameServer.InstanceManagers;
using L2Dn.GameServer.Model.Actor;
using L2Dn.Network;
using L2Dn.Packets;

namespace L2Dn.GameServer.Network.IncomingPackets.Revenges;

public struct RequestExPvpBookShareRevengeSharedTeleportToKillerPacket: IIncomingPacket<GameSession>
{
    private string _victimName;
    private string _killerName;

    public void ReadContent(PacketBitReader reader)
    {
        _victimName = reader.ReadSizedString();
        _killerName = reader.ReadSizedString();
    }

    public ValueTask ProcessAsync(Connection connection, GameSession session)
    {
        Player? player = session.Player;
        if (player == null)
            return ValueTask.CompletedTask;

        RevengeHistoryManager.getInstance().teleportToSharedKiller(player, _victimName, _killerName);
        
        return ValueTask.CompletedTask;
    }
}