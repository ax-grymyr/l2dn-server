using L2Dn.Events;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Items.Instances;

namespace L2Dn.GameServer.Model.Events.Impl.Items;

/**
 * @author UnAfraid
 */
public class OnItemBypassEvent: EventBase
{
	private readonly Item _item;
	private readonly Player _player;
	private readonly string _event;
	
	public OnItemBypassEvent(Item item, Player player, string @event)
	{
		_item = item;
		_player = player;
		_event = @event;
	}
	
	public Item getItem()
	{
		return _item;
	}
	
	public Player getActiveChar()
	{
		return _player;
	}
	
	public string getEvent()
	{
		return _event;
	}
}