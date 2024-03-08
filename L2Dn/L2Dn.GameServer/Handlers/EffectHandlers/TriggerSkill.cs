using L2Dn.GameServer.Data.Xml;
using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Effects;
using L2Dn.GameServer.Model.Holders;
using L2Dn.GameServer.Model.Items.Instances;
using L2Dn.GameServer.Model.Skills;
using L2Dn.GameServer.Model.Skills.Targets;
using ThreadPool = L2Dn.GameServer.Utilities.ThreadPool;

namespace L2Dn.GameServer.Handlers.EffectHandlers;

/**
 * Trigger Skill effect implementation.
 * @author Mobius
 */
public class TriggerSkill: AbstractEffect
{
	private readonly SkillHolder _skill;
	private readonly TargetType _targetType;
	private readonly bool _adjustLevel;
	
	public TriggerSkill(StatSet @params)
	{
		_skill = new SkillHolder(@params.getInt("skillId"), @params.getInt("skillLevel", 1));
		_targetType = @params.getEnum<TargetType>("targetType", TargetType.TARGET);
		_adjustLevel = @params.getBoolean("adjustLevel", false);
	}
	
	public override bool isInstant()
	{
		return true;
	}
	
	public override void instant(Creature effector, Creature effected, Skill skill, Item item)
	{
		if ((effected == null) || !effected.isCreature() || !effector.isPlayer())
		{
			return;
		}
		
		Skill triggerSkill = _adjustLevel ? SkillData.getInstance().getSkill(_skill.getSkillId(), skill.getLevel()) : _skill.getSkill();
		if (triggerSkill == null)
		{
			return;
		}
		
		WorldObject target = null;
		try
		{
			target = TargetHandler.getInstance().getHandler(_targetType).getTarget(effector, effected, triggerSkill, false, false, false);
		}
		catch (Exception e)
		{
			LOGGER.Warn("Exception in ITargetTypeHandler.getTarget(): " + e);
		}
		
		if ((target == null) || !target.isCreature())
		{
			return;
		}
		
		SkillUseHolder queuedSkill = effector.getActingPlayer().getQueuedSkill();
		if (queuedSkill != null)
		{
			ThreadPool.schedule(() =>
			{
				effector.getActingPlayer().setQueuedSkill(queuedSkill.getSkill(), queuedSkill.getItem(), queuedSkill.isCtrlPressed(), queuedSkill.isShiftPressed());
			}, 10);
		}
		
		effector.getActingPlayer().setQueuedSkill(triggerSkill, null, false, false);
	}
}