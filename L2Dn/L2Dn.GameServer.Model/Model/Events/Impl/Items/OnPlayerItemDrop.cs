using L2Dn.Events;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Items.Instances;
using L2Dn.Geometry;

namespace L2Dn.GameServer.Model.Events.Impl.Items;

/**
 * @author UnAfraid
 */
public class OnPlayerItemDrop: EventBase
{
	private readonly Player _player;
	private readonly Item _item;
	private readonly Location3D _loc;

	public OnPlayerItemDrop(Player player, Item item, Location3D location)
	{
		_player = player;
		_item = item;
		_loc = location;
	}

	public Player getPlayer()
	{
		return _player;
	}

	public Item getItem()
	{
		return _item;
	}

	public Location3D getLocation()
	{
		return _loc;
	}
}