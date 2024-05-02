using L2Dn.GameServer.Model.InstanceZones;
using L2Dn.Geometry;

namespace L2Dn.GameServer.Model.Events.Impl.Base;

public abstract class LocationEventBase: TerminateEventBase
{
    public LocationHeading? OverridenLocation { get; set; }
    public Instance? OverridenInstance { get; set; }
}