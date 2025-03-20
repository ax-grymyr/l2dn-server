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

[AbstractEffectName("TriggerSkillByBaseStat")]
public sealed class TriggerSkillByBaseStat: AbstractEffect
{
    private readonly BaseStat _baseStat;
    private readonly int _skillId;
    private readonly int _skillLevel;
    private readonly int _skillSubLevel;
    private readonly int _min;
    private readonly int _max;

    public TriggerSkillByBaseStat(EffectParameterSet parameters)
    {
        _baseStat = parameters.GetEnum<BaseStat>(XmlSkillEffectParameterType.BaseStat);
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
            int currentValue = _baseStat switch
            {
                BaseStat.STR => target.getSTR(),
                BaseStat.INT => target.getINT(),
                BaseStat.DEX => target.getDEX(),
                BaseStat.WIT => target.getWIT(),
                BaseStat.CON => target.getCON(),
                BaseStat.MEN => target.getMEN(),
                _ => 0,
            };

            // Synchronized because the same skill could be used twice and isAffectedBySkill ignored.
            Skill? triggerSkill = SkillData.Instance.GetSkill(_skillId, _skillLevel, _skillSubLevel);
            if (triggerSkill == null)
                return;

            lock (target)
            {
                if (currentValue >= _min && currentValue <= _max)
                {
                    if (!target.isAffectedBySkill(_skillId))
                        SkillCaster.triggerCast(target, target, triggerSkill);
                }
                else
                {
                    target.getEffectList().stopSkillEffects(SkillFinishType.REMOVED, _skillId);
                }
            }
        }, 100);
    }

    public override int GetHashCode() => HashCode.Combine(_baseStat, _skillId, _skillLevel, _skillSubLevel, _min, _max);

    public override bool Equals(object? obj) =>
        this.EqualsTo(obj, static x => (x._baseStat, x._skillId, x._skillLevel, x._skillSubLevel, x._min, x._max));
}