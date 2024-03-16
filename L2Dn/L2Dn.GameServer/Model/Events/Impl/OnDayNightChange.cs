using L2Dn.Events;

namespace L2Dn.GameServer.Model.Events.Impl;

public class OnDayNightChange: EventBase
{
    private readonly bool _isNight;

    public OnDayNightChange(bool isNight)
    {
        _isNight = isNight;
    }

    public bool isNight()
    {
        return _isNight;
    }
}