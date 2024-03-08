using L2Dn.GameServer.Enums;
using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Effects;
using L2Dn.GameServer.Model.Items.Instances;
using L2Dn.GameServer.Model.Skills;
using L2Dn.GameServer.Model.Stats;
using L2Dn.GameServer.Utilities;

namespace L2Dn.GameServer.Handlers.EffectHandlers;

/**
 * Steal Abnormal effect implementation.
 * @author Adry_85, Zoey76
 */
public class StealAbnormal: AbstractEffect
{
	private readonly DispelSlotType _slot;
	private readonly int _rate;
	private readonly int _max;
	
	public StealAbnormal(StatSet @params)
	{
		_slot = @params.getEnum("slot", DispelSlotType.BUFF);
		_rate = @params.getInt("rate", 0);
		_max = @params.getInt("max", 0);
	}
	
	public override EffectType getEffectType()
	{
		return EffectType.STEAL_ABNORMAL;
	}
	
	public override bool isInstant()
	{
		return true;
	}
	
	public override void instant(Creature effector, Creature effected, Skill skill, Item item)
	{
		if (effected.isPlayer() && (effector != effected))
		{
			List<BuffInfo> toSteal = Formulas.calcCancelStealEffects(effector, effected, skill, _slot, _rate, _max);
			if (toSteal.isEmpty())
			{
				return;
			}
			
			foreach (BuffInfo infoToSteal in toSteal)
			{
				// Invert effected and effector.
				BuffInfo stolen = new BuffInfo(effected, effector, infoToSteal.getSkill(), false, null, null);
				stolen.setAbnormalTime(infoToSteal.getTime()); // Copy the remaining time.
				// To include all the effects, it's required to go through the template rather the buff info.
				infoToSteal.getSkill().applyEffectScope(EffectScope.GENERAL, stolen, true, true);
				effected.getEffectList().remove(infoToSteal, SkillFinishType.REMOVED, true, true);
				effector.getEffectList().add(stolen);
			}
		}
	}
}