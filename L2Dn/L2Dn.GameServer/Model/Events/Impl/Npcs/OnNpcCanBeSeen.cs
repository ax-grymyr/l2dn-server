using L2Dn.Events;
using L2Dn.GameServer.Model.Actor;

namespace L2Dn.GameServer.Model.Events.Impl.Npcs;

/**
 * @author UnAfraid
 */
public class OnNpcCanBeSeen: EventBase
{
	private readonly Npc _npc;
	private readonly Player _player;
	
	public OnNpcCanBeSeen(Npc npc, Player player)
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
	
	public bool Visible { get; set; }
}