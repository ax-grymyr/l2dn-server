using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Skills;
using L2Dn.GameServer.Network.Enums;
using L2Dn.GameServer.Network.OutgoingPackets;

namespace L2Dn.GameServer.Scripts.Handlers.SkillConditionHandlers;

/**
 * @author Sdw
 */
public class OpResurrectionSkillCondition: ISkillCondition
{
    public OpResurrectionSkillCondition(StatSet @params)
    {
    }

    public bool canUse(Creature caster, Skill skill, WorldObject? target)
    {
        bool canResurrect = true;
        if (target == caster)
        {
            return canResurrect;
        }

        if (target == null)
        {
            return false;
        }

        Player? player = target.getActingPlayer();
        if (target.isPlayer() && player != null)
        {
            if (!player.isDead())
            {
                canResurrect = false;
                if (caster.isPlayer())
                {
                    SystemMessagePacket msg =
                        new SystemMessagePacket(SystemMessageId.S1_CANNOT_BE_USED_THE_REQUIREMENTS_ARE_NOT_MET);
                    msg.Params.addSkillName(skill);
                    caster.sendPacket(msg);
                }
            }
            else if (player.isResurrectionBlocked())
            {
                canResurrect = false;
                if (caster.isPlayer())
                {
                    caster.sendPacket(SystemMessageId.REJECT_RESURRECTION);
                }
            }
            else if (player.isReviveRequested())
            {
                canResurrect = false;
                if (caster.isPlayer())
                {
                    caster.sendPacket(SystemMessageId.RESURRECTION_HAS_ALREADY_BEEN_PROPOSED);
                }
            }
        }
        else if (target.isSummon())
        {
            Summon summon = (Summon)target;
            if (!summon.isDead())
            {
                canResurrect = false;
                if (caster.isPlayer())
                {
                    SystemMessagePacket msg =
                        new SystemMessagePacket(SystemMessageId.S1_CANNOT_BE_USED_THE_REQUIREMENTS_ARE_NOT_MET);
                    msg.Params.addSkillName(skill);
                    caster.sendPacket(msg);
                }
            }
            else if (summon.isResurrectionBlocked())
            {
                canResurrect = false;
                if (caster.isPlayer())
                {
                    caster.sendPacket(SystemMessageId.REJECT_RESURRECTION);
                }
            }
            else if ((player != null) && player.isRevivingPet())
            {
                canResurrect = false;
                if (caster.isPlayer())
                {
                    caster.sendPacket(SystemMessageId.RESURRECTION_HAS_ALREADY_BEEN_PROPOSED);
                }
            }
        }

        return canResurrect;
    }
}