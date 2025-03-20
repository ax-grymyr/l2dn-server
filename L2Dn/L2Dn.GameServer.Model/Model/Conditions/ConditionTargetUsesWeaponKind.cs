using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Items;
using L2Dn.GameServer.Model.Items.Types;
using L2Dn.GameServer.Model.Skills;
using L2Dn.GameServer.Templates;

namespace L2Dn.GameServer.Model.Conditions;

/**
 * The Class ConditionTargetUsesWeaponKind.
 * @author mkizub
 */
public sealed class ConditionTargetUsesWeaponKind(ItemTypeMask weaponMask): Condition
{
    protected override bool TestImpl(Creature effector, Creature? effected, Skill? skill, ItemTemplate? item)
    {
        if (effected is null)
            return false;

        Weapon? weapon = effected.getActiveWeaponItem();
        if (weapon is null)
            return false;

        return (weapon.getItemMask() & weaponMask) != ItemTypeMask.Zero;
    }
}