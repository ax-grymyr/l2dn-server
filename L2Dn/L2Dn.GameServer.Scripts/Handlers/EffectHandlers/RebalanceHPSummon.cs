using L2Dn.GameServer.Enums;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Effects;
using L2Dn.GameServer.Model.Items.Instances;
using L2Dn.GameServer.Model.Skills;
using L2Dn.GameServer.Utilities;
using L2Dn.Utilities;

namespace L2Dn.GameServer.Scripts.Handlers.EffectHandlers;

/// <summary>
/// Rebalance HP effect implementation.
/// </summary>
public sealed class RebalanceHPSummon: AbstractEffect
{
    public override EffectTypes EffectTypes => EffectTypes.REBALANCE_HP;

    public override bool IsInstant => true;

    public override void Instant(Creature effector, Creature effected, Skill skill, Item? item)
    {
        if (!effector.isPlayer())
            return;

        double fullHp = 0;
        double currentHPs = 0;

        foreach (L2Dn.GameServer.Model.Actor.Summon summon in effector.getServitors().Values)
        {
            if (!summon.isDead() && Util.checkIfInRange(skill.AffectRange, effector, summon, true))
            {
                fullHp += summon.getMaxHp();
                currentHPs += summon.getCurrentHp();
            }
        }

        fullHp += effector.getMaxHp();
        currentHPs += effector.getCurrentHp();

        double percentHP = currentHPs / fullHp;
        double newHP;
        foreach (L2Dn.GameServer.Model.Actor.Summon summon in effector.getServitors().Values)
        {
            if (!summon.isDead() && Util.checkIfInRange(skill.AffectRange, effector, summon, true))
            {
                newHP = summon.getMaxHp() * percentHP;
                if (newHP > summon.getCurrentHp()) // The target gets healed
                {
                    // The heal will be blocked if the current hp passes the limit
                    if (summon.getCurrentHp() > summon.getMaxRecoverableHp())
                    {
                        newHP = summon.getCurrentHp();
                    }
                    else if (newHP > summon.getMaxRecoverableHp())
                    {
                        newHP = summon.getMaxRecoverableHp();
                    }
                }

                summon.setCurrentHp(newHP);
            }
        }

        newHP = effector.getMaxHp() * percentHP;
        if (newHP > effector.getCurrentHp()) // The target gets healed
        {
            // The heal will be blocked if the current hp passes the limit
            if (effector.getCurrentHp() > effector.getMaxRecoverableHp())
            {
                newHP = effector.getCurrentHp();
            }
            else if (newHP > effector.getMaxRecoverableHp())
            {
                newHP = effector.getMaxRecoverableHp();
            }
        }

        effector.setCurrentHp(newHP);
    }

    public override int GetHashCode() => this.GetSingletonHashCode();
    public override bool Equals(object? obj) => this.EqualsTo(obj);
}