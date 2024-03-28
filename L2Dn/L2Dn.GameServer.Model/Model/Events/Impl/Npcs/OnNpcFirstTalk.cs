using L2Dn.Events;
using L2Dn.GameServer.Model.Actor;

namespace L2Dn.GameServer.Model.Events.Impl.Npcs;

/**
 * @author UnAfraid
 */
public class OnNpcFirstTalk: EventBase
{
	private readonly Npc _npc;
	private readonly Player _player;
	
	public OnNpcFirstTalk(Npc npc, Player player)
	{
		_npc = npc;
		_player = player;
	}
	
	public Npc getNpc()
	{
		return _npc;
	}
	
	public Player getActiveChar()
	{
		return _player;
	}
}