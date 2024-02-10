using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.ItemContainers;
using L2Dn.GameServer.Model.Items.Instances;

namespace L2Dn.GameServer.Model.Events.Impl.Creatures.Players;

/**
 * @author UnAfraid
 */
public class OnPlayerClanWHItemAdd: IBaseEvent
{
	private readonly String _process;
	private readonly Player _player;
	private readonly Item _item;
	private readonly ItemContainer _container;
	
	public OnPlayerClanWHItemAdd(String process, Player player, Item item, ItemContainer container)
	{
		_process = process;
		_player = player;
		_item = item;
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
	
	public ItemContainer getContainer()
	{
		return _container;
	}
	
	public EventType getType()
	{
		return EventType.ON_PLAYER_CLAN_WH_ITEM_ADD;
	}
}