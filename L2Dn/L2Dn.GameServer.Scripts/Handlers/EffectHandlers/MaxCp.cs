using L2Dn.GameServer.Enums;
using L2Dn.GameServer.Handlers;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Items.Instances;
using L2Dn.GameServer.Model.Skills;
using L2Dn.GameServer.StaticData.Xml.Skills;
using L2Dn.Model.Enums;
using L2Dn.Utilities;
using ThreadPool = L2Dn.GameServer.Utilities.ThreadPool;

namespace L2Dn.GameServer.Scripts.Handlers.EffectHandlers;

public sealed class MaxCp: AbstractStatEffect
{
    private readonly bool _heal;

    public MaxCp(EffectParameterSet parameters): base(parameters, Stat.MAX_CP)
    {
        _heal = parameters.GetBoolean(XmlSkillEffectParameterType.Heal, false);
    }

    public override void ContinuousInstant(Creature effector, Creature effected, Skill skill, Item? item)
    {
        if (_heal)
        {
            ThreadPool.schedule(() =>
            {
                switch (Mode)
                {
                    case StatModifierType.DIFF:
                    {
                        effected.setCurrentCp(effected.getCurrentCp() + Amount);
                        break;
                    }
                    case StatModifierType.PER:
                    {
                        effected.setCurrentCp(effected.getCurrentCp() + effected.getMaxCp() * (Amount / 100));
                        break;
                    }
                }
            }, 100);
        }
    }

    public override int GetHashCode() => HashCode.Combine(base.GetHashCode(), _heal);
    public override bool Equals(object? obj) => base.Equals(obj) && this.EqualsTo(obj, static x => x._heal);
}