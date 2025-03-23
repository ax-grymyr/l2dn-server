using System.Collections.Frozen;
using L2Dn.Extensions;
using L2Dn.GameServer.Handlers;
using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Items;
using L2Dn.GameServer.Model.Items.Types;
using L2Dn.GameServer.Model.Skills;
using L2Dn.GameServer.StaticData.Xml.Skills;
using L2Dn.GameServer.Templates;
using L2Dn.Utilities;

namespace L2Dn.GameServer.Scripts.Handlers.SkillConditionHandlers;

[HandlerStringKey("OpTargetWeaponAttackType")]
public sealed class OpTargetWeaponAttackTypeSkillCondition: ISkillCondition
{
    private readonly FrozenSet<WeaponType> _weaponTypes;

    public OpTargetWeaponAttackTypeSkillCondition(SkillConditionParameterSet parameters)
    {
        List<string>? weaponTypes = parameters.GetStringListOptional(XmlSkillConditionParameterType.WeaponType);
        _weaponTypes = weaponTypes is null
            ? FrozenSet<WeaponType>.Empty
            : weaponTypes.Select(type => Enum.Parse<WeaponType>(type, true)).ToFrozenSet();
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

    public override int GetHashCode() => _weaponTypes.GetSetHashCode();
    public override bool Equals(object? obj) => this.EqualsTo(obj, static x => x._weaponTypes.GetSetComparable());
}