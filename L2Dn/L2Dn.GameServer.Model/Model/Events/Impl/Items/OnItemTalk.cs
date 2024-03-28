using L2Dn.Events;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Items.Instances;

namespace L2Dn.GameServer.Model.Events.Impl.Items;

/**
 * @author UnAfraid
 */
public class OnItemTalk: EventBase
{
	private readonly Item _item;
	private readonly Player _player;
	
	public OnItemTalk(Item item, Player player)
	{
		_item = item;
		_player = player;
	}
	
	public Item getItem()
	{
		return _item;
	}
	
	public Player getActiveChar()
	{
		return _player;
	}
}