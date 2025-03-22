using L2Dn.GameServer.Data.Xml;
using L2Dn.GameServer.Dto;
using L2Dn.GameServer.Handlers;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Effects;
using L2Dn.GameServer.Model.Holders;
using L2Dn.GameServer.Model.Items.Instances;
using L2Dn.GameServer.Model.Variables;
using L2Dn.GameServer.Network.OutgoingPackets.HuntingZones;
using L2Dn.GameServer.StaticData;
using L2Dn.GameServer.StaticData.Xml.Skills;
using L2Dn.GameServer.Templates;
using L2Dn.Utilities;

namespace L2Dn.GameServer.Scripts.Handlers.EffectHandlers;

[HandlerStringKey("AddHuntingTime")]
public sealed class AddHuntingTime: AbstractEffect
{
    private readonly int _zoneId;
    private readonly TimeSpan _time;

    public AddHuntingTime(EffectParameterSet parameters)
    {
        _zoneId = parameters.GetInt32(XmlSkillEffectParameterType.ZoneId, 0);
        _time = parameters.GetTimeSpanMilliSeconds(XmlSkillEffectParameterType.Time, TimeSpan.FromHours(1));
    }

    public override bool IsInstant => true;

    public override void Instant(Creature effector, Creature effected, Skill skill, Item? item)
    {
        if (item == null)
            return;

        Player? player = effected.getActingPlayer();
        if (player == null)
            return;

        TimedHuntingZoneHolder? holder = TimedHuntingZoneData.Instance.GetHuntingZone(_zoneId);
        if (holder == null)
            return;

        DateTime currentTime = DateTime.UtcNow;
        DateTime endTime = currentTime + player.getTimedHuntingZoneRemainingTime(_zoneId);
        if (endTime > currentTime &&
            endTime - currentTime + _time > holder.MaximumAddedTime)
        {
            player.getInventory().addItem("AddHuntingTime effect refund", item.Id, 1, player, player);
            player.sendMessage("You cannot exceed the time zone limit.");
            return;
        }

        TimeSpan remainRefill = player.getVariables().
            Get(PlayerVariables.HUNTING_ZONE_REMAIN_REFILL + _zoneId, holder.RemainRefillTime);

        if (_time < remainRefill || remainRefill == TimeSpan.Zero)
        {
            player.getInventory().addItem("AddHuntingTime effect refund", item.Id, 1, player, player);
            player.sendMessage("You cannot exceed the time zone limit.");
            return;
        }

        TimeSpan remainTime = player.getVariables().
            Get(PlayerVariables.HUNTING_ZONE_TIME + _zoneId, holder.InitialTime);

        player.getVariables().Set(PlayerVariables.HUNTING_ZONE_TIME + _zoneId, remainTime + _time);
        player.getVariables().Set(PlayerVariables.HUNTING_ZONE_REMAIN_REFILL + _zoneId, remainRefill - _time);

        player.sendPacket(
            new TimedHuntingZoneChargeResultPacket(_zoneId, remainTime + _time, remainRefill - _time, _time));

        if (player.isInTimedHuntingZone(_zoneId))
        {
            player.startTimedHuntingZone(_zoneId, endTime);
            player.sendPacket(new TimeRestrictFieldUserAlarmPacket(player, _zoneId));
        }
    }

    public override int GetHashCode() => HashCode.Combine(_zoneId, _time);
    public override bool Equals(object? obj) => this.EqualsTo(obj, static x => (x._zoneId, x._time));
}