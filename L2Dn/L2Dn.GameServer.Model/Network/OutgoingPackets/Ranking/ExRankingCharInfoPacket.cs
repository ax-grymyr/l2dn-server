using L2Dn.GameServer.InstanceManagers;
using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Utilities;
using L2Dn.Packets;

namespace L2Dn.GameServer.Network.OutgoingPackets.Ranking;

public readonly struct ExRankingCharInfoPacket: IOutgoingPacket
{
    private readonly Player _player;
    private readonly Map<int, StatSet> _playerList;
    private readonly Map<int, StatSet> _snapshotList;

    public ExRankingCharInfoPacket(Player player)
    {
        _player = player;
        _playerList = RankManager.getInstance().getRankList();
        _snapshotList = RankManager.getInstance().getSnapshotList();
    }

    public void WriteContent(PacketBitWriter writer)
    {
        writer.WritePacketCode(OutgoingPacketCodes.EX_RANKING_CHAR_INFO);

        if (_playerList.Count != 0)
        {
            foreach (int id in _playerList.Keys)
            {
                StatSet player = _playerList[id];
                if (player.getInt("charId") == _player.ObjectId)
                {
                    writer.WriteInt32(id); // server rank
                    writer.WriteInt32(player.getInt("raceRank")); // race rank
                    writer.WriteInt32(player.getInt("classRank")); // class rank
                    foreach (int id2 in _snapshotList.Keys)
                    {
                        StatSet snapshot = _snapshotList[id2];
                        if (player.getInt("charId") == snapshot.getInt("charId"))
                        {
                            writer.WriteInt32(id2); // server rank snapshot
                            writer.WriteInt32(snapshot.getInt("classRank")); // class rank snapshot
                            writer.WriteInt32(player.getInt("classRank")); // class rank snapshot
                            writer.WriteInt32(0);
                            writer.WriteInt32(0);
                            writer.WriteInt32(0);
                            return;
                        }
                    }
                }
            }

            writer.WriteInt32(0); // server rank
            writer.WriteInt32(0); // race rank
            writer.WriteInt32(0); // server rank snapshot
            writer.WriteInt32(0); // race rank snapshot
            writer.WriteInt32(0); // nClassRank
            writer.WriteInt32(0); // nClassRank_Snapshot snapshot
        }
        else
        {
            writer.WriteInt32(0); // server rank
            writer.WriteInt32(0); // race rank
            writer.WriteInt32(0); // server rank snapshot
            writer.WriteInt32(0); // race rank snapshot
            writer.WriteInt32(0); // nClassRank
            writer.WriteInt32(0); // nClassRank_Snapshot snapshot
        }
    }
}