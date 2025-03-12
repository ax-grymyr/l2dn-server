using L2Dn.GameServer.Data.Xml;
using L2Dn.GameServer.Enums;
using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Effects;
using L2Dn.GameServer.Model.Items.Instances;
using L2Dn.GameServer.Model.Skills;
using L2Dn.Utilities;
using ThreadPool = L2Dn.GameServer.Utilities.ThreadPool;

namespace L2Dn.GameServer.Scripts.Handlers.EffectHandlers;

public sealed class TriggerSkillByMaxHp: AbstractEffect
{
    private readonly int _skillId;
    private readonly int _skillLevel;
    private readonly int _from;
    private readonly int _to;

    public TriggerSkillByMaxHp(StatSet @params)
    {
        _skillId = @params.getInt("skillId", 0);
        _skillLevel = @params.getInt("skillLevel", 1);
        _from = @params.getInt("from", 0);
        _to = @params.getInt("to", int.MaxValue);
    }

    public override void onStart(Creature effector, Creature effected, Skill skill, Item? item)
    {
        // Delay so that HP bonuses will be calculated first.
        ThreadPool.schedule(() =>
        {
            int hpMax = effected.getMaxHp();
            if (hpMax >= _from && hpMax <= _to)
            {
                if (!effected.isAffectedBySkill(_skillId))
                {
                    Skill? triggerSkill = SkillData.getInstance().getSkill(_skillId, _skillLevel);
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