using L2Dn.GameServer.InstanceManagers;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Network.OutgoingPackets;
using L2Dn.GameServer.Utilities;
using L2Dn.Network;
using L2Dn.Packets;

namespace L2Dn.GameServer.Network.IncomingPackets;

public struct RequestPledgeRecruitBoardSearchPacket: IIncomingPacket<GameSession>
{
    private int _clanLevel;
    private int _karma;
    private int _type;
    private string _query;
    private int _sort;
    private bool _descending;
    private int _page;
    private int _applicationType;

    public void ReadContent(PacketBitReader reader)
    {
        _clanLevel = reader.ReadInt32();
        _karma = reader.ReadInt32();
        _type = reader.ReadInt32();
        _query = reader.ReadString();
        _sort = reader.ReadInt32();
        _descending = reader.ReadInt32() == 2;
        _page = reader.ReadInt32();
        _applicationType = reader.ReadInt32(); // Helios
    }

    public ValueTask ProcessAsync(Connection connection, GameSession session)
    {
        Player? player = session.Player;
        if (player == null)
            return ValueTask.CompletedTask;

        if (string.IsNullOrEmpty(_query))
        {
            if (_karma < 0 && _clanLevel < 0)
            {
                player.sendPacket(
                    new ExPledgeRecruitBoardSearchPacket(ClanEntryManager.getInstance().getUnSortedClanList(), _page));
            }
            else
            {
                player.sendPacket(new ExPledgeRecruitBoardSearchPacket(
                    ClanEntryManager.getInstance().getSortedClanList(_clanLevel, _karma, _sort, _descending), _page));
            }
        }
        else
        {
            player.sendPacket(new ExPledgeRecruitBoardSearchPacket(
                ClanEntryManager.getInstance().getSortedClanListByName(_query.toLowerCase(), _type), _page));
        }

        return ValueTask.CompletedTask;
    }
}