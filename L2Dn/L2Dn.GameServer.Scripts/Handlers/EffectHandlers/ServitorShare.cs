using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Effects;
using L2Dn.GameServer.Model.Skills;
using L2Dn.GameServer.Model.Stats;
using L2Dn.GameServer.Utilities;

namespace L2Dn.GameServer.Scripts.Handlers.EffectHandlers;

/**
 * Servitor Share effect implementation.
 */
public class ServitorShare: AbstractEffect
{
	private readonly Map<Stat, float> _sharedStats = new();
	
	public ServitorShare(StatSet @params)
	{
		if (@params.isEmpty())
		{
			return;
		}
		
		foreach (var param in @params.getSet())
		{
			_sharedStats.put(Enum.Parse<Stat>(param.Key), (float.Parse(param.Value.ToString() ?? string.Empty)) / 100);
		}
	}
	
	public override bool canPump(Creature effector, Creature effected, Skill skill)
	{
		return effected.isSummon();
	}
	
	public override void pump(Creature effected, Skill skill)
	{
		Player owner = effected.getActingPlayer();
		if (owner != null)
		{
			foreach (var stats in _sharedStats)
			{
				effected.getStat().mergeAdd(stats.Key, owner.getStat().getValue(stats.Key) * stats.Value);
			}
		}
	}
}