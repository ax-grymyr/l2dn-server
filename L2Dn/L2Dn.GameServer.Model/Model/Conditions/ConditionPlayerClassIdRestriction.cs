using L2Dn.GameServer.Enums;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Items;
using L2Dn.GameServer.Model.Skills;
using L2Dn.GameServer.Utilities;
using L2Dn.Model;

namespace L2Dn.GameServer.Model.Conditions;

/**
 * The Class ConditionPlayerClassIdRestriction.
 */
public sealed class ConditionPlayerClassIdRestriction(Set<CharacterClass> classId): Condition
{
    protected override bool TestImpl(Creature effector, Creature? effected, Skill? skill, ItemTemplate? item)
    {
        Player? player = effector.getActingPlayer();
        return player is not null && classId.Contains(player.getClassId());
    }
}