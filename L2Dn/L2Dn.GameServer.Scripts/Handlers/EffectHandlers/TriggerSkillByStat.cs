using L2Dn.GameServer.Data.Xml;
using L2Dn.GameServer.Enums;
using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Effects;
using L2Dn.GameServer.Model.Skills;
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

    public TriggerSkillByStat(StatSet @params)
    {
        _stat = @params.getEnum<Stat>("stat");
        _skillId = @params.getInt("skillId", 0);
        _skillLevel = @params.getInt("skillLevel", 1);
        _skillSubLevel = @params.getInt("skillSubLevel", 0);
        _min = @params.getInt("min", 0);
        _max = @params.getInt("max", 9999);
    }

    public override void pump(Creature effected, Skill skill)
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