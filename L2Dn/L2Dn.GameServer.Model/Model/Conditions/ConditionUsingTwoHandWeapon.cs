using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Items;
using L2Dn.GameServer.Model.Skills;

namespace L2Dn.GameServer.Model.Conditions;

public sealed class ConditionUsingTwoHandWeapon(bool value): Condition
{
    protected override bool TestImpl(Creature effector, Creature? effected, Skill? skill, ItemTemplate? item)
    {
        if (effected == null || !effected.isPlayer()) // TODO: maybe condition for effector, not for effected? verify
            return false;

        ItemTemplate? weapon = effected.getActiveWeaponItem();
        if (weapon != null)
        {
            if (value)
                return (weapon.getBodyPart() & ItemTemplate.SLOT_LR_HAND) != 0;

            return (weapon.getBodyPart() & ItemTemplate.SLOT_LR_HAND) == 0;
        }

        return false;
    }
}