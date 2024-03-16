using L2Dn.GameServer.Model.Actor;

namespace L2Dn.GameServer.Model.Events.Impl.Creatures.Npcs;

/**
 * @author UnAfraid
 */
public class OnAttackableHate: IBaseEvent
{
	private readonly Attackable _npc;
	private readonly Player _player;
	private readonly bool _isSummon;
	
	public OnAttackableHate(Attackable npc, Player player, bool isSummon)
	{
		_npc = npc;
		_player = player;
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
	
	public EventType getType()
	{
		return EventType.ON_NPC_HATE;
	}
}