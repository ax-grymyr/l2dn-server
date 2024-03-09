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
using L2Dn.GameServer.Model.Stats;

namespace L2Dn.GameServer.Scripts.Handlers.EffectHandlers;

/**
 * @author Sdw
 */
public class ReduceDamage: AbstractEffect
{
	private readonly double _amount;
	private readonly StatModifierType _mode;
	
	public ReduceDamage(StatSet @params)
	{
		_amount = @params.getDouble("amount");
		_mode = @params.getEnum("mode", StatModifierType.DIFF);
	}
	
	private DamageReturn onDamageReceivedEvent(OnCreatureDamageReceived @event)
	{
		// DOT effects are not taken into account.
		if (@event.isDamageOverTime())
		{
			return null;
		}
		
		double newDamage;
		if (_mode == StatModifierType.PER)
		{
			newDamage = @event.getDamage() - (@event.getDamage() * (_amount / 100));
		}
		else // DIFF
		{
			newDamage = @event.getDamage() - Math.Max((_amount - @event.getAttacker().getStat().getAdd(Stat.IGNORE_REDUCE_DAMAGE)), 0.0);
		}
		
		return new DamageReturn(false, true, false, newDamage);
	}
	
	public override void onExit(Creature effector, Creature effected, Skill skill)
	{
		effected.removeListenerIf(EventType.ON_CREATURE_DAMAGE_RECEIVED, listener => listener.getOwner() == this);
	}
	
	public override void onStart(Creature effector, Creature effected, Skill skill, Item item)
	{
		effected.addListener(new FunctionEventListener(effected, EventType.ON_CREATURE_DAMAGE_RECEIVED,
			@event => onDamageReceivedEvent((OnCreatureDamageReceived)@event), this));
	}
}