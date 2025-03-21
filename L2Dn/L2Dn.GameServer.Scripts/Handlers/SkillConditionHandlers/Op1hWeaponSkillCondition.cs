using L2Dn.GameServer.Handlers;
using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Items;
using L2Dn.GameServer.Model.Items.Types;
using L2Dn.GameServer.Model.Skills;
using L2Dn.GameServer.StaticData.Xml.Skills;
using L2Dn.GameServer.Templates;
using L2Dn.GameServer.Utilities;

namespace L2Dn.GameServer.Scripts.Handlers.SkillConditionHandlers;

[HandlerStringKey("Op1hWeapon")]
public sealed class Op1hWeaponSkillCondition: ISkillCondition
{
    private readonly Set<WeaponType> _weaponTypes = new();

    public Op1hWeaponSkillCondition(SkillConditionParameterSet parameters)
    {
        List<string>? weaponTypes = parameters.GetStringListOptional(XmlSkillConditionParameterType.WeaponType);
        if (weaponTypes != null)
        {
            foreach (string type in weaponTypes)
                _weaponTypes.add(Enum.Parse<WeaponType>(type, true));
        }
    }

    public bool canUse(Creature caster, Skill skill, WorldObject? target)
    {
        Weapon? weapon = caster.getActiveWeaponItem();
        if (weapon == null)
            return false;

        foreach (WeaponType weaponType in _weaponTypes)
        {
            if (weapon.getItemType() == weaponType)
                return (weapon.getBodyPart() & ItemTemplate.SLOT_LR_HAND) == 0;
        }

        return false;
    }
}