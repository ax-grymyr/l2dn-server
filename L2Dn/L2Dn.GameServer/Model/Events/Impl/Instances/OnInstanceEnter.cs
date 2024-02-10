using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.InstanceZones;

namespace L2Dn.GameServer.Model.Events.Impl.Instances;

/**
 * @author malyelfik
 */
public class OnInstanceEnter: IBaseEvent
{
	private readonly Player _player;
	private readonly Instance _instance;
	
	public OnInstanceEnter(Player player, Instance instance)
	{
		_player = player;
		_instance = instance;
	}
	
	public Player getPlayer()
	{
		return _player;
	}
	
	public Instance getInstanceWorld()
	{
		return _instance;
	}
	
	public EventType getType()
	{
		return EventType.ON_INSTANCE_ENTER;
	}
}