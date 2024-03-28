using L2Dn.GameServer.Handlers;
using L2Dn.GameServer.Model.Actor;
using L2Dn.Network;
using L2Dn.Packets;

namespace L2Dn.GameServer.Network.IncomingPackets;

public struct SendBypassBuildCmdPacket: IIncomingPacket<GameSession>
{
    private string? _command;

    public void ReadContent(PacketBitReader reader)
    {
        _command = reader.ReadString();
    }

    public ValueTask ProcessAsync(Connection connection, GameSession session)
    {
        Player? player = session.Player;
        if (player == null)
            return ValueTask.CompletedTask;
		
        AdminCommandHandler.getInstance().useAdminCommand(player, "admin_" + _command, true);
        return ValueTask.CompletedTask;
    }
}