using L2Dn.Events;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Items.Instances;

namespace L2Dn.GameServer.Model.Events.Impl.Items;

/**
 * @author UnAfraid
 */
public class OnPlayerItemDestroy: EventBase
{
	private readonly Player? _player;
	private readonly Item _item;

	public OnPlayerItemDestroy(Player? player, Item item)
	{
		_player = player;
		_item = item;
	}

	public Player? getPlayer()
	{
		return _player;
	}

	public Item getItem()
	{
		return _item;
	}
}