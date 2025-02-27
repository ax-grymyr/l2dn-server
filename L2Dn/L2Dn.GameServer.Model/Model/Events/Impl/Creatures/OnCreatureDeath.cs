using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Events.Impl.Base;

namespace L2Dn.GameServer.Model.Events.Impl.Creatures;

/**
 * An instantly executed event when Creature is killed by Creature.
 * @author UnAfraid
 */
public class OnCreatureDeath: TerminateEventBase
{
	private readonly Creature? _attacker;
	private readonly Creature _target;

	public OnCreatureDeath(Creature? attacker, Creature target)
	{
		_attacker = attacker;
		_target = target;
	}

	public Creature? getAttacker()
	{
		return _attacker;
	}

	public Creature getTarget()
	{
		return _target;
	}
}