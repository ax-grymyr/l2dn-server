using System.Runtime.CompilerServices;
using L2Dn.GameServer.Data.Xml;
using L2Dn.GameServer.Enums;
using L2Dn.GameServer.Geo;
using L2Dn.GameServer.InstanceManagers;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Events;
using L2Dn.GameServer.Model.Events.Impl.Creatures.Players;
using L2Dn.GameServer.Model.Interfaces;
using L2Dn.GameServer.Model.ItemContainers;
using L2Dn.GameServer.Model.Items.Instances;
using L2Dn.GameServer.Model.Items.Types;
using L2Dn.GameServer.Model.Stats;
using L2Dn.GameServer.Model.Zones;
using L2Dn.GameServer.Model.Zones.Types;
using L2Dn.GameServer.Network.Enums;
using L2Dn.GameServer.Network.OutgoingPackets;
using L2Dn.GameServer.Utilities;
using NLog;
using ThreadPool = L2Dn.GameServer.Utilities.ThreadPool;

namespace L2Dn.GameServer.Model.Fishings;

public class Fishing
{
	protected static readonly Logger LOGGER = LogManager.GetLogger(nameof(Fishing));
	private ILocational _baitLocation = new Location(0, 0, 0);
	
	private readonly Player _player;
	private ScheduledFuture _reelInTask;
	private ScheduledFuture _startFishingTask;
	private bool _isFishing = false;
	
	public Fishing(Player player)
	{
		_player = player;
	}
	
	[MethodImpl(MethodImplOptions.Synchronized)]
	public bool isFishing()
	{
		return _isFishing;
	}
	
	public bool isAtValidLocation()
	{
		// TODO: implement checking direction
		// if (calculateBaitLocation() == null)
		// {
		// return false;
		// }
		return _player.isInsideZone(ZoneId.FISHING);
	}
	
	public bool canFish()
	{
		return !_player.isDead() && !_player.isAlikeDead() && !_player.hasBlockActions() && !_player.isSitting();
	}
	
	private FishingBait getCurrentBaitData()
	{
		Item bait = _player.getInventory().getPaperdollItem(Inventory.PAPERDOLL_LHAND);
		return bait != null ? FishingData.getInstance().getBaitData(bait.getId()) : null;
	}
	
	private void cancelTasks()
	{
		if (_reelInTask != null)
		{
			_reelInTask.cancel(false);
			_reelInTask = null;
		}
		
		if (_startFishingTask != null)
		{
			_startFishingTask.cancel(false);
			_startFishingTask = null;
		}
	}
	
	[MethodImpl(MethodImplOptions.Synchronized)]
	public void startFishing()
	{
		if (_isFishing)
		{
			return;
		}
		_isFishing = true;
		castLine();
	}
	
