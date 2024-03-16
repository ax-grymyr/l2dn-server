using L2Dn.Events;
using L2Dn.GameServer.Model.InstanceZones;

namespace L2Dn.GameServer.Model.Events.Impl.Instances;

/**
 * @author malyelfik
 */
public class OnInstanceDestroy: EventBase
{
	private readonly Instance _instance;

	public OnInstanceDestroy(Instance instance)
	{
		_instance = instance;
	}

	public Instance getInstanceWorld()
	{
		return _instance;
	}
}