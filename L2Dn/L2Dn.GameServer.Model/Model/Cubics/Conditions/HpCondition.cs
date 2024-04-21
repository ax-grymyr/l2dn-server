using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Actor.Instances;
using L2Dn.Model.Enums;

namespace L2Dn.GameServer.Model.Cubics.Conditions;

public class HpCondition: ICubicCondition
{
    private readonly CubicHpConditionType _type;
    private readonly int _hpPer;

    public HpCondition(CubicHpConditionType type, int hpPer)
    {
        _type = type;
        _hpPer = hpPer;
    }

    public bool test(Cubic cubic, Creature owner, WorldObject target)
    {
        if (target.isCreature() || target.isDoor())
        {
            double hpPer = (target.isDoor() ? (Door)target : (Creature)target).getCurrentHpPercent();
            switch (_type)
            {
                case CubicHpConditionType.GREATER:
                {
                    return hpPer > _hpPer;
                }
                case CubicHpConditionType.LESSER:
                {
                    return hpPer < _hpPer;
                }
            }
        }

        return false;
    }

    public override string ToString()
    {
        return GetType().Name + " chance: " + _hpPer;
    }
}