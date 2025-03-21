using L2Dn.GameServer.Enums;
using L2Dn.GameServer.Handlers;
using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Effects;
using L2Dn.GameServer.Model.InstanceZones;
using L2Dn.GameServer.Model.Items.Instances;
using L2Dn.GameServer.Model.Olympiads;
using L2Dn.GameServer.Model.Zones;
using L2Dn.GameServer.Network.Enums;
using L2Dn.GameServer.Network.OutgoingPackets;
using L2Dn.GameServer.Templates;
using L2Dn.Utilities;

namespace L2Dn.GameServer.Scripts.Handlers.EffectHandlers;

[HandlerStringKey("TeleportToPlayer")]
public sealed class TeleportToPlayer: AbstractEffect
{
    public override EffectTypes EffectTypes => EffectTypes.TELEPORT_TO_TARGET;

    public override bool IsInstant => true;

    public override void Instant(Creature effector, Creature effected, Skill skill, Item? item)
    {
        if (effector.getTarget() is Player target && effector.getTarget() != effector)
        {
            if (target.isAlikeDead())
            {
                SystemMessagePacket sm =
                    new SystemMessagePacket(SystemMessageId.
                        C1_IS_DEAD_AT_THE_MOMENT_AND_CANNOT_BE_SUMMONED_OR_TELEPORTED);

                sm.Params.addPcName(target);
                effector.sendPacket(sm);
                return;
            }

            if (target.isInStoreMode())
            {
                SystemMessagePacket sm = new SystemMessagePacket(SystemMessageId.
                    C1_IS_CURRENTLY_TRADING_OR_OPERATING_A_PRIVATE_STORE_AND_CANNOT_BE_SUMMONED_OR_TELEPORTED);

                sm.Params.addPcName(target);
                effector.sendPacket(sm);
                return;
            }

            if (target.isRooted() || target.isInCombat())
            {
                SystemMessagePacket sm =
                    new SystemMessagePacket(
                        SystemMessageId.C1_IS_ENGAGED_IN_COMBAT_AND_CANNOT_BE_SUMMONED_OR_TELEPORTED);

                sm.Params.addPcName(target);
                effector.sendPacket(sm);
                return;
            }

            if (target.isInOlympiadMode())
            {
                effector.sendPacket(SystemMessageId.
                    A_USER_PARTICIPATING_IN_THE_OLYMPIAD_CANNOT_USE_SUMMONING_OR_TELEPORTING);

                return;
            }

            if (target.isFlyingMounted() || target.isCombatFlagEquipped() || target.isInTraingCamp())
            {
                effector.sendPacket(SystemMessageId.YOU_CANNOT_USE_SUMMONING_OR_TELEPORTING_IN_THIS_AREA);
                return;
            }

            if (target.inObserverMode() || OlympiadManager.getInstance().isRegisteredInComp(target))
            {
                SystemMessagePacket sm =
                    new SystemMessagePacket(
                        SystemMessageId.C1_IS_IN_AN_AREA_WHERE_SUMMONING_OR_TELEPORTING_IS_BLOCKED_2);

                sm.Params.addString(target.getName());
                effector.sendPacket(sm);
                return;
            }

            if (target.isInsideZone(ZoneId.NO_SUMMON_FRIEND) || target.isInsideZone(ZoneId.JAIL))
            {
                SystemMessagePacket sm =
                    new SystemMessagePacket(SystemMessageId.C1_IS_IN_AN_AREA_WHERE_SUMMONING_OR_TELEPORTING_IS_BLOCKED);

                sm.Params.addString(target.getName());
                effector.sendPacket(sm);
                return;
            }

            Instance? instance = target.getInstanceWorld();
            if ((instance != null && !instance.isPlayerSummonAllowed()) || target.isInsideZone(ZoneId.TIMED_HUNTING))
            {
                SystemMessagePacket sm =
                    new SystemMessagePacket(SystemMessageId.C1_IS_IN_AN_AREA_WHERE_SUMMONING_OR_TELEPORTING_IS_BLOCKED);

                sm.Params.addString(target.getName());
                effector.sendPacket(sm);
                return;
            }

            effector.teleToLocation(target.Location, null, true);
        }
    }

    public override int GetHashCode() => this.GetSingletonHashCode();
    public override bool Equals(object? obj) => this.EqualsTo(obj);
}