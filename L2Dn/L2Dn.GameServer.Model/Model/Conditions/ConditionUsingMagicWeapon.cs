using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Items;
using L2Dn.GameServer.Model.Skills;

namespace L2Dn.GameServer.Model.Conditions;

public class ConditionUsingMagicWeapon : Condition
{
    private readonly bool _value;

    public ConditionUsingMagicWeapon(bool value)
    {
        _value = value;
    }

    public override bool testImpl(Creature effector, Creature effected, Skill skill, ItemTemplate item)
    {
        if (effected == null || !effected.isPlayer())
        {
            return false;
        }

        ItemTemplate weapon = effected.getActiveWeaponItem();
        return weapon != null && weapon.isMagicWeapon() == _value;
    }
}