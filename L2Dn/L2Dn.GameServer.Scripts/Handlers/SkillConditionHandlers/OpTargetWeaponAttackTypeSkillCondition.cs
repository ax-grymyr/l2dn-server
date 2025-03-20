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

public sealed class OpTargetWeaponAttackTypeSkillCondition: ISkillCondition
{
    private readonly Set<WeaponType> _weaponTypes = new();

    public OpTargetWeaponAttackTypeSkillCondition(SkillConditionParameterSet parameters)
    {
        List<string>? weaponTypes = parameters.GetStringListOptional(XmlSkillConditionParameterType.WeaponType);
        if (weaponTypes != null)
        {
            foreach (string type in weaponTypes)
            {
                _weaponTypes.add(Enum.Parse<WeaponType>(type));
            }
        }
    }

    public bool canUse(Creature caster, Skill skill, WorldObject? target)
    {
        if (target == null || !target.isCreature())
        {
            return false;
        }

        Creature targetCreature = (Creature)target;
        Weapon? weapon = targetCreature.getActiveWeaponItem();
        if (weapon == null)
        {
            return false;
        }

        WeaponType equippedType = weapon.getWeaponType();
        foreach (WeaponType weaponType in _weaponTypes)
        {
            if (weaponType == equippedType)
            {
                return true;
            }
        }

        return false;
    }
}