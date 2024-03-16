using L2Dn.GameServer.Model.Actor;

namespace L2Dn.GameServer.Model.Events.Impl.Creatures.Npcs;

/**
 * @author UnAfraid
 */
public class OnNpcMoveNodeArrived: IBaseEvent
{
	private readonly Npc _npc;
	
	public OnNpcMoveNodeArrived(Npc npc)
	{
		_npc = npc;
	}
	
	public Npc getNpc()
	{
		return _npc;
	}
	
	public EventType getType()
	{
		return EventType.ON_NPC_MOVE_NODE_ARRIVED;
	}
}