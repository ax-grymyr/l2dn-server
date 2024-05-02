using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Events.Impl.Creatures;
using L2Dn.GameServer.Model.Items.Instances;
using L2Dn.GameServer.Model.Skills;
using L2Dn.GameServer.Model.Stats;
using L2Dn.Model.Enums;

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
		effected.Events.Subscribe<OnCreatureDamageReceived>(this, onDamageReceivedEvent);
	}
	
	public override void onExit(Creature effector, Creature effected, Skill skill)
	{
		effected.Events.Unsubscribe<OnCreatureDamageReceived>(onDamageReceivedEvent);
	}
	
	private void onDamageReceivedEvent(OnCreatureDamageReceived ev)
	{
		if (ev.getAttacker().calculateDistance3D(ev.getTarget().getLocation().Location3D) > _amount)
		{
			ev.OverrideDamage = true;
			ev.OverridenDamage = 0;
		}
	}
}