using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Items;
using L2Dn.GameServer.Model.Skills;
using L2Dn.Model.Enums;

namespace L2Dn.GameServer.Model.Conditions;

/**
 * The Class ConditionPlayerPledgeClass.
 * @author MrPoke
 */
public sealed class ConditionPlayerPledgeClass(SocialClass pledgeClass): Condition
{
    /**
     * Test impl.
     * @return true, if successful
     */
    protected override bool TestImpl(Creature effector, Creature? effected, Skill? skill, ItemTemplate? item)
    {
        Player? player = effector.getActingPlayer();
        if (player?.getClan() == null)
            return false;

        bool isClanLeader = player.isClanLeader();
        if (pledgeClass == (SocialClass)(-1) && !isClanLeader)
            return false;

        return isClanLeader || player.getPledgeClass() >= pledgeClass;
    }
}