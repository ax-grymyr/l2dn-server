using L2Dn.Events;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.InstanceZones;

namespace L2Dn.GameServer.Model.Events.Impl.Instances;

/**
 * @author malyeflik
 */
public class OnInstanceLeave: EventBase
{
	private readonly Player _player;
	private readonly Instance _instance;
	
	public OnInstanceLeave(Player player, Instance instance)
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
}