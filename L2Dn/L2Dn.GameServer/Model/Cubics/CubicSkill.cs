using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Cubics.Conditions;
using L2Dn.GameServer.Model.Holders;
using L2Dn.GameServer.Utilities;

namespace L2Dn.GameServer.Model.Cubics;

public class CubicSkill: SkillHolder, ICubicConditionHolder
{
    private readonly int _triggerRate;
    private readonly int _successRate;
    private readonly bool _canUseOnStaticObjects;
    private readonly CubicTargetType _targetType;
    private readonly List<ICubicCondition> _conditions = new();
    private readonly bool _targetDebuff;

    public CubicSkill(StatSet set): base(set.getInt("id"), set.getInt("level"))
    {
        _triggerRate = set.getInt("triggerRate", 100);
        _successRate = set.getInt("successRate", 100);
        _canUseOnStaticObjects = set.getBoolean("canUseOnStaticObjects", false);
        _targetType = set.getEnum<CubicTargetType>("target", CubicTargetType.TARGET);
        _targetDebuff = set.getBoolean("targetDebuff", false);
    }

    public int getTriggerRate()
    {
        return _triggerRate;
    }

    public int getSuccessRate()
    {
        return _successRate;
    }

    public bool canUseOnStaticObjects()
    {
        return _canUseOnStaticObjects;
    }

    public CubicTargetType getTargetType()
    {
        return _targetType;
    }

    public bool isTargetingDebuff()
    {
        return _targetDebuff;
    }

    public override bool validateConditions(Cubic cubic, Creature owner, WorldObject target)
    {
        if (_targetDebuff && target.isCreature() && (((Creature)target).getEffectList().getDebuffCount() == 0))
        {
            return false;
        }

        if (_conditions.isEmpty())
        {
            return true;
        }

        foreach (ICubicCondition condition in _conditions)
        {
            if (!condition.test(cubic, owner, target))
            {
                return false;
            }
        }

        return true;
    }

    public override void addCondition(ICubicCondition condition)
    {
        _conditions.Add(condition);
    }

    public override String ToString()
    {
        return "Cubic skill id: " + getSkillId() + " level: " + getSkillLevel() + " triggerRate: " + _triggerRate +
               " successRate: " + _successRate + " canUseOnStaticObjects: " + _canUseOnStaticObjects + " targetType: " +
               _targetType + " isTargetingDebuff: " + _targetDebuff + Environment.NewLine;
    }
}
