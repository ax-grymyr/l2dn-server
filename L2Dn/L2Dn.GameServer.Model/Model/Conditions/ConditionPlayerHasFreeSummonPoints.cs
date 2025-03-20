using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Items;
using L2Dn.GameServer.Model.Skills;
using L2Dn.GameServer.Network.Enums;
using L2Dn.GameServer.Network.OutgoingPackets;
using L2Dn.GameServer.Templates;

namespace L2Dn.GameServer.Model.Conditions;

/**
 * @author Sdw
 */
public sealed class ConditionPlayerHasFreeSummonPoints(int summonPoints): Condition
{
    protected override bool TestImpl(Creature effector, Creature? effected, Skill? skill, ItemTemplate? item)
    {
        if (skill is null)
            return false;

        Player? player = effector.getActingPlayer();
        if (player is null)
            return false;

        bool canSummon = true;
        if (summonPoints == 0 && player.hasServitors())
        {
            player.sendPacket(SystemMessageId.YOU_CANNOT_USE_THE_S1_SKILL_DUE_TO_INSUFFICIENT_SUMMON_POINTS);
            canSummon = false;
        }
        else if (player.getSummonPoints() + summonPoints > player.getMaxSummonPoints())
        {
            SystemMessagePacket sm =
                new SystemMessagePacket(SystemMessageId.YOU_CANNOT_USE_THE_S1_SKILL_DUE_TO_INSUFFICIENT_SUMMON_POINTS);

            sm.Params.addSkillName(skill);
            player.sendPacket(sm);
            canSummon = false;
        }

        return canSummon;
    }
}