using L2Dn.Events;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.ItemContainers;
using L2Dn.GameServer.Model.Items.Instances;

namespace L2Dn.GameServer.Model.Events.Impl.Items;

/**
 * @author UnAfraid
 */
public class OnPlayerItemTransfer: EventBase
{
	private readonly Player? _player;
	private readonly Item _item;
	private readonly ItemContainer _container;

	public OnPlayerItemTransfer(Player? player, Item item, ItemContainer container)
	{
		_player = player;
		_item = item;
		_container = container;
	}

	public Player? getPlayer()
	{
		return _player;
	}

	public Item getItem()
	{
		return _item;
	}

	public ItemContainer getContainer()
	{
		return _container;
	}
}