using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Cubics.Conditions;
using L2Dn.GameServer.Model.Holders;
using L2Dn.GameServer.Utilities;
using L2Dn.Model.DataPack;
using L2Dn.Model.Enums;

namespace L2Dn.GameServer.Model.Cubics;

public class CubicSkill: SkillHolder
{
    private readonly int _triggerRate;
    private readonly int _successRate;
    private readonly bool _canUseOnStaticObjects;
    private readonly CubicTargetType _targetType;
    private readonly List<ICubicCondition> _conditions = new();
    private readonly bool _targetDebuff;

    public CubicSkill(XmlCubicSkill xmlCubicSkill): base(xmlCubicSkill.Id, xmlCubicSkill.Level)
    {
        _triggerRate = xmlCubicSkill.TriggerRateSpecified ? xmlCubicSkill.TriggerRate : 100;
        _successRate = xmlCubicSkill.SuccessRateSpecified ? xmlCubicSkill.SuccessRate : 100;
        _canUseOnStaticObjects = xmlCubicSkill.CanUseOnStaticObjects;
        _targetType = xmlCubicSkill.TargetSpecified ? xmlCubicSkill.Target : CubicTargetType.TARGET;
        _targetDebuff = xmlCubicSkill.TargetDebuff;
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

    public bool validateConditions(Cubic cubic, Creature owner, WorldObject target)
    {
        if (_targetDebuff && target.isCreature() && ((Creature)target).getEffectList().getDebuffCount() == 0)
        {
            return false;
        }

        if (_conditions.Count == 0)
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

    public void addCondition(ICubicCondition condition)
    {
        _conditions.Add(condition);
    }

    public override string ToString()
    {
        return "Cubic skill id: " + getSkillId() + " level: " + getSkillLevel() + " triggerRate: " + _triggerRate +
               " successRate: " + _successRate + " canUseOnStaticObjects: " + _canUseOnStaticObjects + " targetType: " +
               _targetType + " isTargetingDebuff: " + _targetDebuff + Environment.NewLine;
    }
}
