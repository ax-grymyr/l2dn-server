using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Effects;
using L2Dn.GameServer.Model.Holders;
using L2Dn.GameServer.Model.Items.Instances;
using L2Dn.GameServer.Model.Skills;
using L2Dn.Utilities;

namespace L2Dn.GameServer.Scripts.Handlers.EffectHandlers;

/// <summary>
/// Damage Over Time effect implementation.
/// </summary>
public sealed class CallSkillOnActionTime: AbstractEffect
{
    private readonly SkillHolder _skill;

    public CallSkillOnActionTime(StatSet @params)
    {
        _skill = new SkillHolder(@params.getInt("skillId"), @params.getInt("skillLevel", 1),
            @params.getInt("skillSubLevel", 0));

        Ticks = @params.getInt("ticks");
    }

    public override void onStart(Creature effector, Creature effected, Skill skill, Item? item)
    {
        effected.getEffectList().stopEffects([_skill.getSkill().getAbnormalType()]);
        effected.getEffectList().addBlockedAbnormalTypes([_skill.getSkill().getAbnormalType()]);
    }

    public override void onExit(Creature effector, Creature effected, Skill skill)
    {
        effected.getEffectList().removeBlockedAbnormalTypes([_skill.getSkill().getAbnormalType()]);
    }

    public override bool onActionTime(Creature effector, Creature effected, Skill skill, Item? item)
    {
        if (effector.isDead())
            return false;

        Skill triggerSkill = _skill.getSkill();
        if (triggerSkill != null)
        {
            if (triggerSkill.isSynergySkill())
            {
                triggerSkill.applyEffects(effector, effector);
            }

            World.getInstance().forEachVisibleObjectInRange<Creature>(effector, _skill.getSkill().getAffectRange(), c =>
            {
                WorldObject? target = triggerSkill.getTarget(effector, c, false, false, false);
                if (target != null && target.isCreature())
                    SkillCaster.triggerCast(effector, (Creature)target, triggerSkill);
            });
        }
        else
        {
            LOGGER.Warn("Skill not found effect called from " + skill);
        }

        return skill.isToggle();
    }

    public override int GetHashCode() => HashCode.Combine(_skill, Ticks);
    public override bool Equals(object? obj) => this.EqualsTo(obj, static x => (x._skill, x.Ticks));
}