namespace L2Dn.GameServer.Model.Clans;

public class ClanInfo
{
    private readonly Clan _clan;
    private readonly int _total;
    private readonly int _online;

    public ClanInfo(Clan clan)
    {
        _clan = clan;
        _total = clan.getMembersCount();
        _online = clan.getOnlineMembersCount();
    }

    public Clan getClan()
    {
        return _clan;
    }

    public int getTotal()
    {
        return _total;
    }

    public int getOnline()
    {
        return _online;
    }
}