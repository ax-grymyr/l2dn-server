using L2Dn.Events;
using L2Dn.GameServer.Model.Actor;

namespace L2Dn.GameServer.Model.Events.Impl.Npcs;

/**
 * @author UnAfraid
 */
public class OnNpcSpawn: EventBase
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
}