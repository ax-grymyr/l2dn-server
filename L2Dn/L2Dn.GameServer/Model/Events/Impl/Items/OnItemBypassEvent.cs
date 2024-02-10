using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Items.Instances;

namespace L2Dn.GameServer.Model.Events.Impl.Items;

/**
 * @author UnAfraid
 */
public class OnItemBypassEvent: IBaseEvent
{
	private readonly Item _item;
	private readonly Player _player;
	private readonly String _event;
	
	public OnItemBypassEvent(Item item, Player player, String @event)
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
	
	public String getEvent()
	{
		return _event;
	}
	
	public EventType getType()
	{
		return EventType.ON_ITEM_BYPASS_EVENT;
	}
}