using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Items;
using L2Dn.GameServer.Model.Skills;

namespace L2Dn.GameServer.Model.Conditions;

public class ConditionUsingTwoHandWeapon : Condition
{
    private readonly bool _value;

    public ConditionUsingTwoHandWeapon(bool value)
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
        if (weapon != null)
        {
            if (_value)
            {
                return (weapon.getBodyPart() & ItemTemplate.SLOT_LR_HAND) != 0;
            }

            return (weapon.getBodyPart() & ItemTemplate.SLOT_LR_HAND) == 0;
        }

        return false;
    }
}