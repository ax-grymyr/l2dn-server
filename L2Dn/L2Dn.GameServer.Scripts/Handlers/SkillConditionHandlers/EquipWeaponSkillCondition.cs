using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Items;
using L2Dn.GameServer.Model.Items.Types;
using L2Dn.GameServer.Model.Skills;

namespace L2Dn.GameServer.Scripts.Handlers.SkillConditionHandlers;

/**
 * @author Sdw
 */
public class EquipWeaponSkillCondition: ISkillCondition
{
    private readonly ItemTypeMask _weaponTypesMask = ItemTypeMask.Zero;

    public EquipWeaponSkillCondition(StatSet @params)
    {
        List<WeaponType>? weaponTypes = @params.getEnumList<WeaponType>("weaponType");
        if (weaponTypes != null)
        {
            foreach (WeaponType weaponType in weaponTypes)
            {
                _weaponTypesMask |= weaponType;
            }
        }
    }

    public bool canUse(Creature caster, Skill skill, WorldObject? target)
    {
        ItemTemplate? weapon = caster.getActiveWeaponItem();
        return weapon != null && (weapon.getItemMask() & _weaponTypesMask) != ItemTypeMask.Zero;
    }
}