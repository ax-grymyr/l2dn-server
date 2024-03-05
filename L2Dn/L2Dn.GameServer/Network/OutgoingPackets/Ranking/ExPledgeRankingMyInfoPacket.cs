using L2Dn.GameServer.InstanceManagers;
using L2Dn.GameServer.Model.Actor;
using L2Dn.Packets;

namespace L2Dn.GameServer.Network.OutgoingPackets.Ranking;

public readonly struct ExPledgeRankingMyInfoPacket: IOutgoingPacket
{
    private readonly Player _player;
	
    public ExPledgeRankingMyInfoPacket(Player player)
    {
        _player = player;
    }
	
    public void WriteContent(PacketBitWriter writer)
    {
        writer.WritePacketCode(OutgoingPacketCodes.EX_PLEDGE_RANKING_MY_INFO);

        int? clanId = _player.getClanId();
        writer.WriteInt32(_player.getClan() != null ? RankManager.getInstance().getClanRankList().FirstOrDefault(it => it.Value.getInt("clan_id") == clanId).Key : 0); // rank
        writer.WriteInt32(_player.getClan() != null ? RankManager.getInstance().getSnapshotClanRankList().FirstOrDefault(it => it.Value.getInt("clan_id") == clanId).Key : 0); // snapshot
        writer.WriteInt32(_player.getClan() != null ? _player.getClan().getExp() : 0); // exp
    }
}