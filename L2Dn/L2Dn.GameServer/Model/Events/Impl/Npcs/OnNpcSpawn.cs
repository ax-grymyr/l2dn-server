using L2Dn.GameServer.Model.Actor;

namespace L2Dn.GameServer.Model.Events.Impl.Creatures.Npcs;

/**
 * @author UnAfraid
 */
public class OnNpcSpawn: IBaseEvent
{
	private readonly Npc _npc;
	
	public OnNpcSpawn(Npc npc)
	{
		_npc = npc;
	}
	
	public Npc getNpc()
	{
		return _npc;
	}
	
	public EventType getType()
	{
		return EventType.ON_NPC_SPAWN;
	}
}