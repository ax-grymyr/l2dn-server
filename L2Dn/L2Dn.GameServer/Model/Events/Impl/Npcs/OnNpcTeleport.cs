using L2Dn.Events;
using L2Dn.GameServer.Model.Actor;

namespace L2Dn.GameServer.Model.Events.Impl.Npcs;

/**
 * @author Zealar
 */
public class OnNpcTeleport: EventBase
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
}