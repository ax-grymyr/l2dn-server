using L2Dn.GameServer.Data.Xml;
using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Effects;
using L2Dn.GameServer.Model.Holders;
using L2Dn.GameServer.Model.Items.Instances;
using L2Dn.GameServer.Model.Skills;
using L2Dn.GameServer.Model.Variables;
using L2Dn.GameServer.Network.OutgoingPackets.HuntingZones;

namespace L2Dn.GameServer.Scripts.Handlers.EffectHandlers;

/**
 * @author Mobius
 */
public class AddHuntingTime: AbstractEffect
{
	private readonly int _zoneId;
	private readonly TimeSpan _time;
	
	public AddHuntingTime(StatSet @params)
	{
		_zoneId = @params.getInt("zoneId", 0);
		_time = TimeSpan.FromMilliseconds(@params.getLong("time", 3600000));
	}
	
	public override bool isInstant()
	{
		return true;
	}
	
	public override void instant(Creature effector, Creature effected, Skill skill, Item item)
	{
		Player player = effected.getActingPlayer();
		if (player == null)
		{
			return;
		}
		
		TimedHuntingZoneHolder holder = TimedHuntingZoneData.getInstance().getHuntingZone(_zoneId);
		if (holder == null)
		{
			return;
		}
		
		DateTime currentTime = DateTime.UtcNow;
		DateTime endTime = currentTime + TimeSpan.FromMilliseconds(player.getTimedHuntingZoneRemainingTime(_zoneId));
		if ((endTime > currentTime) && (((endTime - currentTime) + _time) > TimeSpan.FromMilliseconds(holder.getMaximumAddedTime())))
		{
			player.getInventory().addItem("AddHuntingTime effect refund", item.getId(), 1, player, player);
			player.sendMessage("You cannot exceed the time zone limit.");
			return;
		}
		
		long remainRefill = player.getVariables().getInt(PlayerVariables.HUNTING_ZONE_REMAIN_REFILL + _zoneId, holder.getRemainRefillTime());
		if ((_time < TimeSpan.FromMilliseconds(remainRefill)) || (remainRefill == 0))
		{
			player.getInventory().addItem("AddHuntingTime effect refund", item.getId(), 1, player, player);
			player.sendMessage("You cannot exceed the time zone limit.");
			return;
		}
		
		long remainTime = player.getVariables().getLong(PlayerVariables.HUNTING_ZONE_TIME + _zoneId, holder.getInitialTime());
		player.getVariables().set(PlayerVariables.HUNTING_ZONE_TIME + _zoneId, remainTime + (int)_time.TotalMilliseconds);
		player.getVariables().set(PlayerVariables.HUNTING_ZONE_REMAIN_REFILL + _zoneId, remainRefill - (int)(_time.TotalMilliseconds / 1000));
		player.sendPacket(new TimedHuntingZoneChargeResultPacket(_zoneId, (int) ((remainTime + _time.TotalMilliseconds) / 1000), (int) (remainRefill - (_time.TotalMilliseconds / 1000)), (int) _time.TotalMilliseconds / 1000));
		
		if (player.isInTimedHuntingZone(_zoneId))
		{
			player.startTimedHuntingZone(_zoneId, endTime);
			player.sendPacket(new TimeRestrictFieldUserAlarmPacket(player, _zoneId));
		}
	}
}