using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Skills;
using L2Dn.GameServer.Model.Stats;

namespace L2Dn.GameServer.Scripts.Handlers.EffectHandlers;

/**
 * @author Mobius
 */
public class ExpModify: AbstractStatAddEffect
{
	public ExpModify(StatSet @params): base(@params, Stat.BONUS_EXP)
	{
	}
	
	public override void pump(Creature effected, Skill skill)
	{
		effected.getStat().mergeAdd(Stat.BONUS_EXP, _amount);
		if ((skill != null) && skill.isActive())
		{
			effected.getStat().mergeAdd(Stat.ACTIVE_BONUS_EXP, _amount);
			effected.getStat().mergeAdd(Stat.BONUS_EXP_BUFFS, 1d);
		}
		else
		{
			effected.getStat().mergeAdd(Stat.BONUS_EXP_PASSIVES, 1d);
		}
		
		Player player = effected.getActingPlayer();
		if (player == null)
		{
			return;
		}
		
		player.sendUserBoostStat();
	}
}