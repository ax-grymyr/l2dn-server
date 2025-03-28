using L2Dn.GameServer.Enums;
using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Conditions;
using L2Dn.GameServer.Model.Skills;
using L2Dn.Model.Enums;

namespace L2Dn.GameServer.Scripts.Handlers.EffectHandlers;

public sealed class CriticalRate: AbstractConditionalHpEffect
{
    public CriticalRate(StatSet @params): base(@params, Stat.CRITICAL_RATE)
    {
    }

    public override void pump(Creature effected, Skill skill)
    {
        foreach (Condition cond in Conditions)
        {
            if (!cond.test(effected, effected, skill))
                return;
        }

        switch (Mode)
        {
            case StatModifierType.DIFF:
            {
                effected.getStat().mergeAdd(AddStat, Amount);
                break;
            }
            case StatModifierType.PER:
            {
                effected.getStat().mergeMul(MulStat, Amount / 100);
                break;
            }
        }
    }
}