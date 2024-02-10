using L2Dn.GameServer.Model.Actor;

namespace L2Dn.GameServer.Model.Events.Impl.Creatures.Players;

/**
 * @author St3eT
 */
public class OnPlayerSummonTalk: IBaseEvent
{
	private readonly Summon _summon;
	
	public OnPlayerSummonTalk(Summon summon)
	{
		_summon = summon;
	}
	
	public Summon getSummon()
	{
		return _summon;
	}
	
	public EventType getType()
	{
		return EventType.ON_PLAYER_SUMMON_TALK;
	}
}