using L2Dn.GameServer.Data.Sql;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Items;
using L2Dn.GameServer.Model.Skills;
using L2Dn.GameServer.Network.Enums;
using L2Dn.GameServer.Templates;
using Config = L2Dn.GameServer.Configuration.Config;

namespace L2Dn.GameServer.Model.Conditions;

/**
 * Player Can Summon condition implementation.
 * @author Zoey76
 */
public sealed class ConditionPlayerCanSummonPet(bool value): Condition
{
    protected override bool TestImpl(Creature effector, Creature? effected, Skill? skill, ItemTemplate? item)
    {
        Player? player = effector.getActingPlayer();
        if (player is null)
            return false;

        bool canSummon = true;
        if (Config.Character.RESTORE_PET_ON_RECONNECT && CharSummonTable.getInstance().getPets().ContainsKey(player.ObjectId))
        {
            player.sendPacket(SystemMessageId.YOU_MAY_NOT_SUMMON_MULTIPLE_PETS_AT_THE_SAME_TIME);
            canSummon = false;
        }
        else if (player.hasPet())
        {
            player.sendPacket(SystemMessageId.YOU_MAY_NOT_SUMMON_MULTIPLE_PETS_AT_THE_SAME_TIME);
            canSummon = false;
        }
        else if (player.isFlyingMounted() || player.isMounted() || player.inObserverMode() || player.isTeleporting())
            canSummon = false;

        return value == canSummon;
    }
}