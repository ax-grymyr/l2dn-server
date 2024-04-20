using L2Dn.GameServer.Enums;
using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Effects;
using L2Dn.GameServer.Model.Skills;
using L2Dn.GameServer.Model.Stats;
using L2Dn.Model.Enums;

namespace L2Dn.GameServer.Scripts.Handlers.EffectHandlers;

/**
 * @author Sdw
 */
public class ReduceDropPenalty: AbstractEffect
{
	private readonly double _exp;
	private readonly double _deathPenalty;
	private readonly ReduceDropType _type;
	
	public ReduceDropPenalty(StatSet @params)
	{
		_exp = @params.getDouble("exp", 0);
		_deathPenalty = @params.getDouble("deathPenalty", 0);
		_type = @params.getEnum("type", ReduceDropType.MOB);
	}
	
	public override void pump(Creature effected, Skill skill)
	{
		switch (_type)
		{
			case ReduceDropType.MOB:
			{
				effected.getStat().mergeMul(Stat.REDUCE_EXP_LOST_BY_MOB, (_exp / 100) + 1);
				effected.getStat().mergeMul(Stat.REDUCE_DEATH_PENALTY_BY_MOB, (_deathPenalty / 100) + 1);
				break;
			}
			case ReduceDropType.PK:
			{
				effected.getStat().mergeMul(Stat.REDUCE_EXP_LOST_BY_PVP, (_exp / 100) + 1);
				effected.getStat().mergeMul(Stat.REDUCE_DEATH_PENALTY_BY_PVP, (_deathPenalty / 100) + 1);
				break;
			}
			case ReduceDropType.RAID:
			{
				effected.getStat().mergeMul(Stat.REDUCE_EXP_LOST_BY_RAID, (_exp / 100) + 1);
				effected.getStat().mergeMul(Stat.REDUCE_DEATH_PENALTY_BY_RAID, (_deathPenalty / 100) + 1);
				break;
			}
			case ReduceDropType.ANY:
			{
				effected.getStat().mergeMul(Stat.REDUCE_EXP_LOST_BY_MOB, (_exp / 100) + 1);
				effected.getStat().mergeMul(Stat.REDUCE_DEATH_PENALTY_BY_MOB, (_deathPenalty / 100) + 1);
				effected.getStat().mergeMul(Stat.REDUCE_EXP_LOST_BY_PVP, (_exp / 100) + 1);
				effected.getStat().mergeMul(Stat.REDUCE_DEATH_PENALTY_BY_PVP, (_deathPenalty / 100) + 1);
				effected.getStat().mergeMul(Stat.REDUCE_EXP_LOST_BY_RAID, (_exp / 100) + 1);
				effected.getStat().mergeMul(Stat.REDUCE_DEATH_PENALTY_BY_RAID, (_deathPenalty / 100) + 1);
				break;
			}
		}
	}
}