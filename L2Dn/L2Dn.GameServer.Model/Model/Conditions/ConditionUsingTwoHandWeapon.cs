using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Items;
using L2Dn.GameServer.Model.Skills;
using L2Dn.Utilities;

namespace L2Dn.GameServer.Model.Conditions;

public sealed class ConditionUsingTwoHandWeapon(bool value): Condition
{
    private bool Value => value;

    protected override bool TestImpl(Creature effector, Creature? effected, Skill? skill, ItemTemplate? item)
    {
        if (effected == null || !effected.isPlayer()) // TODO: maybe condition for effector, not for effected? verify
            return false;

        ItemTemplate? weapon = effected.getActiveWeaponItem();
        if (weapon != null)
        {
            if (value)
                return (weapon.getBodyPart() & ItemTemplate.SLOT_LR_HAND) != 0;

            return (weapon.getBodyPart() & ItemTemplate.SLOT_LR_HAND) == 0;
        }

        return false;
    }

    public override int GetHashCode() => HashCode.Combine(value);
    public override bool Equals(object? obj) => this.EqualsTo(obj, static x => x.Value);
}