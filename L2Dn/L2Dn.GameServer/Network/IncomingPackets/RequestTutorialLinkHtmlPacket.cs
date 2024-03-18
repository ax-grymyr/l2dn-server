using L2Dn.GameServer.Handlers;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Utilities;
using L2Dn.Network;
using L2Dn.Packets;

namespace L2Dn.GameServer.Network.IncomingPackets;

public struct RequestTutorialLinkHtmlPacket: IIncomingPacket<GameSession>
{
    private string _bypass;

    public void ReadContent(PacketBitReader reader)
    {
        reader.ReadInt32();
        _bypass = reader.ReadString();
    }

    public ValueTask ProcessAsync(Connection connection, GameSession session)
    {
        Player? player = session.Player;
        if (player == null)
            return ValueTask.CompletedTask;
		
        if (_bypass.startsWith("admin_"))
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

        return ValueTask.CompletedTask;
    }
}