using L2Dn.GameServer.Enums;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Items;
using L2Dn.GameServer.Model.Skills;

namespace L2Dn.GameServer.Model.Conditions;

/**
 * The Class ConditionPlayerIsPvpFlagged.
 * @author Mobius
 */
public sealed class ConditionPlayerIsPvpFlagged(bool value): Condition
{
    protected override bool TestImpl(Creature effector, Creature effected, Skill? skill, ItemTemplate? item)
    {
        Player? player = effector.getActingPlayer();
        return player is not null && (player.getPvpFlag() != PvpFlagStatus.None) == value;
    }
}