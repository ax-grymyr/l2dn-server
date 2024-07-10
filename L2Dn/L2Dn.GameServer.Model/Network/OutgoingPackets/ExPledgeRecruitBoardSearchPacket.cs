using L2Dn.GameServer.Model.Clans;
using L2Dn.GameServer.Model.Clans.Entries;
using L2Dn.GameServer.Utilities;
using L2Dn.Packets;

namespace L2Dn.GameServer.Network.OutgoingPackets;

public readonly struct ExPledgeRecruitBoardSearchPacket: IOutgoingPacket
{
    private const int CLAN_PER_PAGE = 12;
    private readonly List<PledgeRecruitInfo> _clanList;
    private readonly int _currentPage;
    private readonly int _totalNumberOfPage;
    private readonly int _clanOnCurrentPage;
    private readonly int _startIndex;
    private readonly int _endIndex;
	
    public ExPledgeRecruitBoardSearchPacket(List<PledgeRecruitInfo> clanList, int currentPage)
    {
        _clanList = clanList;
        _currentPage = currentPage;
        _totalNumberOfPage = (int)Math.Ceiling((double) _clanList.Count / CLAN_PER_PAGE);
        _startIndex = (_currentPage - 1) * CLAN_PER_PAGE;
        _endIndex = (_startIndex + CLAN_PER_PAGE) > _clanList.Count ? _clanList.Count : _startIndex + CLAN_PER_PAGE;
        _clanOnCurrentPage = _endIndex - _startIndex;
    }
	
    public void WriteContent(PacketBitWriter writer)
    {
        writer.WritePacketCode(OutgoingPacketCodes.EX_PLEDGE_RECRUIT_BOARD_SEARCH);
        
        writer.WriteInt32(_currentPage);
        writer.WriteInt32(_totalNumberOfPage);
        writer.WriteInt32(_clanOnCurrentPage);
        for (int i = _startIndex; i < _endIndex; i++)
        {
            writer.WriteInt32(_clanList.get(i).getClanId());
            writer.WriteInt32(_clanList.get(i).getClan().getAllyId() ?? 0);
        }
        
        for (int i = _startIndex; i < _endIndex; i++)
        {
            Clan clan = _clanList.get(i).getClan();
            writer.WriteInt32(clan.getCrestId() ?? 0);
            writer.WriteInt32(clan.getAllyCrestId() ?? 0);
            writer.WriteString(clan.getName());
            writer.WriteString(clan.getLeaderName());
            writer.WriteInt32(clan.getLevel());
            writer.WriteInt32(clan.getMembersCount());
            writer.WriteInt32(_clanList.get(i).getKarma());
            writer.WriteString(_clanList.get(i).getInformation());
            writer.WriteInt32(_clanList.get(i).getApplicationType()); // Helios
            writer.WriteInt32(_clanList.get(i).getRecruitType()); // Helios
        }
    }
}