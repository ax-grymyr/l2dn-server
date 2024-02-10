using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Items.Instances;

namespace L2Dn.GameServer.Model.Events.Impl.Creatures.Players;

/**
 * @author UnAfraid
 */
public class OnPlayerItemDrop: IBaseEvent
{
	private readonly Player _player;
	private readonly Item _item;
	private readonly Location _loc;
	
	public OnPlayerItemDrop(Player player, Item item, Location loc)
	{
		_player = player;
		_item = item;
		_loc = loc;
	}
	
	public Player getPlayer()
	{
		return _player;
	}
	
	public Item getItem()
	{
		return _item;
	}
	
	public Location getLocation()
	{
		return _loc;
	}
	
	public EventType getType()
	{
		return EventType.ON_PLAYER_ITEM_DROP;
	}
}