	private void castLine()
	{
		if (!Config.ALLOW_FISHING && !_player.canOverrideCond(PlayerCondOverride.ZONE_CONDITIONS))
		{
			_player.sendMessage("Fishing is disabled.");
			_player.sendPacket(ActionFailedPacket.STATIC_PACKET);
			stopFishing(FishingEndType.ERROR);
			return;
		}
		
		cancelTasks();
		
		if (!canFish())
		{
			if (_isFishing)
			{
				_player.sendPacket(SystemMessageId.YOUR_ATTEMPT_AT_FISHING_HAS_BEEN_CANCELLED);
			}
			stopFishing(FishingEndType.ERROR);
			return;
		}
		
		FishingBait baitData = getCurrentBaitData();
		if (baitData == null)
		{
			_player.sendPacket(SystemMessageId.YOU_MUST_PUT_BAIT_ON_YOUR_HOOK_BEFORE_YOU_CAN_FISH);
			_player.sendPacket(ActionFailedPacket.STATIC_PACKET);
			stopFishing(FishingEndType.ERROR);
			return;
		}
		
		if (Config.PREMIUM_SYSTEM_ENABLED)
		{
			if (Config.PREMIUM_ONLY_FISHING && !_player.hasPremiumStatus())
			{
				_player.sendPacket(SystemMessageId.YOU_CANNOT_FISH_AS_YOU_DO_NOT_MEET_THE_REQUIREMENTS);
				_player.sendPacket(ActionFailedPacket.STATIC_PACKET);
				stopFishing(FishingEndType.ERROR);
				return;
			}
			
			if (baitData.isPremiumOnly() && !_player.hasPremiumStatus())
			{
				_player.sendPacket(SystemMessageId.FAILED_PLEASE_TRY_AGAIN_USING_THE_CORRECT_BAIT);
				_player.sendPacket(ActionFailedPacket.STATIC_PACKET);
				stopFishing(FishingEndType.ERROR);
				return;
			}
		}
		
		int minPlayerLevel = baitData.getMinPlayerLevel();
		int maxPLayerLevel = baitData.getMaxPlayerLevel();
		if ((_player.getLevel() < minPlayerLevel) || (_player.getLevel() > maxPLayerLevel))
		{
			_player.sendPacket(SystemMessageId.YOU_DO_NOT_MEET_THE_FISHING_LEVEL_REQUIREMENTS);
			_player.sendPacket(ActionFailedPacket.STATIC_PACKET);
			stopFishing(FishingEndType.ERROR);
			return;
		}
		
		Item rod = _player.getActiveWeaponInstance();
		if ((rod == null) || (rod.getItemType() != WeaponType.FISHINGROD))
		{
			_player.sendPacket(SystemMessageId.YOU_DON_T_HAVE_A_FISHING_ROD_EQUIPPED);
			_player.sendPacket(ActionFailedPacket.STATIC_PACKET);
			stopFishing(FishingEndType.ERROR);
			return;
		}
		
		FishingRod rodData = FishingData.getInstance().getRodData(rod.getId());
		if (rodData == null)
		{
			_player.sendPacket(SystemMessageId.YOU_DON_T_HAVE_A_FISHING_ROD_EQUIPPED);
			_player.sendPacket(ActionFailedPacket.STATIC_PACKET);
			stopFishing(FishingEndType.ERROR);
			return;
		}
		
		if (_player.isTransformed() || _player.isInBoat())
		{
			_player.sendPacket(SystemMessageId.YOU_CANNOT_FISH_WHILE_RIDING_AS_A_PASSENGER_OF_A_BOAT_OR_TRANSFORMED);
			_player.sendPacket(ActionFailedPacket.STATIC_PACKET);
			stopFishing(FishingEndType.ERROR);
			return;
		}
		
		if (_player.isCrafting() || _player.isInStoreMode())
		{
			_player.sendPacket(SystemMessageId.YOU_CANNOT_FISH_WHILE_USING_A_RECIPE_BOOK_PRIVATE_WORKSHOP_OR_PRIVATE_STORE);
			_player.sendPacket(ActionFailedPacket.STATIC_PACKET);
			stopFishing(FishingEndType.ERROR);
			return;
		}
		
		if (_player.isInsideZone(ZoneId.WATER) || _player.isInWater())
		{
			_player.sendPacket(SystemMessageId.YOU_CANNOT_FISH_WHILE_UNDER_WATER);
			_player.sendPacket(ActionFailedPacket.STATIC_PACKET);
			stopFishing(FishingEndType.ERROR);
			return;
		}
		
		_baitLocation = calculateBaitLocation();
		if (!_player.isInsideZone(ZoneId.FISHING) || (_baitLocation == null))
		{
			if (_isFishing)
			{
				// _player.sendPacket(SystemMessageId.YOUR_ATTEMPT_AT_FISHING_HAS_BEEN_CANCELLED);
				_player.sendPacket(ActionFailedPacket.STATIC_PACKET);
			}
			else
			{
				_player.sendPacket(SystemMessageId.YOU_CAN_T_FISH_HERE);
				_player.sendPacket(ActionFailedPacket.STATIC_PACKET);
			}
			stopFishing(FishingEndType.ERROR);
			return;
		}
		
		if (!_player.isChargedShot(ShotType.FISH_SOULSHOTS))
		{
			_player.rechargeShots(false, false, true);
		}
		
		long fishingTime = Math.Max(Rnd.get(baitData.getTimeMin(), baitData.getTimeMax()) - rodData.getReduceFishingTime(), 1000);
		long fishingWaitTime = Rnd.get(baitData.getWaitMin(), baitData.getWaitMax());
		_reelInTask = ThreadPool.schedule(() =>
		{
			_player.getFishing().reelInWithReward();
			_startFishingTask = ThreadPool.schedule(() => _player.getFishing().castLine(), fishingWaitTime);
		}, fishingTime);
		_player.stopMove(null);
		_player.broadcastPacket(new ExFishingStart(_player, -1, _baitLocation));
		_player.sendPacket(new ExUserInfoFishing(_player, true, _baitLocation));
		_player.sendPacket(new PlaySoundPacket(1, "sf_p_01", 0, 0, 0, 0, 0));
		_player.sendPacket(SystemMessageId.YOU_CAST_YOUR_LINE_AND_START_TO_FISH);
	}
	
