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

[HandlerStringKey("Op1hWeapon")]
public sealed class Op1hWeaponSkillCondition: ISkillCondition
{
    private readonly FrozenSet<WeaponType> _weaponTypes;

    public Op1hWeaponSkillCondition(SkillConditionParameterSet parameters)
    {
        List<string>? weaponTypes = parameters.GetStringListOptional(XmlSkillConditionParameterType.WeaponType);
        _weaponTypes = weaponTypes is null
            ? FrozenSet<WeaponType>.Empty
            : weaponTypes.Select(type => Enum.Parse<WeaponType>(type, true)).ToFrozenSet();
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

    public override int GetHashCode() => _weaponTypes.GetSetHashCode();
    public override bool Equals(object? obj) => this.EqualsTo(obj, static x => x._weaponTypes.GetSetComparable());
}