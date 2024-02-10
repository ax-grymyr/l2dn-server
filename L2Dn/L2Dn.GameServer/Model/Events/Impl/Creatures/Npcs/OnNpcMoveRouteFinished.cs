using L2Dn.GameServer.Model.Actor;

namespace L2Dn.GameServer.Model.Events.Impl.Creatures.Npcs;

/**
 * @author UnAfraid
 */
public class OnNpcMoveRouteFinished: IBaseEvent
{
	private readonly Npc _npc;
	
	public OnNpcMoveRouteFinished(Npc npc)
	{
		_npc = npc;
	}
	
	public Npc getNpc()
	{
		return _npc;
	}
	
	public EventType getType()
	{
		return EventType.ON_NPC_MOVE_ROUTE_FINISHED;
	}
}