	public void reelInWithReward()
	{
		// Fish may or may not eat the hook. If it does - it consumes fishing bait and fishing shot.
		// Then player may or may not catch the fish. Using fishing shots increases chance to win.
		FishingBait baitData = getCurrentBaitData();
		if (baitData == null)
		{
			reelIn(FishingEndReason.LOSE, false);
			LOGGER.Warn("Player " + _player + " is fishing with unhandled bait: " + _player.getInventory().getPaperdollItem(Inventory.PAPERDOLL_LHAND));
			return;
		}
		
		double chance = baitData.getChance();
		if (_player.isChargedShot(ShotType.FISH_SOULSHOTS))
		{
			chance *= 2;
		}
		
		if (Rnd.get(100) <= chance)
		{
			reelIn(FishingEndReason.WIN, true);
		}
		else
		{
			reelIn(FishingEndReason.LOSE, true);
		}
	}
	
	private void reelIn(FishingEndReason reasonValue, bool consumeBait)
	{
		if (!_isFishing)
		{
			return;
		}
		
		cancelTasks();
		
		FishingEndReason reason = reasonValue;
		try
		{
			Item bait = _player.getInventory().getPaperdollItem(Inventory.PAPERDOLL_LHAND);
			if (consumeBait && ((bait == null) || !_player.getInventory().updateItemCount(null, bait, -1, _player, null)))
			{
				reason = FishingEndReason.LOSE; // no bait - no reward
				return;
			}
			
			if ((reason == FishingEndReason.WIN) && (bait != null))
			{
				FishingBait baitData = FishingData.getInstance().getBaitData(bait.getId());
				FishingCatch fishingCatchData = baitData.getRandom();
				if (fishingCatchData != null)
				{
					FishingData fishingData = FishingData.getInstance();
					double lvlModifier = (Math.Pow(_player.getLevel(), 2.2) * fishingCatchData.getMultiplier());
					long xp = (long) (Rnd.get(fishingData.getExpRateMin(), fishingData.getExpRateMax()) * lvlModifier * _player.getStat().getMul(Stat.FISHING_EXP_SP_BONUS, 1));
					long sp = (long) (Rnd.get(fishingData.getSpRateMin(), fishingData.getSpRateMax()) * lvlModifier * _player.getStat().getMul(Stat.FISHING_EXP_SP_BONUS, 1));
					_player.addExpAndSp(xp, sp, true);
					_player.getInventory().addItem("Fishing Reward", fishingCatchData.getItemId(), 1, _player, null);
					SystemMessagePacket msg = new SystemMessagePacket(SystemMessageId.YOU_HAVE_ACQUIRED_S1);
					msg.Params.addItemName(fishingCatchData.getItemId());
					_player.sendPacket(msg);
					_player.unchargeShot(ShotType.FISH_SOULSHOTS);
					_player.rechargeShots(false, false, true);
				}
				else
				{
					LOGGER.Warn("Could not find fishing rewards for bait " + bait.getId());
				}
			}
			else if (reason == FishingEndReason.LOSE)
			{
				_player.sendPacket(SystemMessageId.THE_BAIT_HAS_BEEN_LOST_BECAUSE_THE_FISH_GOT_AWAY);
			}
			
			if (consumeBait && EventDispatcher.getInstance().hasListener(EventType.ON_PLAYER_FISHING, _player))
			{
				EventDispatcher.getInstance().notifyEventAsync(new OnPlayerFishing(_player, reason), _player);
			}
		}
		finally
		{
			_player.broadcastPacket(new ExFishingEnd(_player, reason));
			_player.sendPacket(new ExUserInfoFishing(_player, false));
		}
	}
	
