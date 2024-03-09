using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor;
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
public class SphericBarrier: AbstractStatAddEffect
{
	public SphericBarrier(StatSet @params): base(@params, Stat.SPHERIC_BARRIER_RANGE)
	{
	}
	
	public override void onStart(Creature effector, Creature effected, Skill skill, Item item)
	{
		effected.addListener(new FunctionEventListener(effected, EventType.ON_CREATURE_DAMAGE_RECEIVED,
			@event => onDamageReceivedEvent((OnCreatureDamageReceived)@event), this));
	}
	
	public override void onExit(Creature effector, Creature effected, Skill skill)
	{
		effected.removeListenerIf(EventType.ON_CREATURE_DAMAGE_RECEIVED, listener => listener.getOwner() == this);
	}
	
	private DamageReturn onDamageReceivedEvent(OnCreatureDamageReceived @event)
	{
		if (@event.getAttacker().calculateDistance3D(@event.getTarget()) > _amount)
		{
			return new DamageReturn(false, true, false, 0);
		}
		return new DamageReturn(false, false, false, @event.getDamage());
	}
}