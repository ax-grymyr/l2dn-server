using L2Dn.GameServer.Handlers;
using L2Dn.GameServer.Model.Actor;
using L2Dn.Network;
using L2Dn.Packets;

namespace L2Dn.GameServer.Network.IncomingPackets;

public struct RequestOlympiadMatchListPacket: IIncomingPacket<GameSession>
{
    private const string COMMAND = "arenalist";

    public void ReadContent(PacketBitReader reader)
    {
    }

    public ValueTask ProcessAsync(Connection connection, GameSession session)
    {
        Player? player = session.Player;
        if (player == null || !player.inObserverMode())
            return ValueTask.CompletedTask;

        IBypassHandler? handler = BypassHandler.getInstance().getHandler(COMMAND);
        if (handler != null)
        {
            handler.useBypass(COMMAND, player, null);
        }

        return ValueTask.CompletedTask;
    }
}