	public void stopFishing()
	{
		stopFishing(FishingEndType.PLAYER_STOP);
	}
	
	[MethodImpl(MethodImplOptions.Synchronized)]
	public void stopFishing(FishingEndType endType)
	{
		if (_isFishing)
		{
			reelIn(FishingEndReason.STOP, false);
			_isFishing = false;
			switch (endType)
			{
				case FishingEndType.PLAYER_STOP:
				{
					_player.sendPacket(SystemMessageId.YOU_REEL_YOUR_LINE_IN_AND_STOP_FISHING);
					break;
				}
				case FishingEndType.PLAYER_CANCEL:
				{
					_player.sendPacket(SystemMessageId.YOUR_ATTEMPT_AT_FISHING_HAS_BEEN_CANCELLED);
					break;
				}
			}
		}
	}
	
	public ILocational getBaitLocation()
	{
		return _baitLocation;
	}
	
	private Location calculateBaitLocation()
	{
		// calculate a position in front of the player with a random distance
		int distMin = FishingData.getInstance().getBaitDistanceMin();
		int distMax = FishingData.getInstance().getBaitDistanceMax();
		int distance = Rnd.get(distMin, distMax);
		double angle = Util.convertHeadingToDegree(_player.getHeading());
		double radian = MathUtil.toRadians(angle);
		double sin = Math.Sin(radian);
		double cos = Math.Cos(radian);
		int baitX = (int) (_player.getX() + (cos * distance));
		int baitY = (int) (_player.getY() + (sin * distance));
		
		// search for fishing zone
		FishingZone fishingZone = null;
		foreach (ZoneType zone in ZoneManager.getInstance().getZones(_player))
		{
			if (zone is FishingZone)
			{
				fishingZone = (FishingZone) zone;
				break;
			}
		}
		// search for water zone
		WaterZone waterZone = null;
		foreach (ZoneType zone in ZoneManager.getInstance().getZones(baitX, baitY))
		{
			if (zone is WaterZone)
			{
				waterZone = (WaterZone) zone;
				break;
			}
		}
		
		int baitZ = computeBaitZ(_player, baitX, baitY, fishingZone, waterZone);
		if (baitZ == int.MinValue)
		{
			_player.sendPacket(SystemMessageId.YOU_CAN_T_FISH_HERE);
			return null;
		}
		
		return new Location(baitX, baitY, baitZ);
	}
	
	/**
	 * Computes the Z of the bait.
	 * @param player the player
	 * @param baitX the bait x
	 * @param baitY the bait y
	 * @param fishingZone the fishing zone
	 * @param waterZone the water zone
	 * @return the bait z or {@link Integer#MIN_VALUE} when you cannot fish here
	 */
	private static int computeBaitZ(Player player, int baitX, int baitY, FishingZone fishingZone, WaterZone waterZone)
	{
		if ((fishingZone == null))
		{
			return int.MinValue;
		}
		
		if ((waterZone == null))
		{
			return int.MinValue;
		}
		
		// always use water zone, fishing zone high z is high in the air...
		int baitZ = waterZone.getWaterZ();
		
		// if (!GeoEngine.getInstance().canSeeTarget(player.getX(), player.getY(), player.getZ(), baitX, baitY, baitZ))
		//
		// return Integer.MIN_VALUE;
		// }
		if (GeoEngine.getInstance().hasGeo(baitX, baitY))
		{
			if (GeoEngine.getInstance().getHeight(baitX, baitY, baitZ) > baitZ)
			{
				return int.MinValue;
			}
			
			if (GeoEngine.getInstance().getHeight(baitX, baitY, player.getZ()) > baitZ)
			{
				return int.MinValue;
			}
		}
		
		return baitZ;
	}
}