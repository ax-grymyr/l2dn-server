using L2Dn.Events;
using L2Dn.GameServer.Model.Actor;

namespace L2Dn.GameServer.Model.Events.Impl.Attackables;

/**
 * An instantly executed event when Attackable is killed by Player.
 * @author UnAfraid
 */
public class OnAttackableKill: EventBase
{
	private readonly Player _attacker;
	private readonly Attackable _target;
	private readonly bool _isSummon;

	public OnAttackableKill(Player attacker, Attackable target, bool isSummon)
	{
		_attacker = attacker;
		_target = target;
		_isSummon = isSummon;
	}

	public Player getAttacker()
	{
		return _attacker;
	}

	public Attackable getTarget()
	{
		return _target;
	}

	public bool isSummon()
	{
		return _isSummon;
	}
}