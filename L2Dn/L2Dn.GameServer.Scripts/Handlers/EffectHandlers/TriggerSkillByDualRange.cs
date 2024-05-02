using L2Dn.GameServer.Data.Xml;
using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Effects;
using L2Dn.GameServer.Model.Holders;
using L2Dn.GameServer.Model.Items.Instances;
using L2Dn.GameServer.Model.Skills;

namespace L2Dn.GameServer.Scripts.Handlers.EffectHandlers;

/**
 * Trigger Skill By Dual Range effect implementation.
 * @author Mobius
 */
public class TriggerSkillByDualRange: AbstractEffect
{
	private readonly SkillHolder _closeSkill;
	private readonly SkillHolder _rangeSkill;
	private readonly int _distance;
	private readonly bool _adjustLevel;
	
	public TriggerSkillByDualRange(StatSet @params)
	{
		// Just use closeSkill and rangeSkill parameters.
		_closeSkill = new SkillHolder(@params.getInt("closeSkill"), @params.getInt("closeSkillLevel", 1));
		_rangeSkill = new SkillHolder(@params.getInt("rangeSkill"), @params.getInt("rangeSkillLevel", 1));
		_distance = @params.getInt("distance", 120);
		_adjustLevel = @params.getBoolean("adjustLevel", true);
	}
	
	public override bool isInstant()
	{
		return true;
	}
	
	public override EffectType getEffectType()
	{
		return EffectType.DUAL_RANGE;
	}
	
	public override void instant(Creature effector, Creature effected, Skill skill, Item item)
	{
		if ((effected == null) || !effector.isPlayer())
		{
			return;
		}
		
		SkillHolder skillHolder = effector.calculateDistance3D(effected.getLocation().ToLocation3D()) < _distance ? _closeSkill : _rangeSkill;
		Skill triggerSkill = _adjustLevel ? SkillData.getInstance().getSkill(skillHolder.getSkillId(), skill.getLevel()) : skillHolder.getSkill();
		if (triggerSkill == null)
		{
			return;
		}
		
		if (effected.isPlayable() && !effected.isAutoAttackable(effector))
		{
			effector.getActingPlayer().updatePvPStatus();
		}
		
		effector.getActingPlayer().useMagic(triggerSkill, null, true, triggerSkill.getCastRange() > 600);
	}
}