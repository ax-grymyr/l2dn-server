using L2Dn.GameServer.Model.Actor;

namespace L2Dn.GameServer.Model.Events.Impl.Creatures.Npcs;

/**
 * @author UnAfraid
 */
public class OnNpcEventReceived: IBaseEvent
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
	
	public EventType getType()
	{
		return EventType.ON_NPC_EVENT_RECEIVED;
	}
}