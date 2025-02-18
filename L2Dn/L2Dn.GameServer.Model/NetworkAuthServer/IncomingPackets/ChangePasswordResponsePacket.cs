using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor;
using L2Dn.Network;
using L2Dn.Packets;

namespace L2Dn.GameServer.NetworkAuthServer.IncomingPackets;

internal struct ChangePasswordResponsePacket: IIncomingPacket<AuthServerSession>
{
    private int _playerId;
    private ChangePasswordResult _result;

    public void ReadContent(PacketBitReader reader)
    {
        _playerId = reader.ReadInt32();
        _result = (ChangePasswordResult)reader.ReadInt32();
    }

    public ValueTask ProcessAsync(Connection connection, AuthServerSession session)
    {
        Player? player = World.getInstance().getPlayer(_playerId);
        if (player == null)
            return ValueTask.CompletedTask;

        string message = _result switch
        {
            ChangePasswordResult.Ok => "You have successfully changed your password!",
            ChangePasswordResult.InvalidPassword =>
                "The typed current password doesn't match with your current one.",

            _ => "The password change was unsuccessful!",
        };

        player.sendMessage(message);
        return ValueTask.CompletedTask;
    }
}