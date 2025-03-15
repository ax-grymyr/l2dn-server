using L2Dn.GameServer.AI;
using L2Dn.GameServer.Data.Xml;
using L2Dn.GameServer.Handlers;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Skills;
using L2Dn.GameServer.Network.Enums;
using L2Dn.GameServer.Network.OutgoingPackets;
using Config = L2Dn.GameServer.Configuration.Config;

namespace L2Dn.GameServer.Scripts.Handlers.UserCommandHandlers;

/**
 * Unstuck user command.
 */
public class Unstuck: IUserCommandHandler
{
    private static readonly int[] COMMAND_IDS = [52];

    public bool useUserCommand(int id, Player player)
    {
        if (player.isJailed())
        {
            player.sendMessage("You cannot use this function while you are jailed.");
            return false;
        }

        if (Config.FactionSystem.FACTION_SYSTEM_ENABLED && !player.isGood() && !player.isEvil())
        {
            player.sendMessage("You cannot use this function while you are neutral.");
            return false;
        }

        int unstuckTimer = (player.getAccessLevel().IsGM ? 1000 : Config.Character.UNSTUCK_INTERVAL * 1000);

        if (player.isInOlympiadMode())
        {
            player.sendPacket(SystemMessageId.THE_SKILL_CANNOT_BE_USED_IN_THE_OLYMPIAD);
            return false;
        }

        if (player.isCastingNow(x => x.isAnyNormalType()) || player.isMovementDisabled() || player.isMuted() ||
            player.isAlikeDead() || player.inObserverMode() || player.isCombatFlagEquipped())
        {
            return false;
        }

        // TODO: review this and refactor
        Skill? escape = SkillData.getInstance().getSkill(2099, 1); // 5 minutes escape
        Skill? gmEscape = SkillData.getInstance().getSkill(2100, 1); // 1 second escape
        if (player.getAccessLevel().IsGM)
        {
            if (gmEscape != null)
            {
                player.doCast(gmEscape);
                return true;
            }

            player.sendMessage("You use Escape: 1 second.");
        }
        else if ((Config.Character.UNSTUCK_INTERVAL == 300) && (escape != null))
        {
            player.doCast(escape);
            return true;
        }
        else if (escape != null)
        {
            SkillCaster? skillCaster = SkillCaster.castSkill(player, player.getTarget(), escape, null,
                SkillCastingType.NORMAL, false, false, TimeSpan.FromMilliseconds(unstuckTimer));

            if (skillCaster == null)
            {
                player.sendPacket(new ActionFailedPacket(SkillCastingType.NORMAL));
                player.getAI().setIntention(CtrlIntention.AI_INTENTION_ACTIVE);
                return false;
            }

            if (Config.Character.UNSTUCK_INTERVAL > 100)
            {
                player.sendMessage("You use Escape: " + (unstuckTimer / 60000) + " minutes.");
            }
            else
            {
                player.sendMessage("You use Escape: " + (unstuckTimer / 1000) + " seconds.");
            }
        }

        return true;
    }

    public int[] getUserCommandList()
    {
        return COMMAND_IDS;
    }
}