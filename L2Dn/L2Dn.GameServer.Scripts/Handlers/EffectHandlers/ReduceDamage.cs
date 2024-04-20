using L2Dn.GameServer.Enums;
using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Effects;
using L2Dn.GameServer.Model.Events.Impl.Creatures;
using L2Dn.GameServer.Model.Items.Instances;
using L2Dn.GameServer.Model.Skills;
using L2Dn.GameServer.Model.Stats;
using L2Dn.Model.Enums;

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
	
	private void onDamageReceivedEvent(OnCreatureDamageReceived ev)
	{
		// DOT effects are not taken into account.
		if (ev.isDamageOverTime())
		{
			return;
		}
		
		double newDamage;
		if (_mode == StatModifierType.PER)
		{
			newDamage = ev.getDamage() - (ev.getDamage() * (_amount / 100));
		}
		else // DIFF
		{
			newDamage = ev.getDamage() - Math.Max((_amount - ev.getAttacker().getStat().getAddValue(Stat.IGNORE_REDUCE_DAMAGE)), 0.0);
		}

		ev.OverrideDamage = true;
		ev.OverridenDamage = newDamage;
	}
	
	public override void onExit(Creature effector, Creature effected, Skill skill)
	{
		effected.Events.Unsubscribe<OnCreatureDamageReceived>(onDamageReceivedEvent);
	}
	
	public override void onStart(Creature effector, Creature effected, Skill skill, Item item)
	{
		effected.Events.Subscribe<OnCreatureDamageReceived>(this, onDamageReceivedEvent);
	}
}