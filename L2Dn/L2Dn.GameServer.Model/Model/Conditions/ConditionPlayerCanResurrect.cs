using L2Dn.GameServer.InstanceManagers;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Clans;
using L2Dn.GameServer.Model.Items;
using L2Dn.GameServer.Model.Sieges;
using L2Dn.GameServer.Model.Skills;
using L2Dn.GameServer.Network.Enums;
using L2Dn.GameServer.Network.OutgoingPackets;
using L2Dn.GameServer.Templates;

namespace L2Dn.GameServer.Model.Conditions;

/**
 * Player Can Resurrect condition implementation.
 * @author UnAfraid
 */
public sealed class ConditionPlayerCanResurrect(bool value): Condition
{
    protected override bool TestImpl(Creature effector, Creature? effected, Skill? skill, ItemTemplate? item)
    {
        // Need skill rework for fix that properly
        if (skill is null || skill.AffectRange > 0)
            return true;

        if (effected == null)
            return false;

        bool canResurrect = true;
        if (effected.isPlayer())
        {
            Player? player = effected.getActingPlayer();
            if (player is null)
                return false;

            if (!player.isDead())
            {
                canResurrect = false;
                if (effector.isPlayer())
                {
                    SystemMessagePacket msg =
                        new SystemMessagePacket(SystemMessageId.S1_CANNOT_BE_USED_THE_REQUIREMENTS_ARE_NOT_MET);
                    msg.Params.addSkillName(skill);
                    effector.sendPacket(msg);
                }
            }
            else if (player.isResurrectionBlocked())
            {
                canResurrect = false;
                if (effector.isPlayer())
                {
                    effector.sendPacket(SystemMessageId.REJECT_RESURRECTION);
                }
            }
            else if (player.isReviveRequested())
            {
                canResurrect = false;
                if (effector.isPlayer())
                {
                    effector.sendPacket(SystemMessageId.RESURRECTION_HAS_ALREADY_BEEN_PROPOSED);
                }
            }
            else if (skill.Id != 2393) // Blessed Scroll of Battlefield Resurrection
            {
                Siege? siege = SiegeManager.getInstance().getSiege(player);
                if (siege != null && siege.isInProgress())
                {
                    Clan? clan = player.getClan();
                    if (clan == null)
                    {
                        canResurrect = false;
                        if (effector.isPlayer())
                        {
                            effector.sendPacket(SystemMessageId.
                                IT_IS_NOT_POSSIBLE_TO_RESURRECT_IN_BATTLEGROUNDS_WHERE_A_SIEGE_WAR_IS_TAKING_PLACE);
                        }
                    }
                    else if (siege.checkIsDefender(clan) && siege.getControlTowerCount() == 0)
                    {
                        canResurrect = false;
                        if (effector.isPlayer())
                        {
                            effector.sendPacket(SystemMessageId.
                                THE_GUARDIAN_TOWER_HAS_BEEN_DESTROYED_AND_RESURRECTION_IS_NOT_POSSIBLE);
                        }
                    }
                    else if (siege.checkIsAttacker(clan) && siege.getAttackerClan(clan)?.getNumFlags() == 0)
                    {
                        canResurrect = false;
                        if (effector.isPlayer())
                        {
                            effector.sendPacket(SystemMessageId.
                                IF_A_BASE_CAMP_DOES_NOT_EXIST_RESURRECTION_IS_NOT_POSSIBLE);
                        }
                    }
                    else
                    {
                        canResurrect = false;
                        if (effector.isPlayer())
                        {
                            effector.sendPacket(SystemMessageId.
                                IT_IS_NOT_POSSIBLE_TO_RESURRECT_IN_BATTLEGROUNDS_WHERE_A_SIEGE_WAR_IS_TAKING_PLACE);
                        }
                    }
                }
            }
        }
        else if (effected.isSummon())
        {
            Summon summon = (Summon)effected;
            Player player = summon.getOwner();
            if (!summon.isDead())
            {
                canResurrect = false;
                if (effector.isPlayer())
                {
                    SystemMessagePacket msg =
                        new SystemMessagePacket(SystemMessageId.S1_CANNOT_BE_USED_THE_REQUIREMENTS_ARE_NOT_MET);
                    msg.Params.addSkillName(skill);
                    effector.sendPacket(msg);
                }
            }
            else if (summon.isResurrectionBlocked())
            {
                canResurrect = false;
                if (effector.isPlayer())
                {
                    effector.sendPacket(SystemMessageId.REJECT_RESURRECTION);
                }
            }
            else if (player != null && player.isRevivingPet())
            {
                canResurrect = false;
                if (effector.isPlayer())
                {
                    effector.sendPacket(SystemMessageId.
                        RESURRECTION_HAS_ALREADY_BEEN_PROPOSED); // Resurrection is already been proposed.
                }
            }
        }

        return value == canResurrect;
    }
}