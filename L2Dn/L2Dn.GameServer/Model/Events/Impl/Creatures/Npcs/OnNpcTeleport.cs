using L2Dn.GameServer.Model.Actor;

namespace L2Dn.GameServer.Model.Events.Impl.Creatures.Npcs;

/**
 * @author Zealar
 */
public class OnNpcTeleport: IBaseEvent
{
	private readonly Npc _npc;
	
	public OnNpcTeleport(Npc npc)
	{
		_npc = npc;
	}
	
	public Npc getNpc()
	{
		return _npc;
	}
	
	public EventType getType()
	{
		return EventType.ON_NPC_TELEPORT;
	}
}