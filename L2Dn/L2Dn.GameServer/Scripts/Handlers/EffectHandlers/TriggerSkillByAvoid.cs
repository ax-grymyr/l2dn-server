using L2Dn.GameServer.Data.Xml;
using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Effects;
using L2Dn.GameServer.Model.Events;
using L2Dn.GameServer.Model.Events.Impl.Creatures;
using L2Dn.GameServer.Model.Events.Listeners;
using L2Dn.GameServer.Model.Holders;
using L2Dn.GameServer.Model.Items.Instances;
using L2Dn.GameServer.Model.Skills;
using L2Dn.GameServer.Model.Skills.Targets;
using L2Dn.GameServer.Utilities;

namespace L2Dn.GameServer.Handlers.EffectHandlers;

/**
 * Trigger Skill By Avoid effect implementation.
 * @author Zealar
 */
public class TriggerSkillByAvoid: AbstractEffect
{
	private readonly int _chance;
	private readonly SkillHolder _skill;
	private readonly TargetType _targetType;
	private readonly int _skillLevelScaleTo;
	
	/**
	 * @param @params
	 */
	public TriggerSkillByAvoid(StatSet @params)
	{
		_chance = @params.getInt("chance", 100);
		_skill = new SkillHolder(@params.getInt("skillId", 0), @params.getInt("skillLevel", 0));
		_targetType = @params.getEnum("targetType", TargetType.TARGET);
		_skillLevelScaleTo = @params.getInt("skillLevelScaleTo", 0);
	}
	
	private void onAvoidEvent(OnCreatureAttackAvoid @event)
	{
		if (@event.isDamageOverTime() || (_chance == 0) || ((_skill.getSkillId() == 0) || (_skill.getSkillLevel() == 0)))
		{
			return;
		}
		
		ITargetTypeHandler targetHandler = TargetHandler.getInstance().getHandler(_targetType);
		if (targetHandler == null)
		{
			LOGGER.Warn("Handler for target type: " + _targetType + " does not exist.");
			return;
		}
		
		if ((_chance < 100) && (Rnd.get(100) > _chance))
		{
			return;
		}
		
		WorldObject target = null;
		try
		{
			target = TargetHandler.getInstance().getHandler(_targetType).getTarget(@event.getTarget(), @event.getAttacker(), _skill.getSkill(), false, false, false);
		}
		catch (Exception e)
		{
			LOGGER.Warn("Exception in ITargetTypeHandler.getTarget(): " + e);
		}
		if ((target == null) || !target.isCreature())
		{
			return;
		}
		
		Skill triggerSkill;
		if (_skillLevelScaleTo <= 0)
		{
			triggerSkill = _skill.getSkill();
		}
		else
		{
			BuffInfo buffInfo = ((Creature) target).getEffectList().getBuffInfoBySkillId(_skill.getSkillId());
			if (buffInfo != null)
			{
				triggerSkill = SkillData.getInstance().getSkill(_skill.getSkillId(), Math.Min(_skillLevelScaleTo, buffInfo.getSkill().getLevel() + 1));
			}
			else
			{
				triggerSkill = _skill.getSkill();
			}
		}
		
		SkillCaster.triggerCast(@event.getAttacker(), (Creature) target, triggerSkill);
	}
	
	public override void onExit(Creature effector, Creature effected, Skill skill)
	{
		effected.removeListenerIf(EventType.ON_CREATURE_ATTACK_AVOID, listener => listener.getOwner() == this);
	}
	
	public override void onStart(Creature effector, Creature effected, Skill skill, Item item)
	{
		effected.addListener(new ConsumerEventListener(effected, EventType.ON_CREATURE_ATTACK_AVOID,
			@event => onAvoidEvent((OnCreatureAttackAvoid)@event), this));
	}
}