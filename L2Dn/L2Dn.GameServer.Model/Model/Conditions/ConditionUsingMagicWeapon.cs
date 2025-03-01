using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Items;
using L2Dn.GameServer.Model.Skills;

namespace L2Dn.GameServer.Model.Conditions;

public sealed class ConditionUsingMagicWeapon(bool value): Condition
{
    protected override bool TestImpl(Creature effector, Creature? effected, Skill? skill, ItemTemplate? item)
    {
        if (effected == null || !effected.isPlayer())
            return false;

        ItemTemplate? weapon = effected.getActiveWeaponItem();
        return weapon != null && weapon.isMagicWeapon() == value;
    }
}