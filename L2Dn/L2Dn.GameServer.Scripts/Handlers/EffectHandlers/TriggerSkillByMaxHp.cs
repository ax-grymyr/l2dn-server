using L2Dn.GameServer.Data.Xml;
using L2Dn.GameServer.Enums;
using L2Dn.GameServer.Handlers;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Effects;
using L2Dn.GameServer.Model.Items.Instances;
using L2Dn.GameServer.Model.Skills;
using L2Dn.GameServer.StaticData;
using L2Dn.GameServer.StaticData.Xml.Skills;
using L2Dn.GameServer.Templates;
using L2Dn.Utilities;
using ThreadPool = L2Dn.GameServer.Utilities.ThreadPool;

namespace L2Dn.GameServer.Scripts.Handlers.EffectHandlers;

public sealed class TriggerSkillByMaxHp: AbstractEffect
{
    private readonly int _skillId;
    private readonly int _skillLevel;
    private readonly int _from;
    private readonly int _to;

    public TriggerSkillByMaxHp(EffectParameterSet parameters)
    {
        _skillId = parameters.GetInt32(XmlSkillEffectParameterType.SkillId, 0);
        _skillLevel = parameters.GetInt32(XmlSkillEffectParameterType.SkillLevel, 1);
        _from = parameters.GetInt32(XmlSkillEffectParameterType.From, 0);
        _to = parameters.GetInt32(XmlSkillEffectParameterType.To, int.MaxValue);
    }

    public override void OnStart(Creature effector, Creature effected, Skill skill, Item? item)
    {
        // Delay so that HP bonuses will be calculated first.
        ThreadPool.schedule(() =>
        {
            int hpMax = effected.getMaxHp();
            if (hpMax >= _from && hpMax <= _to)
            {
                if (!effected.isAffectedBySkill(_skillId))
                {
                    Skill? triggerSkill = SkillData.Instance.GetSkill(_skillId, _skillLevel);
                    if (triggerSkill == null)
                        return;

                    SkillCaster.triggerCast(effected, effected, triggerSkill);
                }
            }
            else
            {
                effected.getEffectList().stopSkillEffects(SkillFinishType.REMOVED, _skillId);
            }
        }, 100);
    }

    public override int GetHashCode() => HashCode.Combine(_skillId, _skillLevel, _from, _to);

    public override bool Equals(object? obj) =>
        this.EqualsTo(obj, static x => (x._skillId, x._skillLevel, x._from, x._to));
}