using L2Dn.GameServer.Model.Actor;

namespace L2Dn.GameServer.Model.Events.Impl.Creatures.Npcs;

/**
 * @author UnAfraid
 */
public class OnAttackableFactionCall: IBaseEvent
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
	
	public EventType getType()
	{
		return EventType.ON_ATTACKABLE_FACTION_CALL;
	}
}