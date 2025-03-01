using L2Dn.GameServer.Db;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Items;
using L2Dn.GameServer.Model.Skills;

namespace L2Dn.GameServer.Model.Conditions;

/**
 * The Class ConditionPlayerSex.
 */
public sealed class ConditionPlayerSex(Sex sex): Condition
{
    protected override bool TestImpl(Creature effector, Creature? effected, Skill? skill, ItemTemplate? item)
    {
        Player? actingPlayer = effector.getActingPlayer();
        if (actingPlayer is null)
            return false;

        return actingPlayer.getAppearance().getSex() == sex;
    }
}