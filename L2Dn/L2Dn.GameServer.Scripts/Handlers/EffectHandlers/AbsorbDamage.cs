using L2Dn.GameServer.Enums;
using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Effects;
using L2Dn.GameServer.Model.Events.Impl.Creatures;
using L2Dn.GameServer.Model.Items.Instances;
using L2Dn.GameServer.Model.Skills;
using L2Dn.GameServer.Utilities;

namespace L2Dn.GameServer.Scripts.Handlers.EffectHandlers;

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
	
	private void onDamageReceivedDiffEvent(OnCreatureDamageReceived ev, Creature effected, Skill skill)
	{
		// DOT effects are not taken into account.
		if (ev.isDamageOverTime())
		{
			return;
		}
		
		int objectId = ev.getTarget().getObjectId();
		
		double damageLeft = DIFF_DAMAGE_HOLDER.getOrDefault(objectId, 0d);
		double newDamageLeft = Math.Max(damageLeft - ev.getDamage(), 0);
		double newDamage = Math.Max(ev.getDamage() - damageLeft, 0);
		
		if (newDamageLeft > 0)
		{
			DIFF_DAMAGE_HOLDER.put(objectId, newDamageLeft);
		}
		else
		{
			effected.stopSkillEffects(skill);
		}

		ev.OverrideDamage = true;
		ev.OverridenDamage = newDamage;
	}
	
	private void onDamageReceivedPerEvent(OnCreatureDamageReceived ev)
	{
		// DOT effects are not taken into account.
		if (ev.isDamageOverTime())
		{
			return;
		}
		
		int objectId = ev.getTarget().getObjectId();
		
		double damagePercent = PER_DAMAGE_HOLDER.getOrDefault(objectId, 0d);
		double currentDamage = ev.getDamage();
		double newDamage = currentDamage - ((currentDamage / 100) * damagePercent);

		ev.OverrideDamage = true;
		ev.OverridenDamage = newDamage;
	}
	
	public override void onExit(Creature effector, Creature effected, Skill skill)
	{
		if (_mode == StatModifierType.DIFF)
		{
			effected.Events.UnsubscribeAll<OnCreatureDamageReceived>(this);
			DIFF_DAMAGE_HOLDER.remove(effected.getObjectId());
		}
		else
		{
			effected.Events.Unsubscribe<OnCreatureDamageReceived>(onDamageReceivedPerEvent);
			PER_DAMAGE_HOLDER.remove(effected.getObjectId());
		}
	}
	
	public override void onStart(Creature effector, Creature effected, Skill skill, Item item)
	{
		if (_mode == StatModifierType.DIFF)
		{
			DIFF_DAMAGE_HOLDER.put(effected.getObjectId(), _damage);
			effected.Events.Subscribe<OnCreatureDamageReceived>(this, ev => onDamageReceivedDiffEvent(ev, effected, skill));
		}
		else
		{
			PER_DAMAGE_HOLDER.put(effected.getObjectId(), _damage);
			effected.Events.Subscribe<OnCreatureDamageReceived>(this, onDamageReceivedPerEvent);
		}
	}
}
