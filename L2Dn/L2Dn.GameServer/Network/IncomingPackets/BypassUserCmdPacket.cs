using L2Dn.GameServer.Handlers;
using L2Dn.GameServer.Model.Actor;
using L2Dn.Network;
using L2Dn.Packets;

namespace L2Dn.GameServer.Network.IncomingPackets;

public struct BypassUserCmdPacket: IIncomingPacket<GameSession>
{
    private int _command;

    public void ReadContent(PacketBitReader reader)
    {
        _command = reader.ReadInt32();
    }

    public ValueTask ProcessAsync(Connection connection, GameSession session)
    {
        Player? player = session.Player;
        if (player == null)
            return ValueTask.CompletedTask;
		
        IUserCommandHandler handler = UserCommandHandler.getInstance().getHandler(_command);
        if (handler == null)
        {
            if (player.isGM())
            {
                player.sendMessage("User commandID " + _command + " not implemented yet.");
            }
        }
        else
        {
            handler.useUserCommand(_command, player);
        }
        
        return ValueTask.CompletedTask;
    }
}