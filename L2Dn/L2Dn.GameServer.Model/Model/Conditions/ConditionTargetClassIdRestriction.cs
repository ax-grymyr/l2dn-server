using System.Collections.Frozen;
using L2Dn.GameServer.Enums;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Items;
using L2Dn.GameServer.Templates;

namespace L2Dn.GameServer.Model.Conditions;

/**
 * The Class ConditionTargetClassIdRestriction.
 */
public sealed class ConditionTargetClassIdRestriction(FrozenSet<CharacterClass> classId): Condition
{
    protected override bool TestImpl(Creature effector, Creature? effected, Skill? skill, ItemTemplate? item)
    {
        if (effected is null)
            return false;

        Player? effectedPlayer = effected.getActingPlayer();
        return effected.isPlayer() && effectedPlayer is not null && classId.Contains(effectedPlayer.getClassId());
    }
}