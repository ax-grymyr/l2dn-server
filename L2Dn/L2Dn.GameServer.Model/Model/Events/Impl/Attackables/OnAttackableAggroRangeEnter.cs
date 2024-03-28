using L2Dn.Events;
using L2Dn.GameServer.Model.Actor;

namespace L2Dn.GameServer.Model.Events.Impl.Attackables;

/**
 * @author UnAfraid
 */
public class OnAttackableAggroRangeEnter: EventBase
{
	private readonly Attackable _npc;
	private readonly Player _player;
	private readonly bool _isSummon;
	
	public OnAttackableAggroRangeEnter(Attackable npc, Player attacker, bool isSummon)
	{
		_npc = npc;
		_player = attacker;
		_isSummon = isSummon;
	}
	
	public Attackable getNpc()
	{
		return _npc;
	}
	
	public Player getActiveChar()
	{
		return _player;
	}
	
	public bool isSummon()
	{
		return _isSummon;
	}
}