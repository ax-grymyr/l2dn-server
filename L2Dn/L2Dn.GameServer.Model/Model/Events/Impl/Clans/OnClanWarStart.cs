using L2Dn.Events;
using L2Dn.GameServer.Model.Clans;

namespace L2Dn.GameServer.Model.Events.Impl.Clans;

public class OnClanWarStart: EventBase
{
    private readonly Clan _clan1;
    private readonly Clan _clan2;

    public OnClanWarStart(Clan clan1, Clan clan2)
    {
        _clan1 = clan1;
        _clan2 = clan2;
    }

    public Clan getClan1()
    {
        return _clan1;
    }

    public Clan getClan2()
    {
        return _clan2;
    }
}