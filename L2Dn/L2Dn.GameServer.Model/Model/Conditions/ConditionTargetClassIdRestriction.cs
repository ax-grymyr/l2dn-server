using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Items;
using L2Dn.GameServer.Model.Skills;
using L2Dn.GameServer.Utilities;
using L2Dn.Model;

namespace L2Dn.GameServer.Model.Conditions;

/**
 * The Class ConditionTargetClassIdRestriction.
 */
public sealed class ConditionTargetClassIdRestriction(Set<CharacterClass> classId): Condition
{
    protected override bool TestImpl(Creature effector, Creature effected, Skill? skill, ItemTemplate? item)
    {
        Player? effectedPlayer = effected.getActingPlayer();
        return effected.isPlayer() && effectedPlayer is not null && classId.Contains(effectedPlayer.getClassId());
    }
}