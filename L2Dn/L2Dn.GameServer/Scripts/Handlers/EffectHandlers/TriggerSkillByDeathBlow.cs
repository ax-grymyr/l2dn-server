using L2Dn.GameServer.Enums;
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
 * Trigger Skill By Death Blow effect implementation.
 * @author Sdw
 */
public class TriggerSkillByDeathBlow: AbstractEffect
{
	private readonly int _minAttackerLevel;
	private readonly int _maxAttackerLevel;
	private readonly int _chance;
	private readonly SkillHolder _skill;
	private readonly TargetType _targetType;
	private readonly InstanceType _attackerType;
	
	public TriggerSkillByDeathBlow(StatSet @params)
	{
		_minAttackerLevel = @params.getInt("minAttackerLevel", 1);
		_maxAttackerLevel = @params.getInt("maxAttackerLevel", int.MaxValue);
		_chance = @params.getInt("chance", 100);
		_skill = new SkillHolder(@params.getInt("skillId"), @params.getInt("skillLevel", 1));
		_targetType = @params.getEnum("targetType", TargetType.SELF);
		_attackerType = @params.getEnum("attackerType", InstanceType.Creature);
	}
	
	private void onDamageReceivedEvent(OnCreatureDamageReceived @event)
	{
		if (@event.getDamage() < @event.getTarget().getCurrentHp())
		{
			return;
		}
		
		if ((_chance == 0) || (_skill.getSkillLevel() == 0))
		{
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
		
		if (((_chance < 100) && (Rnd.get(100) > _chance)) || !@event.getAttacker().getInstanceType().IsType(_attackerType))
		{
			return;
		}
		
		Skill triggerSkill = _skill.getSkill();
		WorldObject target = null;
		try
		{
			target = TargetHandler.getInstance().getHandler(_targetType).getTarget(@event.getTarget(), @event.getAttacker(), triggerSkill, false, false, false);
		}
		catch (Exception e)
		{
			LOGGER.Warn("Exception in ITargetTypeHandler.getTarget(): " + e);
		}
		
		if ((target != null) && target.isCreature())
		{
			SkillCaster.triggerCast(@event.getTarget(), (Creature) target, triggerSkill);
		}
	}
	
	public override void onExit(Creature effector, Creature effected, Skill skill)
	{
		effected.removeListenerIf(EventType.ON_CREATURE_DAMAGE_RECEIVED, listener => listener.getOwner() == this);
	}
	
	public override void onStart(Creature effector, Creature effected, Skill skill, Item item)
	{
		effected.addListener(new ConsumerEventListener(effected, EventType.ON_CREATURE_DAMAGE_RECEIVED,
			@event => onDamageReceivedEvent((OnCreatureDamageReceived)@event), this));
	}
}