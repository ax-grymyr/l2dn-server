using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Effects;
using L2Dn.GameServer.Model.Events.Impl.Creatures;
using L2Dn.GameServer.Model.Holders;
using L2Dn.GameServer.Model.Items.Instances;
using L2Dn.GameServer.Model.Skills;
using L2Dn.Utilities;

namespace L2Dn.GameServer.Scripts.Handlers.EffectHandlers;

/// <summary>
/// Trigger Skill By Kill effect implementation.
/// </summary>
public sealed class TriggerSkillByKill: AbstractEffect
{
    private readonly int _chance;
    private readonly SkillHolder _skill;

    public TriggerSkillByKill(StatSet @params)
    {
        _chance = @params.getInt("chance", 100);
        _skill = new SkillHolder(@params.getInt("skillId", 0), @params.getInt("skillLevel", 0));
    }

    private void onCreatureKilled(OnCreatureKilled ev, Creature target)
    {
        if (_chance == 0 || _skill.getSkillId() == 0 || _skill.getSkillLevel() == 0)
            return;

        if (Rnd.get(100) > _chance)
            return;

        Skill triggerSkill = _skill.getSkill();

        if (ev.getAttacker() == target)
            SkillCaster.triggerCast(target, target, triggerSkill);
    }

    public override void onExit(Creature effector, Creature effected, Skill skill)
    {
        effected.Events.UnsubscribeAll<OnCreatureKilled>(this);
    }

    public override void onStart(Creature effector, Creature effected, Skill skill, Item? item)
    {
        effected.Events.Subscribe<OnCreatureKilled>(this, ev => onCreatureKilled(ev, effected));
    }

    public override int GetHashCode() => HashCode.Combine(_chance, _skill);
    public override bool Equals(object? obj) => this.EqualsTo(obj, static x => (x._chance, x._skill));
}