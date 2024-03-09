using L2Dn.GameServer.Enums;
using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Effects;
using L2Dn.GameServer.Model.Events;
using L2Dn.GameServer.Model.Events.Impl.Creatures;
using L2Dn.GameServer.Model.Events.Listeners;
using L2Dn.GameServer.Model.Events.Returns;
using L2Dn.GameServer.Model.Items.Instances;
using L2Dn.GameServer.Model.Skills;
using L2Dn.GameServer.Utilities;

namespace L2Dn.GameServer.Handlers.EffectHandlers;

/**
 * @author Sdw, Mobius
 */
public class AbsorbDamage: AbstractEffect
{
	private static readonly Map<int, Double> DIFF_DAMAGE_HOLDER = new();
	private static readonly Map<int, Double> PER_DAMAGE_HOLDER = new();
	
	private readonly double _damage;
	private readonly StatModifierType _mode;
	
	public AbsorbDamage(StatSet @params)
	{
		_damage = @params.getDouble("damage", 0);
		_mode = @params.getEnum("mode", StatModifierType.DIFF);
	}
	
	private DamageReturn onDamageReceivedDiffEvent(OnCreatureDamageReceived @event, Creature effected, Skill skill)
	{
		// DOT effects are not taken into account.
		if (@event.isDamageOverTime())
		{
			return null;
		}
		
		int objectId = @event.getTarget().getObjectId();
		
		double damageLeft = DIFF_DAMAGE_HOLDER.getOrDefault(objectId, 0d);
		double newDamageLeft = Math.Max(damageLeft - @event.getDamage(), 0);
		double newDamage = Math.Max(@event.getDamage() - damageLeft, 0);
		
		if (newDamageLeft > 0)
		{
			DIFF_DAMAGE_HOLDER.put(objectId, newDamageLeft);
		}
		else
		{
			effected.stopSkillEffects(skill);
		}
		
		return new DamageReturn(false, true, false, newDamage);
	}
	
	private DamageReturn onDamageReceivedPerEvent(OnCreatureDamageReceived @event)
	{
		// DOT effects are not taken into account.
		if (@event.isDamageOverTime())
		{
			return null;
		}
		
		int objectId = @event.getTarget().getObjectId();
		
		double damagePercent = PER_DAMAGE_HOLDER.getOrDefault(objectId, 0d);
		double currentDamage = @event.getDamage();
		double newDamage = currentDamage - ((currentDamage / 100) * damagePercent);
		
		return new DamageReturn(false, true, false, newDamage);
	}
	
	public override void onExit(Creature effector, Creature effected, Skill skill)
	{
		effected.removeListenerIf(EventType.ON_CREATURE_DAMAGE_RECEIVED, listener => listener.getOwner() == this);
		if (_mode == StatModifierType.DIFF)
		{
			DIFF_DAMAGE_HOLDER.remove(effected.getObjectId());
		}
		else
		{
			PER_DAMAGE_HOLDER.remove(effected.getObjectId());
		}
	}
	
	public override void onStart(Creature effector, Creature effected, Skill skill, Item item)
	{
		if (_mode == StatModifierType.DIFF)
		{
			DIFF_DAMAGE_HOLDER.put(effected.getObjectId(), _damage);
			effected.addListener(new FunctionEventListener(effected, EventType.ON_CREATURE_DAMAGE_RECEIVED,
				@event => onDamageReceivedDiffEvent((OnCreatureDamageReceived)@event, effected, skill), this));
		}
		else
		{
			PER_DAMAGE_HOLDER.put(effected.getObjectId(), _damage);
			effected.addListener(new FunctionEventListener(effected, EventType.ON_CREATURE_DAMAGE_RECEIVED,
				@event => onDamageReceivedPerEvent((OnCreatureDamageReceived)@event), this));
		}
	}
}
