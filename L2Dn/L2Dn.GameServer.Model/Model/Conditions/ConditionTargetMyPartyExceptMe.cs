using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Items;
using L2Dn.GameServer.Model.Skills;
using L2Dn.GameServer.Network.Enums;
using L2Dn.GameServer.Network.OutgoingPackets;

namespace L2Dn.GameServer.Model.Conditions;

/**
 * Target My Party Except Me condition implementation.
 * @author Adry_85
 */
public sealed class ConditionTargetMyPartyExceptMe(bool value): Condition
{
    protected override bool TestImpl(Creature effector, Creature? effected, Skill? skill, ItemTemplate? item)
    {
        Player? player = effector.getActingPlayer();
        bool isPartyMember = true;
        if (player == null || effected == null || !effected.isPlayer() || skill is null)
        {
            isPartyMember = false;
        }
        else if (player == effected)
        {
            player.sendPacket(SystemMessageId.YOU_CANNOT_USE_THIS_ON_YOURSELF);
            isPartyMember = false;
        }
        else
        {
            Party? party = player.getParty();
            if (!player.isInParty() || party == null || !party.Equals(effected.getParty()))
            {
                SystemMessagePacket sm =
                    new SystemMessagePacket(SystemMessageId.S1_CANNOT_BE_USED_THE_REQUIREMENTS_ARE_NOT_MET);

                sm.Params.addSkillName(skill);
                player.sendPacket(sm);
                isPartyMember = false;
            }
        }

        return value == isPartyMember;
    }
}