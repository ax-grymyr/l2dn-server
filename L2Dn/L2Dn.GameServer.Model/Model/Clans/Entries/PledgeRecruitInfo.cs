using L2Dn.GameServer.Data.Sql;

namespace L2Dn.GameServer.Model.Clans.Entries;

public class PledgeRecruitInfo
{
    private int _clanId;
    private int _karma;
    private String _information;
    private String _detailedInformation;
    private readonly Clan _clan;
    private readonly int _applicationType;
    private readonly int _recruitType;
	
    public PledgeRecruitInfo(int clanId, int karma, String information, String detailedInformation, int applicationType, int recruitType)
    {
        _clanId = clanId;
        _karma = karma;
        _information = information;
        _detailedInformation = detailedInformation;
        _clan = ClanTable.getInstance().getClan(clanId);
        _applicationType = applicationType;
        _recruitType = recruitType;
    }
	
    public int getClanId()
    {
        return _clanId;
    }
	
    public void setClanId(int clanId)
    {
        _clanId = clanId;
    }
	
    public String getClanName()
    {
        return _clan.getName();
    }
	
    public String getClanLeaderName()
    {
        return _clan.getLeaderName();
    }
	
    public int getClanLevel()
    {
        return _clan.getLevel();
    }
	
    public int getKarma()
    {
        return _karma;
    }
	
    public void setKarma(int karma)
    {
        _karma = karma;
    }
	
    public String getInformation()
    {
        return _information;
    }
	
    public void setInformation(String information)
    {
        _information = information;
    }
	
    public String getDetailedInformation()
    {
        return _detailedInformation;
    }
	
    public void setDetailedInformation(String detailedInformation)
    {
        _detailedInformation = detailedInformation;
    }
	
    public int getApplicationType()
    {
        return _applicationType;
    }
	
    public int getRecruitType()
    {
        return _recruitType;
    }
	
    public Clan getClan()
    {
        return _clan;
    }
}