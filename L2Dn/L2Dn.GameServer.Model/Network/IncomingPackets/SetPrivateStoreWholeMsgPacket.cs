using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Network.OutgoingPackets;
using L2Dn.GameServer.Utilities;
using L2Dn.Network;
using L2Dn.Packets;
using Config = L2Dn.GameServer.Configuration.Config;

namespace L2Dn.GameServer.Network.IncomingPackets;

public struct SetPrivateStoreWholeMsgPacket: IIncomingPacket<GameSession>
{
    private const int MAX_MSG_LENGTH = 29;

    private string _msg;

    public void ReadContent(PacketBitReader reader)
    {
        _msg = reader.ReadString();
    }

    public ValueTask ProcessAsync(Connection connection, GameSession session)
    {
        Player? player = session.Player;
        if (player == null || player.getSellList() == null)
            return ValueTask.CompletedTask;

        if (_msg != null && _msg.Length > MAX_MSG_LENGTH)
        {
            Util.handleIllegalPlayerAction(player, player + " tried to overflow private store whole message",
                Config.General.DEFAULT_PUNISH);

            return ValueTask.CompletedTask;
        }

        player.getSellList().setTitle(_msg ?? string.Empty);
        player.sendPacket(new ExPrivateStoreSetWholeMsgPacket(player));

        return ValueTask.CompletedTask;
    }
}