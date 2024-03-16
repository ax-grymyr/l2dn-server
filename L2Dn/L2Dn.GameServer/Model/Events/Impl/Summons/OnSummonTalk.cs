using L2Dn.GameServer.Model.Actor;

namespace L2Dn.GameServer.Model.Events.Impl.Players;

/**
 * @author St3eT
 */
public class OnSummonTalk: IBaseEvent
{
	private readonly Summon _summon;
	
	public OnSummonTalk(Summon summon)
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