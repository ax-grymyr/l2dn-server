using L2Dn.Events;
using L2Dn.GameServer.Model.Actor;

namespace L2Dn.GameServer.Model.Events.Impl.Npcs;

/**
 * @author UnAfraid
 */
public class OnNpcEventReceived: EventBase
{
	private readonly String _eventName;
	private readonly Npc _sender;
	private readonly Npc _receiver;
	private readonly WorldObject _reference;
	
	public OnNpcEventReceived(String eventName, Npc sender, Npc receiver, WorldObject reference)
	{
		_eventName = eventName;
		_sender = sender;
		_receiver = receiver;
		_reference = reference;
	}
	
	public String getEventName()
	{
		return _eventName;
	}
	
	public Npc getSender()
	{
		return _sender;
	}
	
	public Npc getReceiver()
	{
		return _receiver;
	}
	
	public WorldObject getReference()
	{
		return _reference;
	}
}