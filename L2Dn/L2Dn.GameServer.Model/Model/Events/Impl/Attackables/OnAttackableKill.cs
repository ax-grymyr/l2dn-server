using L2Dn.Events;
using L2Dn.GameServer.Model.Actor;

namespace L2Dn.GameServer.Model.Events.Impl.Attackables;

/**
 * An instantly executed event when Attackable is killed by Player.
 * @author UnAfraid
 */
public class OnAttackableKill(Player? attacker, Attackable target, bool summon): EventBase
{
    public Player? getAttacker()
	{
		return attacker;
	}

	public Attackable getTarget()
	{
		return target;
	}

	public bool isSummon()
	{
		return summon;
	}
}