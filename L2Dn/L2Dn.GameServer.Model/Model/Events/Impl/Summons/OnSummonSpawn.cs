using L2Dn.Events;
using L2Dn.GameServer.Model.Actor;

namespace L2Dn.GameServer.Model.Events.Impl.Summons;

/**
 * @author UnAfraid
 */
public class OnSummonSpawn: EventBase
{
	private readonly Summon _summon;
	
	public OnSummonSpawn(Summon summon)
	{
		_summon = summon;
	}
	
	public Summon getSummon()
	{
		return _summon;
	}
}