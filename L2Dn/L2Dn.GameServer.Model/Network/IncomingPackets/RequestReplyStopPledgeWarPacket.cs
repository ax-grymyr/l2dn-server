using L2Dn.GameServer.Data.Sql;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Network.Enums;
using L2Dn.Network;
using L2Dn.Packets;

namespace L2Dn.GameServer.Network.IncomingPackets;

public struct RequestReplyStopPledgeWarPacket: IIncomingPacket<GameSession>
{
    private int _answer;

    public void ReadContent(PacketBitReader reader)
    {
        reader.ReadString();
        _answer = reader.ReadInt32();
    }

    public ValueTask ProcessAsync(Connection connection, GameSession session)
    {
        Player? player = session.Player;
        if (player == null || player.getClanId() is null)
            return ValueTask.CompletedTask;

        Player requestor = player.getActiveRequester();
        if (requestor == null || requestor.getClanId() is null)
            return ValueTask.CompletedTask;
		
        if (_answer == 1)
        {
            ClanTable.getInstance().deleteClanWars(requestor.getClanId().Value, player.getClanId().Value);
        }
        else
        {
            requestor.sendPacket(SystemMessageId.REQUEST_TO_END_WAR_HAS_BEEN_DENIED);
        }
		
        player.setActiveRequester(null);
        requestor.onTransactionResponse();
        return ValueTask.CompletedTask;
    }
}