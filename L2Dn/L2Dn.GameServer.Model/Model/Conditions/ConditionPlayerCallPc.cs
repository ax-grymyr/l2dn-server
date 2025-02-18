using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Items;
using L2Dn.GameServer.Model.Skills;
using L2Dn.GameServer.Model.Zones;
using L2Dn.GameServer.Network.Enums;

namespace L2Dn.GameServer.Model.Conditions;

/**
 * Player Call Pc condition implementation.
 * @author Adry_85
 */
public sealed class ConditionPlayerCallPc(bool value): Condition
{
    protected override bool TestImpl(Creature effector, Creature effected, Skill? skill, ItemTemplate? item)
    {
        bool canCallPlayer = true;
        Player? player = effector.getActingPlayer();

        if (player is null)
            canCallPlayer = false;
        else if (player.isInOlympiadMode())
        {
            player.sendPacket(SystemMessageId.CANNOT_BE_SUMMONED_IN_THIS_LOCATION);
            canCallPlayer = false;
        }
        else if (player.inObserverMode())
        {
            canCallPlayer = false;
        }
        else if (player.isInsideZone(ZoneId.NO_SUMMON_FRIEND) || player.isInsideZone(ZoneId.JAIL) ||
                 player.isFlyingMounted())
        {
            player.sendPacket(SystemMessageId.YOU_CANNOT_USE_SUMMONING_OR_TELEPORTING_IN_THIS_AREA);
            canCallPlayer = false;
        }

        return value == canCallPlayer;
    }
}