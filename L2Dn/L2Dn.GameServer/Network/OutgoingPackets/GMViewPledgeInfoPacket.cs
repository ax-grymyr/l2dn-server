using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Clans;
using L2Dn.Packets;

namespace L2Dn.GameServer.Network.OutgoingPackets;

public readonly struct GMViewPledgeInfoPacket: IOutgoingPacket
{
    private readonly Clan _clan;
    private readonly Player _player;
	
    public GMViewPledgeInfoPacket(Clan clan, Player player)
    {
        _clan = clan;
        _player = player;
    }
	
    public void WriteContent(PacketBitWriter writer)
    {
        writer.WritePacketCode(OutgoingPacketCodes.GM_VIEW_PLEDGE_INFO);
        
        writer.WriteInt32(0);
        writer.WriteString(_player.getName());
        writer.WriteInt32(_clan.getId());
        writer.WriteInt32(0);
        writer.WriteString(_clan.getName());
        writer.WriteString(_clan.getLeaderName());
        writer.WriteInt32(_clan.getCrestId() ?? 0); // -> no, it's no longer used (nuocnam) fix by game
        writer.WriteInt32(_clan.getLevel());
        writer.WriteInt32(_clan.getCastleId() ?? 0);
        writer.WriteInt32(_clan.getHideoutId());
        writer.WriteInt32(_clan.getFortId() ?? 0);
        writer.WriteInt32(_clan.getRank());
        writer.WriteInt32(_clan.getReputationScore());
        writer.WriteInt32(0);
        writer.WriteInt32(0);
        writer.WriteInt32(0);
        writer.WriteInt32(_clan.getAllyId() ?? 0); // c2
        writer.WriteString(_clan.getAllyName()); // c2
        writer.WriteInt32(_clan.getAllyCrestId() ?? 0); // c2
        writer.WriteInt32(_clan.isAtWar()); // c3
        writer.WriteInt32(0); // T3 Unknown
        writer.WriteInt32(_clan.getMembers().Count);
        foreach (ClanMember member in _clan.getMembers())
        {
            if (member != null)
            {
                writer.WriteString(member.getName());
                writer.WriteInt32(member.getLevel());
                writer.WriteInt32((int)member.getClassId());
                writer.WriteInt32((int)member.getSex());
                writer.WriteInt32((int)member.getRace());
                writer.WriteInt32(member.isOnline() ? member.getObjectId() : 0);
                writer.WriteInt32(member.getSponsor() != 0);
            }
        }
    }
}