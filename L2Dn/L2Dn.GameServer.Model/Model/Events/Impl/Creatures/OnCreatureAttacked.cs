using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Events.Impl.Base;
using L2Dn.GameServer.Model.Skills;
using L2Dn.GameServer.Templates;

namespace L2Dn.GameServer.Model.Events.Impl.Creatures;

/**
 * An instantly executed event when Creature is attacked by Creature.
 * @author UnAfraid
 */
public class OnCreatureAttacked(Creature attacker, Creature target, Skill? skill = null): TerminateEventBase
{
    private Creature _attacker = attacker;
    private Creature _target = target;
    private Skill? _skill = skill;

    public Creature getAttacker() => _attacker;

    public void setAttacker(Creature attacker)
    {
        _attacker = attacker;
    }

    public Creature getTarget() => _target;

    public void setTarget(Creature target)
    {
        _target = target;
    }

    public Skill? getSkill() => _skill;

    public void setSkill(Skill? skill)
    {
        _skill = skill;
    }
}