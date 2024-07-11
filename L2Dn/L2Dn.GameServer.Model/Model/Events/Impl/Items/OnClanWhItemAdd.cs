using L2Dn.Events;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.ItemContainers;
using L2Dn.GameServer.Model.Items.Instances;

namespace L2Dn.GameServer.Model.Events.Impl.Items;

/**
 * @author UnAfraid
 */
public class OnClanWhItemAdd: EventBase
{
	private readonly string _process;
	private readonly Player _player;
	private readonly Item _item;
	private readonly ItemContainer _container;
	
	public OnClanWhItemAdd(string process, Player player, Item item, ItemContainer container)
	{
		_process = process;
		_player = player;
		_item = item;
		_container = container;
	}
	
	public string getProcess()
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
	
	public ItemContainer getContainer()
	{
		return _container;
	}
}