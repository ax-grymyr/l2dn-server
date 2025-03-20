using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Skills;
using L2Dn.GameServer.Network.Enums;
using L2Dn.GameServer.Templates;

namespace L2Dn.GameServer.Scripts.Handlers.SkillConditionHandlers;

public sealed class CanSummonSkillCondition: ISkillCondition
{
    public bool canUse(Creature caster, Skill skill, WorldObject? target)
    {
        Player? player = caster.getActingPlayer();
        if (player == null || player.isSpawnProtected() || player.isTeleportProtected())
        {
            return false;
        }

        bool canSummon = true;
        if (player.isFlyingMounted() || player.isMounted() || player.inObserverMode() || player.isTeleporting())
        {
            canSummon = false;
        }
        else if (player.isInAirShip())
        {
            player.sendPacket(SystemMessageId.YOU_CANNOT_SUMMON_A_SERVITOR_WHILE_MOUNTED);
            canSummon = false;
        }

        return canSummon;
    }
}