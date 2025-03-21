using L2Dn.GameServer.Enums;
using L2Dn.GameServer.Handlers;
using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Effects;
using L2Dn.GameServer.Model.Holders;
using L2Dn.GameServer.Model.InstanceZones;
using L2Dn.GameServer.Model.Items.Instances;
using L2Dn.GameServer.Model.Olympiads;
using L2Dn.GameServer.Model.Zones;
using L2Dn.GameServer.Network.Enums;
using L2Dn.GameServer.Network.OutgoingPackets;
using L2Dn.GameServer.StaticData.Xml.Skills;
using L2Dn.GameServer.Templates;
using L2Dn.Geometry;
using L2Dn.Utilities;

namespace L2Dn.GameServer.Scripts.Handlers.EffectHandlers;

/// <summary>
/// Call Pc effect implementation.
/// </summary>
[HandlerStringKey("CallPc")]
public sealed class CallPc: AbstractEffect
{
    private readonly int _itemId;
    private readonly int _itemCount;

    public CallPc(EffectParameterSet parameters)
    {
        _itemId = parameters.GetInt32(XmlSkillEffectParameterType.ItemId, 0);
        _itemCount = parameters.GetInt32(XmlSkillEffectParameterType.ItemCount, 0);
    }

    public override bool IsInstant => true;

    public override void Instant(Creature effector, Creature effected, Skill skill, Item? item)
    {
        if (effector == effected)
        {
            return;
        }

        Player? player = effector.getActingPlayer();
        Player? target = effected.getActingPlayer();
        if (player != null && target != null)
        {
            if (CheckSummonTargetStatus(target, player))
            {
                if (_itemId != 0 && _itemCount != 0)
                {
                    SystemMessagePacket sm;
                    if (target.getInventory().getInventoryItemCount(_itemId, 0) < _itemCount)
                    {
                        sm = new SystemMessagePacket(SystemMessageId.S1_IS_REQUIRED_FOR_SUMMONING);
                        sm.Params.addItemName(_itemId);
                        target.sendPacket(sm);
                        return;
                    }

                    target.getInventory().destroyItemByItemId("Consume", _itemId, _itemCount, player, target);
                    sm = new SystemMessagePacket(SystemMessageId.S1_DISAPPEARED);
                    sm.Params.addItemName(_itemId);
                    target.sendPacket(sm);
                }

                target.addScript(new SummonRequestHolder(player));

                ConfirmDialogPacket confirm = new(30000, player.ObjectId,
                    SystemMessageId.C1_WANTS_TO_SUMMON_YOU_TO_S2_ACCEPT);

                confirm.Params.addString(player.getName());
                confirm.Params.addZoneName(player.getX(), player.getY(), player.getZ());
                target.sendPacket(confirm);
            }
        }
        else if (target != null)
        {
            if (skill.TargetType == TargetType.ENEMY)
            {
                effected.abortCast();
                effected.abortAttack();
                effected.stopMove(null);
                effected.sendPacket(new FlyToLocationPacket(effected,
                    new Location3D(effector.getX(), effector.getY(), effector.getZ()), FlyType.DUMMY));

                effected.setLocation(effector.Location);
            }
            else
            {
                WorldObject? previousTarget = target.getTarget();
                target.teleToLocation(effector.Location);
                target.setTarget(previousTarget);
            }
        }
    }

    public static bool CheckSummonTargetStatus(Player target, Player effector)
    {
        if (target == effector)
            return false;

        if (target.isAlikeDead())
        {
            SystemMessagePacket sm =
                new SystemMessagePacket(SystemMessageId.C1_IS_DEAD_AT_THE_MOMENT_AND_CANNOT_BE_SUMMONED_OR_TELEPORTED);

            sm.Params.addPcName(target);
            effector.sendPacket(sm);
            return false;
        }

        if (target.isInStoreMode())
        {
            SystemMessagePacket sm = new SystemMessagePacket(SystemMessageId.
                C1_IS_CURRENTLY_TRADING_OR_OPERATING_A_PRIVATE_STORE_AND_CANNOT_BE_SUMMONED_OR_TELEPORTED);

            sm.Params.addPcName(target);
            effector.sendPacket(sm);
            return false;
        }

        if (target.isRooted() || target.isInCombat())
        {
            SystemMessagePacket sm =
                new SystemMessagePacket(SystemMessageId.C1_IS_ENGAGED_IN_COMBAT_AND_CANNOT_BE_SUMMONED_OR_TELEPORTED);

            sm.Params.addPcName(target);
            effector.sendPacket(sm);
            return false;
        }

        if (target.isInOlympiadMode())
        {
            effector.sendPacket(
                SystemMessageId.A_USER_PARTICIPATING_IN_THE_OLYMPIAD_CANNOT_USE_SUMMONING_OR_TELEPORTING);

            return false;
        }

        if (target.isOnEvent() || target.isFlyingMounted() || target.isCombatFlagEquipped() ||
            target.isInTraingCamp() || target.isInsideZone(ZoneId.TIMED_HUNTING) ||
            effector.isInsideZone(ZoneId.TIMED_HUNTING))
        {
            effector.sendPacket(SystemMessageId.YOU_CANNOT_USE_SUMMONING_OR_TELEPORTING_IN_THIS_AREA);
            return false;
        }

        if (target.inObserverMode() || OlympiadManager.getInstance().isRegisteredInComp(target))
        {
            SystemMessagePacket sm =
                new SystemMessagePacket(SystemMessageId.C1_IS_IN_AN_AREA_WHERE_SUMMONING_OR_TELEPORTING_IS_BLOCKED_2);

            sm.Params.addString(target.getName());
            effector.sendPacket(sm);
            return false;
        }

        if (target.isInsideZone(ZoneId.NO_SUMMON_FRIEND) || target.isInsideZone(ZoneId.JAIL))
        {
            SystemMessagePacket sm =
                new SystemMessagePacket(SystemMessageId.C1_IS_IN_AN_AREA_WHERE_SUMMONING_OR_TELEPORTING_IS_BLOCKED);

            sm.Params.addString(target.getName());
            effector.sendPacket(sm);
            return false;
        }

        Instance? instance = effector.getInstanceWorld();
        if (instance != null && !instance.isPlayerSummonAllowed())
        {
            effector.sendPacket(SystemMessageId.CANNOT_BE_SUMMONED_IN_THIS_LOCATION);
            return false;
        }

        return true;
    }

    public override int GetHashCode() => HashCode.Combine(_itemId, _itemCount);
    public override bool Equals(object? obj) => this.EqualsTo(obj, static x => (x._itemId, x._itemCount));
}