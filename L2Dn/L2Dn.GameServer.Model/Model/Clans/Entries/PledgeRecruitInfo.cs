using L2Dn.GameServer.Data.Sql;

namespace L2Dn.GameServer.Model.Clans.Entries;

public class PledgeRecruitInfo(
    Clan clan, int karma, string information, string detailedInformation, int applicationType, int recruitType)
{
    private int _karma = karma;
    private string _information = information;
    private string _detailedInformation = detailedInformation;

    public int getClanId()
    {
        return clan.getId();
    }

    public string getClanName()
    {
        return clan.getName();
    }

    public string getClanLeaderName()
    {
        return clan.getLeaderName();
    }

    public int getClanLevel()
    {
        return clan.getLevel();
    }

    public int getKarma()
    {
        return _karma;
    }

    public void setKarma(int karma)
    {
        _karma = karma;
    }

    public string getInformation()
    {
        return _information;
    }

    public void setInformation(string information)
    {
        _information = information;
    }

    public string getDetailedInformation()
    {
        return _detailedInformation;
    }

    public void setDetailedInformation(string detailedInformation)
    {
        _detailedInformation = detailedInformation;
    }

    public int getApplicationType()
    {
        return applicationType;
    }

    public int getRecruitType()
    {
        return recruitType;
    }

    public Clan getClan()
    {
        return clan;
    }
}