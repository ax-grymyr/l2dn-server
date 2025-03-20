using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Items;
using L2Dn.GameServer.Model.Skills;
using L2Dn.GameServer.Templates;

namespace L2Dn.GameServer.Model.Conditions;

/**
 * @author NosBit
 */
public sealed class ConditionUsingSlotType(long mask): Condition
{
    protected override bool TestImpl(Creature effector, Creature? effected, Skill? skill, ItemTemplate? item)
    {
        if (!effector.isPlayer())
            return false;

        Weapon? activeWeapon = effector.getActiveWeaponItem();
        if (activeWeapon == null)
            return false;

        return (activeWeapon.getBodyPart() & mask) != 0;
    }
}