using L2Dn.GameServer.Enums;
using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Actor.Instances;
using L2Dn.GameServer.Model.Effects;
using L2Dn.GameServer.Model.Events.Impl.Traps;
using L2Dn.GameServer.Model.Items.Instances;
using L2Dn.GameServer.Model.Skills;
using L2Dn.GameServer.Network.Enums;

namespace L2Dn.GameServer.Scripts.Handlers.EffectHandlers;

/**
 * Trap Remove effect implementation.
 * @author UnAfraid
 */
public class TrapRemove: AbstractEffect
{
	private readonly int _power;
	
	public TrapRemove(StatSet @params)
	{
		if (@params.isEmpty())
		{
			throw new ArgumentException(GetType().Name + ": effect without power!");
		}
		
		_power = @params.getInt("power");
	}
	
	public override bool isInstant()
	{
		return true;
	}
	
	public override void instant(Creature effector, Creature effected, Skill skill, Item item)
	{
		if (!effected.isTrap())
		{
			return;
		}
		
		if (effected.isAlikeDead())
		{
			return;
		}
		
		Trap trap = (Trap) effected;
		if (!trap.canBeSeen(effector))
		{
			if (effector.isPlayer())
			{
				effector.sendPacket(SystemMessageId.INVALID_TARGET);
			}
			return;
		}
		
		if (trap.getLevel() > _power)
		{
			return;
		}
		
		// Notify to scripts
		if (trap.Events.HasSubscribers<OnTrapAction>())
		{
			trap.Events.NotifyAsync(new OnTrapAction(trap, effector, TrapAction.TRAP_DISARMED));
		}
		
		trap.unSummon();
		if (effector.isPlayer())
		{
			effector.sendPacket(SystemMessageId.THE_TRAP_DEVICE_HAS_BEEN_STOPPED);
		}
	}
}