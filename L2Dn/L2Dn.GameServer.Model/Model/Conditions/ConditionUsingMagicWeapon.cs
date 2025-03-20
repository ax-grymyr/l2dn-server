using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Items;
using L2Dn.GameServer.Model.Skills;
using L2Dn.GameServer.Templates;
using L2Dn.Utilities;

namespace L2Dn.GameServer.Model.Conditions;

public sealed class ConditionUsingMagicWeapon(bool value): Condition
{
    private bool Value => value;

    protected override bool TestImpl(Creature effector, Creature? effected, Skill? skill, ItemTemplate? item)
    {
        if (effected == null || !effected.isPlayer())
            return false;

        ItemTemplate? weapon = effected.getActiveWeaponItem();
        return weapon != null && weapon.isMagicWeapon() == value;
    }

    public override int GetHashCode() => HashCode.Combine(value);
    public override bool Equals(object? obj) => this.EqualsTo(obj, static x => x.Value);
}