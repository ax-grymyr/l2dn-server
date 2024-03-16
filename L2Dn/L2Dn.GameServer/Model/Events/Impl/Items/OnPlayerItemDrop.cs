using L2Dn.Events;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Items.Instances;

namespace L2Dn.GameServer.Model.Events.Impl.Items;

/**
 * @author UnAfraid
 */
public class OnPlayerItemDrop: EventBase
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
}