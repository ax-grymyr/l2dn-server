using L2Dn.GameServer.Data.Xml;
using L2Dn.GameServer.InstanceManagers;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Holders;
using L2Dn.GameServer.Model.Variables;
using L2Dn.GameServer.Model.Zones;
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
		
		DateTime currentTime = DateTime.UtcNow;
		writer.WriteInt32(TimedHuntingZoneData.getInstance().getSize()); // zone count
		foreach (TimedHuntingZoneHolder holder in TimedHuntingZoneData.getInstance().getAllHuntingZones())
		{
			writer.WriteInt32(holder.getEntryFee() != 0); // required item count
			writer.WriteInt32(holder.getEntryItemId());
			writer.WriteInt64(holder.getEntryFee());
			writer.WriteInt32(!holder.isWeekly()); // reset cycle
			writer.WriteInt32(holder.getZoneId());
			writer.WriteInt32(holder.getMinLevel());
			writer.WriteInt32(holder.getMaxLevel());
			writer.WriteInt32(holder.getInitialTime() / 1000); // remain time base
			int remainingTime = _player.getTimedHuntingZoneRemainingTime(holder.getZoneId());
			if ((remainingTime == 0) && ((_player.getTimedHuntingZoneInitialEntry(holder.getZoneId()) + holder.getResetDelay()) < currentTime))
			{
				remainingTime = holder.getInitialTime();
			}
			writer.WriteInt32(remainingTime / 1000); // remain time
			writer.WriteInt32(holder.getMaximumAddedTime() / 1000);
			writer.WriteInt32(_player.getVariables().getInt(PlayerVariables.HUNTING_ZONE_REMAIN_REFILL + holder.getZoneId(), holder.getRemainRefillTime()));
			writer.WriteInt32(holder.getRefillTimeMax());
			bool isFieldActivated = !_isInTimedHuntingZone;
			if ((holder.getZoneId() == 18) && !GlobalVariablesManager.getInstance().getBoolean("AvailableFrostLord", false))
			{
				isFieldActivated = false;
			}
			
			writer.WriteByte(isFieldActivated);
			writer.WriteByte(0); // bUserBound
			writer.WriteByte(0); // bCanReEnter
			writer.WriteByte(holder.zonePremiumUserOnly()); // bIsInZonePCCafeUserOnly
			writer.WriteByte(_player.hasPremiumStatus()); // bIsPCCafeUser
			writer.WriteByte(holder.useWorldPrefix()); // bWorldInZone
			writer.WriteByte(0); // bCanUseEntranceTicket
			writer.WriteInt32(0); // nEntranceCount
		}
	}
}