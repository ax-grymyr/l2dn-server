using L2Dn.GameServer.Model.InstanceZones;

namespace L2Dn.GameServer.Model.Events.Impl.Base;

public abstract class LocationEventBase: TerminateEventBase
{
    public Location OverridenLocation { get; set; }
    public Instance OverridenInstance { get; set; }
    public bool OverrideLocation { get; set; }
}