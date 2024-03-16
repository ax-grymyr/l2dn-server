using L2Dn.Events;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.ItemContainers;
using L2Dn.GameServer.Model.Items.Instances;

namespace L2Dn.GameServer.Model.Events.Impl.Items;

/**
 * @author UnAfraid
 */
public class OnClanWhItemTransfer: EventBase
{
	private readonly String _process;
	private readonly Player _player;
	private readonly Item _item;
	private readonly long _count;
	private readonly ItemContainer _container;
	
	public OnClanWhItemTransfer(String process, Player player, Item item, long count, ItemContainer container)
	{
		_process = process;
		_player = player;
		_item = item;
		_count = count;
		_container = container;
	}
	
	public String getProcess()
	{
		return _process;
	}
	
	public Player getPlayer()
	{
		return _player;
	}
	
	public Item getItem()
	{
		return _item;
	}
	
	public long getCount()
	{
		return _count;
	}
	
	public ItemContainer getContainer()
	{
		return _container;
	}
}