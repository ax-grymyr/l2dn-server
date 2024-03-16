using L2Dn.Events;
using L2Dn.GameServer.Model.Actor;

namespace L2Dn.GameServer.Model.Events.Impl.Npcs;

/**
 * @author malyelfik
 */
public class OnNpcDespawn: EventBase
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
}