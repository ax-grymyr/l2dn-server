using L2Dn.GameServer.InstanceManagers;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Network.OutgoingPackets;
using L2Dn.GameServer.Utilities;
using L2Dn.Network;
using L2Dn.Packets;

namespace L2Dn.GameServer.Network.IncomingPackets;

public struct RequestPledgeDraftListSearchPacket: IIncomingPacket<GameSession>
{
    private int _levelMin;
    private int _levelMax;
    private int _classId;
    private string _query;
    private int _sortBy;
    private bool _descending;

    public void ReadContent(PacketBitReader reader)
    {
        _levelMin = Math.Clamp(reader.ReadInt32(), 0, 107);
        _levelMax = Math.Clamp(reader.ReadInt32(), 0, 107);
        _classId = reader.ReadInt32();
        _query = reader.ReadString();
        _sortBy = reader.ReadInt32();
        _descending = reader.ReadInt32() == 2;
    }

    public ValueTask ProcessAsync(Connection connection, GameSession session)
    {
        Player? player = session.Player;
        if (player == null)
            return ValueTask.CompletedTask;

        if (string.IsNullOrEmpty(_query))
        {
            player.sendPacket(new ExPledgeDraftListSearchPacket(ClanEntryManager.getInstance()
                .getSortedWaitingList(_levelMin, _levelMax, _classId, _sortBy, _descending)));
        }
        else
        {
            player.sendPacket(new ExPledgeDraftListSearchPacket(ClanEntryManager.getInstance()
                .queryWaitingListByName(_query.toLowerCase())));
        }

        return ValueTask.CompletedTask;
    }
}