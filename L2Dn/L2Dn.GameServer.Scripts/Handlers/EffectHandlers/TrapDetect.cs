using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Actor.Instances;
using L2Dn.GameServer.Model.Effects;
using L2Dn.GameServer.Model.Items.Instances;
using L2Dn.GameServer.Model.Skills;

namespace L2Dn.GameServer.Scripts.Handlers.EffectHandlers;

/**
 * Trap Detect effect implementation.
 * @author UnAfraid
 */
public class TrapDetect: AbstractEffect
{
	private readonly int _power;

	public TrapDetect(StatSet @params)
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

	public override void instant(Creature effector, Creature effected, Skill skill, Item? item)
	{
		if (!effected.isTrap() || effected.isAlikeDead())
		{
			return;
		}

		Trap trap = (Trap) effected;
		if (trap.getLevel() <= _power)
		{
			trap.setDetected(effector);
		}
	}
}