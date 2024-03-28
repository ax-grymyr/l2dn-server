using L2Dn.Events;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Items.Instances;

namespace L2Dn.GameServer.Model.Events.Impl.Items;

/**
 * @author UnAfraid
 */
public class OnItemCreate: EventBase
{
	private readonly String _process;
	private readonly Item _item;
	private readonly Creature _creature;
	private readonly Object _reference;
	
	public OnItemCreate(String process, Item item, Creature actor, Object reference)
	{
		_process = process;
		_item = item;
		_creature = actor;
		_reference = reference;
	}
	
	public String getProcess()
	{
		return _process;
	}
	
	public Item getItem()
	{
		return _item;
	}
	
	public Creature getActiveChar()
	{
		return _creature;
	}
	
	public Object getReference()
	{
		return _reference;
	}
}