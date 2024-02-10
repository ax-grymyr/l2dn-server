using L2Dn.GameServer.Model.Actor;

namespace L2Dn.GameServer.Model.Events.Impl.Creatures.Players;

/**
 * @author UnAfraid
 */
public class OnPlayerSummonSpawn: IBaseEvent
{
	private readonly Summon _summon;
	
	public OnPlayerSummonSpawn(Summon summon)
	{
		_summon = summon;
	}
	
	public Summon getSummon()
	{
		return _summon;
	}
	
	public EventType getType()
	{
		return EventType.ON_PLAYER_SUMMON_SPAWN;
	}
}