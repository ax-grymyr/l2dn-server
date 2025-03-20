using L2Dn.GameServer.Data.Xml;
using L2Dn.GameServer.Enums;
using L2Dn.GameServer.Handlers;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Effects;
using L2Dn.GameServer.Model.Skills;
using L2Dn.GameServer.StaticData;
using L2Dn.GameServer.StaticData.Xml.Skills;
using L2Dn.GameServer.Templates;
using L2Dn.Model.Enums;
using L2Dn.Utilities;
using ThreadPool = L2Dn.GameServer.Utilities.ThreadPool;

namespace L2Dn.GameServer.Scripts.Handlers.EffectHandlers;

public sealed class TriggerSkillByStat: AbstractEffect
{
    private readonly Stat _stat;
    private readonly int _skillId;
    private readonly int _skillLevel;
    private readonly int _skillSubLevel;
    private readonly int _min;
    private readonly int _max;

    public TriggerSkillByStat(EffectParameterSet parameters)
    {
        _stat = parameters.GetEnum<Stat>(XmlSkillEffectParameterType.Stat);
        _skillId = parameters.GetInt32(XmlSkillEffectParameterType.SkillId, 0);
        _skillLevel = parameters.GetInt32(XmlSkillEffectParameterType.SkillLevel, 1);
        _skillSubLevel = parameters.GetInt32(XmlSkillEffectParameterType.SkillSubLevel, 0);
        _min = parameters.GetInt32(XmlSkillEffectParameterType.Min, 0);
        _max = parameters.GetInt32(XmlSkillEffectParameterType.Max, 9999);
    }

    public override void Pump(Creature effected, Skill skill)
    {
        Creature target = effected;

        // In some cases, without ThreadPool, values did not apply.
        ThreadPool.schedule(() =>
        {
            int currentValue = (int)effected.getStat().getValue(_stat);

            // Synchronized because the same skill could be used twice and isAffectedBySkill ignored.
            lock (target)
            {
                if (currentValue >= _min && currentValue <= _max)
                {
                    if (!target.isAffectedBySkill(_skillId))
                    {
                        Skill? triggerSkill = SkillData.Instance.GetSkill(_skillId, _skillLevel, _skillSubLevel);
                        if (triggerSkill == null)
                            return;

                        SkillCaster.triggerCast(target, target, triggerSkill);
                    }
                }
                else
                {
                    target.getEffectList().stopSkillEffects(SkillFinishType.REMOVED, _skillId);
                }
            }
        }, 100);
    }

    public override int GetHashCode() => HashCode.Combine(_stat, _skillId, _skillLevel, _skillSubLevel, _min, _max);

    public override bool Equals(object? obj) =>
        this.EqualsTo(obj, static x => (x._stat, x._skillId, x._skillLevel, x._skillSubLevel, x._min, x._max));
}