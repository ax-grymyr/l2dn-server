using L2Dn.Events;
using L2Dn.GameServer.Model.InstanceZones;

namespace L2Dn.GameServer.Model.Events.Impl.Instances;

/**
 * @author malyelfik
 */
public class OnInstanceStatusChange: EventBase
{
	private readonly Instance _world;
	private readonly int _status;
	
	public OnInstanceStatusChange(Instance world, int status)
	{
		_world = world;
		_status = status;
	}
	
	public Instance getWorld()
	{
		return _world;
	}
	
	public int getStatus()
	{
		return _status;
	}
}