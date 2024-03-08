using L2Dn.GameServer.Enums;
using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Actor.Instances;
using L2Dn.GameServer.Model.Effects;
using L2Dn.GameServer.Model.Events;
using L2Dn.GameServer.Model.Events.Impl.Creatures.Players;
using L2Dn.GameServer.Model.Items.Instances;
using L2Dn.GameServer.Model.Skills;
using L2Dn.GameServer.Network.Enums;

namespace L2Dn.GameServer.Handlers.EffectHandlers;

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
		if (EventDispatcher.getInstance().hasListener(EventType.ON_TRAP_ACTION, trap))
		{
			EventDispatcher.getInstance().notifyEventAsync(new OnTrapAction(trap, effector, TrapAction.TRAP_DISARMED), trap);
		}
		
		trap.unSummon();
		if (effector.isPlayer())
		{
			effector.sendPacket(SystemMessageId.THE_TRAP_DEVICE_HAS_BEEN_STOPPED);
		}
	}
}