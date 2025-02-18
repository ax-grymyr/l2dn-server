using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Items;
using L2Dn.GameServer.Model.Skills;

namespace L2Dn.GameServer.Model.Conditions;

/**
 * The Class ConditionPlayerInvSize.
 * @author Kerberos
 */
public sealed class ConditionPlayerInvSize(int size): Condition
{
    protected override bool TestImpl(Creature effector, Creature effected, Skill? skill, ItemTemplate? item)
	{
        Player? player = effector.getActingPlayer();
        if (player is null)
            return true;

        return player.getInventory().getNonQuestSize() <= player.getInventoryLimit() - size;
    }
}