using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Network.OutgoingPackets;
using L2Dn.GameServer.Utilities;
using L2Dn.Network;
using L2Dn.Packets;
using Config = L2Dn.GameServer.Configuration.Config;

namespace L2Dn.GameServer.Network.IncomingPackets;

public struct SetPrivateStoreMsgBuyPacket: IIncomingPacket<GameSession>
{
    private const int MAX_MSG_LENGTH = 29;

    private string _storeMsg;

    public void ReadContent(PacketBitReader reader)
    {
        _storeMsg = reader.ReadString();
    }

    public ValueTask ProcessAsync(Connection connection, GameSession session)
    {
        Player? player = session.Player;
        if (player == null || player.getBuyList() == null)
            return ValueTask.CompletedTask;

        if (_storeMsg != null && _storeMsg.Length > MAX_MSG_LENGTH)
        {
            Util.handleIllegalPlayerAction(player, player + " tried to overflow private store buy message",
                Config.DEFAULT_PUNISH);

            return ValueTask.CompletedTask;
        }

        player.getBuyList().setTitle(_storeMsg ?? string.Empty);
        player.sendPacket(new PrivateStoreMsgBuyPacket(player));

        return ValueTask.CompletedTask;
    }
}