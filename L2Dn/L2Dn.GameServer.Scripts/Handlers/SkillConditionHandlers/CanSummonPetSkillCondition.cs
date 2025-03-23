using L2Dn.GameServer.Data.Sql;
using L2Dn.GameServer.Enums;
using L2Dn.GameServer.Handlers;
using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Skills;
using L2Dn.GameServer.Network.Enums;
using L2Dn.GameServer.TaskManagers;
using L2Dn.GameServer.Templates;
using L2Dn.Utilities;
using Config = L2Dn.GameServer.Configuration.Config;

namespace L2Dn.GameServer.Scripts.Handlers.SkillConditionHandlers;

[HandlerStringKey("CanSummonPet")]
public sealed class CanSummonPetSkillCondition: ISkillCondition
{
    public bool canUse(Creature caster, Skill skill, WorldObject? target)
    {
        Player? player = caster.getActingPlayer();
        if (player == null || player.isSpawnProtected() || player.isTeleportProtected())
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
        else if (player.getActiveTradeList() != null || player.hasItemRequest() ||
                 player.getPrivateStoreType() != PrivateStoreType.NONE)
        {
            player.sendPacket(SystemMessageId.CANNOT_BE_SUMMONED_WHILE_TRADING);
            canSummon = false;
        }
        else if (AttackStanceTaskManager.getInstance().hasAttackStanceTask(player))
        {
            player.sendPacket(SystemMessageId.CANNOT_BE_SUMMONED_WHILE_IN_COMBAT);
            canSummon = false;
        }
        else if (player.isInAirShip())
        {
            player.sendPacket(SystemMessageId.YOU_CANNOT_SUMMON_A_SERVITOR_WHILE_MOUNTED);
            canSummon = false;
        }
        else if (player.isFlyingMounted() || player.isMounted() || player.inObserverMode() || player.isTeleporting())
        {
            canSummon = false;
        }

        return canSummon;
    }

    public override int GetHashCode() => this.GetSingletonHashCode();
    public override bool Equals(object? obj) => this.EqualsTo(obj);
}