using L2Dn.GameServer.InstanceManagers;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Sieges;
using L2Dn.GameServer.Network.OutgoingPackets;
using L2Dn.GameServer.Utilities;
using L2Dn.Network;
using L2Dn.Packets;

namespace L2Dn.GameServer.Network.IncomingPackets;

public struct RequestFortressMapInfoPacket: IIncomingPacket<GameSession>
{
    private int _fortressId;

    public void ReadContent(PacketBitReader reader)
    {
        _fortressId = reader.ReadInt32();
    }

    public ValueTask ProcessAsync(Connection connection, GameSession session)
    {
        Player? player = session.Player;
        if (player == null)
            return ValueTask.CompletedTask;

        Fort? fort = FortManager.getInstance().getFortById(_fortressId);
        if (fort == null)
        {
            PacketLogger.Instance.Warn("Fort is not found with id (" + _fortressId + ") in all forts with size of (" +
                                       FortManager.getInstance().getForts().Count + ") called by player (" + player +
                                       ")");

            player.sendPacket(ActionFailedPacket.STATIC_PACKET);
            return ValueTask.CompletedTask;
        }

        player.sendPacket(new ExShowFortressMapInfoPacket(fort));

        return ValueTask.CompletedTask;
    }
}