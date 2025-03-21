using L2Dn.GameServer.Handlers;
using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Items;
using L2Dn.GameServer.Model.Items.Types;
using L2Dn.GameServer.Model.Skills;
using L2Dn.GameServer.StaticData.Xml.Skills;
using L2Dn.GameServer.Templates;

namespace L2Dn.GameServer.Scripts.Handlers.SkillConditionHandlers;

[HandlerStringKey("EquipWeapon")]
public sealed class EquipWeaponSkillCondition: ISkillCondition
{
    private readonly ItemTypeMask _weaponTypesMask = ItemTypeMask.Zero;

    public EquipWeaponSkillCondition(SkillConditionParameterSet parameters)
    {
        List<string>? weaponTypes = parameters.GetStringListOptional(XmlSkillConditionParameterType.WeaponType);
        if (weaponTypes != null)
        {
            foreach (string weaponType in weaponTypes)
                _weaponTypesMask |= Enum.Parse<WeaponType>(weaponType, true);
        }
    }

    public bool canUse(Creature caster, Skill skill, WorldObject? target)
    {
        ItemTemplate? weapon = caster.getActiveWeaponItem();
        return weapon != null && (weapon.getItemMask() & _weaponTypesMask) != ItemTypeMask.Zero;
    }
}