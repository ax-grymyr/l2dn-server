using L2Dn.GameServer.Data.Xml;
using L2Dn.GameServer.Enums;
using L2Dn.GameServer.Handlers;
using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Effects;
using L2Dn.GameServer.Model.Events.Impl.Creatures;
using L2Dn.GameServer.Model.Holders;
using L2Dn.GameServer.Model.Items.Instances;
using L2Dn.GameServer.Model.Skills;
using L2Dn.GameServer.Model.Skills.Targets;
using L2Dn.GameServer.Utilities;
using L2Dn.Utilities;

namespace L2Dn.GameServer.Scripts.Handlers.EffectHandlers;

/**
 * Trigger Skill By Skill Attack effect implementation.
 * @author Nik
 */
public class TriggerSkillBySkillAttack: AbstractEffect
{
	private readonly int _minAttackerLevel;
	private readonly int _maxAttackerLevel;
	private readonly int _minDamage;
	private readonly int _chance;
	private readonly SkillHolder _attackSkill;
	private readonly SkillHolder _skill;
	private readonly int _skillLevelScaleTo;
	private readonly TargetType _targetType;
	private readonly InstanceType _attackerType;
	
	public TriggerSkillBySkillAttack(StatSet @params)
	{
		_minAttackerLevel = @params.getInt("minAttackerLevel", 1);
		_maxAttackerLevel = @params.getInt("maxAttackerLevel", int.MaxValue);
		_minDamage = @params.getInt("minDamage", 1);
		_chance = @params.getInt("chance", 100);
		_skill = new SkillHolder(@params.getInt("skillId"), @params.getInt("skillLevel", 1));
		_attackSkill = new SkillHolder(@params.getInt("attackSkillId"), @params.getInt("attackSkillLevel", 1));
		_skillLevelScaleTo = @params.getInt("skillLevelScaleTo", 0);
		_targetType = @params.getEnum("targetType", TargetType.TARGET);
		_attackerType = @params.getEnum("attackerType", InstanceType.Creature);
	}
	
	public override void onStart(Creature effector, Creature effected, Skill skill, Item item)
	{
		effected.Events.Subscribe<OnCreatureDamageDealt>(this, onAttackEvent);
	}
	
	public override void onExit(Creature effector, Creature effected, Skill skill)
	{
		effected.Events.Unsubscribe<OnCreatureDamageDealt>(onAttackEvent);
	}
	
	private void onAttackEvent(OnCreatureDamageDealt @event)
	{
		if (@event.isDamageOverTime() || (_chance == 0) || ((_skill.getSkillId() == 0) || (_skill.getSkillLevel() == 0)) || (_attackSkill.getSkillId() == 0))
		{
			return;
		}
		
		if (@event.getSkill() == null)
		{
			return;
		}
		
		if (@event.getSkill().getId() != _attackSkill.getSkillId())
		{
			return;
		}
		
		ITargetTypeHandler targetHandler = TargetHandler.getInstance().getHandler(_targetType);
		if (targetHandler == null)
		{
			LOGGER.Warn("Handler for target type: " + _targetType + " does not exist.");
			return;
		}
		
		if (@event.getAttacker() == @event.getTarget())
		{
			return;
		}
		
		if ((@event.getAttacker().getLevel() < _minAttackerLevel) || (@event.getAttacker().getLevel() > _maxAttackerLevel))
		{
			return;
		}
		
		if ((@event.getDamage() < _minDamage) || ((_chance < 100) && (Rnd.get(100) > _chance)) || !@event.getAttacker().getInstanceType().IsType(_attackerType))
		{
			return;
		}
		
		Skill triggerSkill = _skill.getSkill();
		WorldObject target = null;
		try
		{
			target = TargetHandler.getInstance().getHandler(_targetType).getTarget(@event.getAttacker(), @event.getTarget(), triggerSkill, false, false, false);
		}
		catch (Exception e)
		{
			LOGGER.Warn("Exception in ITargetTypeHandler.getTarget(): " + e);
		}
		
		if ((target != null) && target.isCreature())
		{
			if (_skillLevelScaleTo > 0)
			{
				BuffInfo buffInfo = ((Creature) target).getEffectList().getBuffInfoBySkillId(_skill.getSkillId());
				if (buffInfo != null)
				{
					triggerSkill = SkillData.getInstance().getSkill(_skill.getSkillId(), Math.Min(_skillLevelScaleTo, buffInfo.getSkill().getLevel() + 1));
				}
			}
			
			SkillCaster.triggerCast(@event.getAttacker(), (Creature) target, triggerSkill);
		}
	}
}