using L2Dn.GameServer.Enums;
using L2Dn.GameServer.Handlers;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Items.Instances;
using L2Dn.GameServer.StaticData.Xml.Skills;
using L2Dn.GameServer.Templates;
using L2Dn.Model.Enums;
using L2Dn.Utilities;
using ThreadPool = L2Dn.GameServer.Utilities.ThreadPool;

namespace L2Dn.GameServer.Scripts.Handlers.EffectHandlers;

[AbstractEffectName("MaxHp")]
public sealed class MaxHp: AbstractStatEffect
{
    private readonly bool _heal;

    public MaxHp(EffectParameterSet parameters): base(parameters, Stat.MAX_HP)
    {
        _heal = parameters.GetBoolean(XmlSkillEffectParameterType.Heal, false);
    }

    public override void ContinuousInstant(Creature effector, Creature effected, Skill skill, Item? item)
    {
        if (_heal)
        {
            ThreadPool.schedule(() =>
            {
                if (!effected.isHpBlocked())
                {
                    switch (Mode)
                    {
                        case StatModifierType.DIFF:
                        {
                            effected.setCurrentHp(effected.getCurrentHp() + Amount);
                            break;
                        }
                        case StatModifierType.PER:
                        {
                            effected.setCurrentHp(effected.getCurrentHp() + effected.getMaxHp() * (Amount / 100));
                            break;
                        }
                    }
                }
            }, 100);
        }
    }

    public override int GetHashCode() => HashCode.Combine(base.GetHashCode(), _heal);
    public override bool Equals(object? obj) => base.Equals(obj) && this.EqualsTo(obj, static x => x._heal);
}