using L2Dn.GameServer.Model.Actor;

namespace L2Dn.GameServer.Model.Events.Impl.Creatures.Npcs;

/**
 * @author malyelfik
 */
public class OnNpcDespawn: IBaseEvent
{
	private readonly Npc _npc;
	
	public OnNpcDespawn(Npc npc)
	{
		_npc = npc;
	}
	
	public Npc getNpc()
	{
		return _npc;
	}
	
	public EventType getType()
	{
		return EventType.ON_NPC_DESPAWN;
	}
}