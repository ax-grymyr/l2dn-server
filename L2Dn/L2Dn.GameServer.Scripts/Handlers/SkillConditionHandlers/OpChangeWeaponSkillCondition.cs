using L2Dn.GameServer.Handlers;
using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Items;
using L2Dn.GameServer.Model.Skills;
using L2Dn.GameServer.Templates;
using L2Dn.Utilities;

namespace L2Dn.GameServer.Scripts.Handlers.SkillConditionHandlers;

[HandlerStringKey("OpChangeWeapon")]
public sealed class OpChangeWeaponSkillCondition: ISkillCondition
{
    public bool canUse(Creature caster, Skill skill, WorldObject? target)
    {
        Weapon? weaponItem = caster.getActiveWeaponItem();
        if (weaponItem == null)
            return false;

        if (weaponItem.getChangeWeaponId() == 0)
            return false;

        Player? player = caster.getActingPlayer();
        if (player != null && player.hasItemRequest())
            return false;

        return true;
    }

    public override int GetHashCode() => this.GetSingletonHashCode();
    public override bool Equals(object? obj) => this.EqualsTo(obj);
}