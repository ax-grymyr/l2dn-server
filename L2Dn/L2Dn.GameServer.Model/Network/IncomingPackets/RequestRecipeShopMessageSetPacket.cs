using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.StaticData;
using L2Dn.GameServer.Utilities;
using L2Dn.Network;
using L2Dn.Packets;

namespace L2Dn.GameServer.Network.IncomingPackets;

public struct RequestRecipeShopMessageSetPacket: IIncomingPacket<GameSession>
{
    private const int MAX_MSG_LENGTH = 29;

    private string _name;

    public void ReadContent(PacketBitReader reader)
    {
        _name = reader.ReadString();
    }

    public ValueTask ProcessAsync(Connection connection, GameSession session)
    {
        Player? player = session.Player;
        if (player == null)
            return ValueTask.CompletedTask;

        if (_name != null && _name.Length > MAX_MSG_LENGTH)
        {
            Util.handleIllegalPlayerAction(player, player + " tried to overflow recipe shop message",
                Config.DEFAULT_PUNISH);

            return ValueTask.CompletedTask;
        }

        player.setStoreName(_name ?? string.Empty);

        return ValueTask.CompletedTask;
    }
}