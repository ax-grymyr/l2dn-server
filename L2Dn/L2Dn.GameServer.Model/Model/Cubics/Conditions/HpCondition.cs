using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Actor.Instances;

namespace L2Dn.GameServer.Model.Cubics.Conditions;

public class HpCondition: ICubicCondition
{
    private readonly HpConditionType _type;
    private readonly int _hpPer;

    public HpCondition(HpConditionType type, int hpPer)
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
                case HpConditionType.GREATER:
                {
                    return hpPer > _hpPer;
                }
                case HpConditionType.LESSER:
                {
                    return hpPer < _hpPer;
                }
            }
        }

        return false;
    }

    public override String ToString()
    {
        return GetType().Name + " chance: " + _hpPer;
    }

    public enum HpConditionType
    {
        GREATER,
        LESSER
    }
}
