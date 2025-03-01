using L2Dn.GameServer.Enums;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Effects;
using L2Dn.GameServer.Model.Items;
using L2Dn.GameServer.Model.Skills;
using L2Dn.GameServer.Network.Enums;
using L2Dn.GameServer.TaskManagers;

namespace L2Dn.GameServer.Model.Conditions;

/**
 * @author Sdw
 */
public sealed class ConditionPlayerCanSwitchSubclass(int subIndex): Condition
{
    protected override bool TestImpl(Creature effector, Creature? effected, Skill? skill, ItemTemplate? item)
    {
        bool canSwitchSub = true;

        Player? player = effector.getActingPlayer();
        if (player is null || player.isAlikeDead())
            canSwitchSub = false;
        else if ((subIndex != 0 && player.getSubClasses().get(subIndex) == null) || player.getClassIndex() == subIndex)
            canSwitchSub = false;
        else if (!player.isInventoryUnder90(true))
        {
            player.sendPacket(SystemMessageId.
                YOU_CANNOT_CREATE_OR_CHANGE_A_SUBCLASS_WHILE_YOU_HAVE_NO_FREE_SPACE_IN_YOUR_INVENTORY);

            canSwitchSub = false;
        }
        else if (player.getWeightPenalty() >= 2)
        {
            player.sendPacket(SystemMessageId.YOU_CANNOT_CREATE_OR_CHANGE_A_DUAL_CLASS_WHILE_YOU_HAVE_OVERWEIGHT);
            canSwitchSub = false;
        }
        else if (player.isRegisteredOnEvent())
        {
            player.sendMessage("You cannot change your subclass while registered in an event.");
            canSwitchSub = false;
        }
        else if (player.isAllSkillsDisabled())
            canSwitchSub = false;
        else if (player.isAffected(EffectFlag.MUTED))
        {
            canSwitchSub = false;
            player.sendPacket(SystemMessageId.YOU_CANNOT_CHANGE_THE_CLASS_BECAUSE_OF_IDENTITY_CRISIS);
        }
        else if (AttackStanceTaskManager.getInstance().hasAttackStanceTask(player) ||
                 player.getPvpFlag() != PvpFlagStatus.None || player.isInInstance() || player.isTransformed() ||
                 player.isMounted())
        {
            canSwitchSub = false;
        }

        return canSwitchSub;
    }
}