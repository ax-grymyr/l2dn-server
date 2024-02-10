using L2Dn.GameServer.Model.Actor;

namespace L2Dn.GameServer.Model.Events.Impl.Creatures.Npcs;

/**
 * @author UnAfraid
 */
public class OnAttackableAggroRangeEnter: IBaseEvent
{
	private readonly Npc _npc;
	private readonly Player _player;
	private readonly bool _isSummon;
	
	public OnAttackableAggroRangeEnter(Npc npc, Player attacker, bool isSummon)
	{
		_npc = npc;
		_player = attacker;
		_isSummon = isSummon;
	}
	
	public Npc getNpc()
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
	
	public EventType getType()
	{
		return EventType.ON_ATTACKABLE_AGGRO_RANGE_ENTER;
	}
}