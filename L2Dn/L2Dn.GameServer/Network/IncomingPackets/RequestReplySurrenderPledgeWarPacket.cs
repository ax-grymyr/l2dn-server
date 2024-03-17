using L2Dn.GameServer.Data.Sql;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Utilities;
using L2Dn.Network;
using L2Dn.Packets;

namespace L2Dn.GameServer.Network.IncomingPackets;

public struct RequestReplySurrenderPledgeWarPacket: IIncomingPacket<GameSession>
{
    private string _reqName;
    private int _answer;

    public void ReadContent(PacketBitReader reader)
    {
        _reqName = reader.ReadString();
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
            PacketLogger.Instance.Info(GetType().Name + ": Missing implementation for answer: " + _answer +
                                       " and name: " + _reqName + "!");

        }
        
        player.onTransactionRequest(requestor);
        return ValueTask.CompletedTask;
    }
}