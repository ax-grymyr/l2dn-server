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

public sealed class TriggerSkillByBaseStat: AbstractEffect
{
    private readonly BaseStat _baseStat;
    private readonly int _skillId;
    private readonly int _skillLevel;
    private readonly int _skillSubLevel;
    private readonly int _min;
    private readonly int _max;

    public TriggerSkillByBaseStat(StatSet @params)
    {
        _baseStat = @params.getEnum<BaseStat>("baseStat");
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
            Skill? triggerSkill = SkillData.getInstance().getSkill(_skillId, _skillLevel, _skillSubLevel);
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