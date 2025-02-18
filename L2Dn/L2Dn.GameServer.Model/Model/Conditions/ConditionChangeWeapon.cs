using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Items;
using L2Dn.GameServer.Model.Skills;

namespace L2Dn.GameServer.Model.Conditions;

/**
 * The Class ConditionChangeWeapon.
 * @author nBd
 */
public sealed class ConditionChangeWeapon(bool required): Condition
{
    /**
     * Test impl.
     * @return true, if successful
     */
    protected override bool TestImpl(Creature effector, Creature effected, Skill? skill, ItemTemplate? item)
    {
        Player? actingPlayer = effector.getActingPlayer();
        if (actingPlayer is null)
            return false;

        if (!required)
            return true;

        Weapon? weaponItem = effector.getActiveWeaponItem();
        if (weaponItem == null)
            return false;

        if (weaponItem.getChangeWeaponId() == 0)
            return false;

        return !actingPlayer.hasItemRequest();
    }
}