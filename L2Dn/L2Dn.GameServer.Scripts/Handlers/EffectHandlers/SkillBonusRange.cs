using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Stats;
using L2Dn.Model.Enums;

namespace L2Dn.GameServer.Scripts.Handlers.EffectHandlers;

/**
 * @author NasSeKa
 */
public class SkillBonusRange: AbstractStatAddEffect
{
	public SkillBonusRange(StatSet @params): base(@params, Stat.MAGIC_ATTACK_RANGE)
	{
	}
}