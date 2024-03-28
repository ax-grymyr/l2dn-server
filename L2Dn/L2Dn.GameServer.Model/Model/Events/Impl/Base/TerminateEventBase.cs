using L2Dn.Events;

namespace L2Dn.GameServer.Model.Events.Impl.Base;

public abstract class TerminateEventBase: EventBase
{
    public bool Terminate { get; set; }
}