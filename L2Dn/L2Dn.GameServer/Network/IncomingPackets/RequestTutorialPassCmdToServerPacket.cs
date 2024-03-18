using L2Dn.GameServer.Handlers;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Events.Impl.Players;
using L2Dn.Network;
using L2Dn.Packets;

namespace L2Dn.GameServer.Network.IncomingPackets;

public struct RequestTutorialPassCmdToServerPacket: IIncomingPacket<GameSession>
{
    private string _bypass;

    public void ReadContent(PacketBitReader reader)
    {
        _bypass = reader.ReadString();
    }

    public ValueTask ProcessAsync(Connection connection, GameSession session)
    {
        Player? player = session.Player;
        if (player == null)
            return ValueTask.CompletedTask;
		
        if (_bypass.StartsWith("admin_"))
        {
            AdminCommandHandler.getInstance().useAdminCommand(player, _bypass, true);
        }
        else
        {
            IBypassHandler handler = BypassHandler.getInstance().getHandler(_bypass);
            if (handler != null)
            {
                handler.useBypass(_bypass, player, null);
            }
        }
		
        if (player.Events.HasSubscribers<OnPlayerBypass>())
        {
            player.Events.NotifyAsync(new OnPlayerBypass(player, _bypass));
        }

        return ValueTask.CompletedTask;
    }
}