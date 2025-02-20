using L2Dn.Events;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Skills;

namespace L2Dn.GameServer.Model.Events.Impl.Attackables;

/**
 * An instantly executed event when Attackable is attacked by Player.
 * @author UnAfraid
 */
public sealed class OnAttackableAttack(Player? attacker, Attackable target, int damage, Skill skill, bool summon)
    : EventBase
{
    public Player? getAttacker()
    {
        return attacker;
    }

    public Attackable getTarget()
    {
        return target;
    }

    public int getDamage()
    {
        return damage;
    }

    public Skill getSkill()
    {
        return skill;
    }

    public bool isSummon()
    {
        return summon;
    }
}