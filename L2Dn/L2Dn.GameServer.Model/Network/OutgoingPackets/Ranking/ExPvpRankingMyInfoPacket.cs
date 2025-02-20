using L2Dn.GameServer.InstanceManagers;
using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Utilities;
using L2Dn.Packets;

namespace L2Dn.GameServer.Network.OutgoingPackets.Ranking;

public readonly struct ExPvpRankingMyInfoPacket: IOutgoingPacket
{
    private readonly Player _player;
    private readonly Map<int, StatSet> _playerList;
    private readonly Map<int, StatSet> _snapshotList;

    public ExPvpRankingMyInfoPacket(Player player)
    {
        _player = player;
        _playerList = RankManager.getInstance().getPvpRankList();
        _snapshotList = RankManager.getInstance().getSnapshotPvpRankList();
    }

    public void WriteContent(PacketBitWriter writer)
    {
        writer.WritePacketCode(OutgoingPacketCodes.EX_PVP_RANKING_MY_INFO);

        if (_playerList.Count != 0)
        {
            bool found = false;
            foreach (int id in _playerList.Keys)
            {
                StatSet ss = _playerList[id];
                if (ss.getInt("charId") == _player.ObjectId)
                {
                    int playerId = _player.ObjectId;
                    var snapshotValue = _snapshotList.FirstOrDefault(it => it.Value.getInt("charId") == playerId);
                    found = true;
                    writer.WriteInt64(ss.getInt("points")); // pvp points
                    writer.WriteInt32(id); // current rank
                    writer.WriteInt32(snapshotValue.Value != null ? snapshotValue.Key : id); // ingame shown change in rank as this value - current rank value.
                    writer.WriteInt32(ss.getInt("kills")); // kills
                    writer.WriteInt32(ss.getInt("deaths")); // deaths
                }
            }
            if (!found)
            {
                writer.WriteInt64(0); // pvp points
                writer.WriteInt32(0); // current rank
                writer.WriteInt32(0); // ingame shown change in rank as this value - current rank value.
                writer.WriteInt32(0); // kills
                writer.WriteInt32(0); // deaths
            }
        }
        else
        {
            writer.WriteInt64(0); // pvp points
            writer.WriteInt32(0); // current rank
            writer.WriteInt32(0); // ingame shown change in rank as this value - current rank value.
            writer.WriteInt32(0); // kills
            writer.WriteInt32(0); // deaths
        }
    }
}