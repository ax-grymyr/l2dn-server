using L2Dn.GameServer.Enums;
using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Effects;
using L2Dn.GameServer.Model.Items.Instances;
using L2Dn.GameServer.Model.Skills;
using L2Dn.GameServer.Model.Stats;

namespace L2Dn.GameServer.Scripts.Handlers.EffectHandlers;

/**
 * Dispel By Category effect implementation.
 * @author DS, Adry_85
 */
public class DispelByCategory: AbstractEffect
{
	private readonly DispelSlotType _slot;
	private readonly int _rate;
	private readonly int _max;

	public DispelByCategory(StatSet @params)
	{
		_slot = @params.getEnum("slot", DispelSlotType.BUFF);
		_rate = @params.getInt("rate", 0);
		_max = @params.getInt("max", 0);
	}

	public override EffectType getEffectType()
	{
		return EffectType.DISPEL;
	}

	public override bool isInstant()
	{
		return true;
	}

	public override void instant(Creature effector, Creature effected, Skill skill, Item? item)
	{
		if (effected.isDead())
		{
			return;
		}

		List<BuffInfo> canceled = Formulas.calcCancelStealEffects(effector, effected, skill, _slot, _rate, _max);
		foreach (BuffInfo can in canceled)
		{
			effected.getEffectList().stopSkillEffects(SkillFinishType.REMOVED, can.getSkill());
		}
	}
}