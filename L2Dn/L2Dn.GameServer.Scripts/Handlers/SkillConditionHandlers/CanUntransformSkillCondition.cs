using L2Dn.GameServer.Handlers;
using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Skills;
using L2Dn.GameServer.Model.Zones;
using L2Dn.GameServer.Network.Enums;
using L2Dn.GameServer.Templates;

namespace L2Dn.GameServer.Scripts.Handlers.SkillConditionHandlers;

[HandlerName("CanUntransform")]
public sealed class CanUntransformSkillCondition: ISkillCondition
{
    public bool canUse(Creature caster, Skill skill, WorldObject? target)
    {
        bool canUntransform = true;
        Player? player = caster.getActingPlayer();
        if (player == null)
        {
            canUntransform = false;
        }
        else if (player.isAlikeDead() || player.isCursedWeaponEquipped())
        {
            canUntransform = false;
        }
        else if (player.isFlyingMounted() && !player.isInsideZone(ZoneId.LANDING))
        {
            player.sendPacket(SystemMessageId.
                YOU_ARE_TOO_HIGH_TO_PERFORM_THIS_ACTION_PLEASE_LOWER_YOUR_ALTITUDE_AND_TRY_AGAIN); // TODO: check if message is retail like.
            canUntransform = false;
        }

        return canUntransform;
    }
}