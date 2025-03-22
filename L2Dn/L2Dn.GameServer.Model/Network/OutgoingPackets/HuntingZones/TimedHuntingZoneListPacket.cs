using System.Collections.Immutable;
using L2Dn.GameServer.Dto;
using L2Dn.GameServer.InstanceManagers;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Variables;
using L2Dn.GameServer.Model.Zones;
using L2Dn.GameServer.StaticData;
using L2Dn.Packets;

namespace L2Dn.GameServer.Network.OutgoingPackets.HuntingZones;

public readonly struct TimedHuntingZoneListPacket: IOutgoingPacket
{
    private readonly Player _player;
    private readonly bool _isInTimedHuntingZone;

    public TimedHuntingZoneListPacket(Player player)
    {
        _player = player;
        _isInTimedHuntingZone = player.isInsideZone(ZoneId.TIMED_HUNTING);
    }

    public void WriteContent(PacketBitWriter writer)
    {
        writer.WritePacketCode(OutgoingPacketCodes.EX_TIME_RESTRICT_FIELD_LIST);

        ImmutableArray<TimedHuntingZoneHolder> huntingZones = TimedHuntingZoneData.Instance.HuntingZones;
        DateTime currentTime = DateTime.UtcNow;
        writer.WriteInt32(huntingZones.Length); // zone count
        foreach (TimedHuntingZoneHolder holder in huntingZones)
        {
            writer.WriteInt32(holder.EntryFee != 0); // required item count
            writer.WriteInt32(holder.EntryItemId);
            writer.WriteInt64(holder.EntryFee);
            writer.WriteInt32(!holder.IsWeekly); // reset cycle
            writer.WriteInt32(holder.ZoneId);
            writer.WriteInt32(holder.MinLevel);
            writer.WriteInt32(holder.MaxLevel);
            writer.WriteInt32((int)holder.InitialTime.TotalSeconds); // remain time base in seconds
            TimeSpan remainingTime = _player.getTimedHuntingZoneRemainingTime(holder.ZoneId);
            if (remainingTime == TimeSpan.Zero &&
                _player.getTimedHuntingZoneInitialEntry(holder.ZoneId) + holder.ResetDelay < currentTime)
            {
                remainingTime = holder.InitialTime; // in seconds
            }

            writer.WriteInt32((int)remainingTime.TotalSeconds); // remain time in seconds
            writer.WriteInt32((int)holder.MaximumAddedTime.TotalSeconds); // in seconds
            writer.WriteInt32(_player.getVariables().
                Get(PlayerVariables.HUNTING_ZONE_REMAIN_REFILL + holder.ZoneId,
                    (int)holder.RemainRefillTime.TotalSeconds));

            writer.WriteInt32((int)holder.RefillTimeMax.TotalSeconds); // in seconds
            bool isFieldActivated = !_isInTimedHuntingZone;
            if (holder.ZoneId == 18 && !GlobalVariablesManager.getInstance().Get("AvailableFrostLord", false))
            {
                isFieldActivated = false;
            }

            writer.WriteByte(isFieldActivated);
            writer.WriteByte(0); // bUserBound
            writer.WriteByte(0); // bCanReEnter
            writer.WriteByte(holder.PremiumUsersOnly); // bIsInZonePCCafeUserOnly
            writer.WriteByte(_player.hasPremiumStatus()); // bIsPCCafeUser
            writer.WriteByte(holder.UseWorldPrefix); // bWorldInZone
            writer.WriteByte(0); // bCanUseEntranceTicket
            writer.WriteInt32(0); // nEntranceCount
        }
    }
}