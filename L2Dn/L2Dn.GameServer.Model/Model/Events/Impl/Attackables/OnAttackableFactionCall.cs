using L2Dn.Events;
using L2Dn.GameServer.Model.Actor;

namespace L2Dn.GameServer.Model.Events.Impl.Attackables;

/**
 * @author UnAfraid
 */
public class OnAttackableFactionCall: EventBase
{
	private readonly Npc _npc;
	private readonly Npc _caller;
	private readonly Player _attacker;
	private readonly bool _isSummon;
	
	public OnAttackableFactionCall(Npc npc, Npc caller, Player attacker, bool isSummon)
	{
		_npc = npc;
		_caller = caller;
		_attacker = attacker;
		_isSummon = isSummon;
	}
	
	public Npc getNpc()
	{
		return _npc;
	}
	
	public Npc getCaller()
	{
		return _caller;
	}
	
	public Player getAttacker()
	{
		return _attacker;
	}
	
	public bool isSummon()
	{
		return _isSummon;
	}
}