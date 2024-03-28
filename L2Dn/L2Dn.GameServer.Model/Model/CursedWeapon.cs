using L2Dn.GameServer.Data.Xml;
using L2Dn.GameServer.Db;
using L2Dn.GameServer.Enums;
using L2Dn.GameServer.InstanceManagers;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Interfaces;
using L2Dn.GameServer.Model.Items;
using L2Dn.GameServer.Model.Items.Instances;
using L2Dn.GameServer.Model.Skills;
using L2Dn.GameServer.Network.Enums;
using L2Dn.GameServer.Network.OutgoingPackets;
using L2Dn.GameServer.Utilities;
using Microsoft.EntityFrameworkCore;
using NLog;
using ThreadPool = L2Dn.GameServer.Utilities.ThreadPool;

namespace L2Dn.GameServer.Model;

public class CursedWeapon : INamable
{
	private static readonly Logger LOGGER = LogManager.GetLogger(nameof(CursedWeapon));
	
	// _name is the name of the cursed weapon associated with its ID.
	private readonly String _name;
	// _itemId is the Item ID of the cursed weapon.
	private readonly int _itemId;
	// _skillId is the skills ID.
	private readonly int _skillId;
	private readonly int _skillMaxLevel;
	private int _dropRate;
	private int _duration;
	private int _durationLost;
	private int _disapearChance;
	private int _stageKills;
	
	// this should be false unless if the cursed weapon is dropped, in that case it would be true.
	private bool _isDropped = false;
	// this sets the cursed weapon status to true only if a player has the cursed weapon, otherwise this should be false.
	private bool _isActivated = false;
	private ScheduledFuture _removeTask;
	
	private int _nbKills = 0;
	private DateTime _endTime;
	
	private int _playerId = 0;
	protected Player _player = null;
	private Item _item = null;
	private int _playerReputation = 0;
	private int _playerPkKills = 0;
	protected int transformationId = 0;
	
	public CursedWeapon(int itemId, int skillId, String name)
	{
		_name = name;
		_itemId = itemId;
		_skillId = skillId;
		_skillMaxLevel = SkillData.getInstance().getMaxLevel(_skillId);
	}
	
	public void endOfLife()
	{
		if (_isActivated)
		{
			if ((_player != null) && _player.isOnline())
			{
				// Remove from player
				LOGGER.Info(_name + " being removed online.");
				_player.abortAttack();
				
				_player.setReputation(_playerReputation);
				_player.setPkKills(_playerPkKills);
				_player.setCursedWeaponEquippedId(0);
				removeSkill();
				
				// Remove
				_player.getInventory().unEquipItemInBodySlot(ItemTemplate.SLOT_LR_HAND);
				_player.storeMe();
				
				// Destroy
				_player.getInventory().destroyItemByItemId("", _itemId, 1, _player, null);
				_player.sendItemList();
				_player.broadcastUserInfo();
			}
			else
			{
				// Remove from Db
				LOGGER.Info(_name + " being removed offline.");

				try
				{
					using GameServerDbContext ctx = new();
					
					int recordCnt = ctx.Items.Where(r => r.OwnerId == _playerId && r.ItemId == _itemId).ExecuteDelete();
					if (recordCnt != 1)
						LOGGER.Error("Error while deleting itemId " + _itemId + " from userId " + _playerId);

					recordCnt = ctx.Characters.Where(r => r.Id == _playerId)
						.ExecuteUpdate(s =>
							s.SetProperty(r => r.Reputation, _playerReputation)
								.SetProperty(r => r.PkKills, _playerPkKills));

					if (recordCnt != 1)
						LOGGER.Warn("Error while updating karma & pkkills for userId " + _playerId);
				}
				catch (Exception e)
				{
					LOGGER.Warn("Could not delete : " + e);
				}
			}
		}
		else
		{
			// either this cursed weapon is in the inventory of someone who has another cursed weapon equipped,
			// OR this cursed weapon is on the ground.
			if ((_player != null) && (_player.getInventory().getItemByItemId(_itemId) != null))
			{
				// Destroy
				_player.getInventory().destroyItemByItemId("", _itemId, 1, _player, null);
				_player.sendItemList();
				_player.broadcastUserInfo();
			}
			// is dropped on the ground
			else if (_item != null)
			{
				_item.decayMe();
				LOGGER.Info(_name + " item has been removed from World.");
			}
		}
		
		// Delete infos from table if any
		CursedWeaponsManager.removeFromDb(_itemId);
		
		SystemMessagePacket sm = new SystemMessagePacket(SystemMessageId.S1_HAS_DISAPPEARED);
		sm.Params.addItemName(_itemId);
		CursedWeaponsManager.announce(sm);
		
		// Reset state
		cancelTask();
		_isActivated = false;
		_isDropped = false;
		_endTime = DateTime.MinValue;
		_player = null;
		_playerId = 0;
		_playerReputation = 0;
		_playerPkKills = 0;
		_item = null;
		_nbKills = 0;
	}
	
	private void cancelTask()
	{
		if (_removeTask != null)
		{
			_removeTask.cancel(true);
			_removeTask = null;
		}
	}
	
	private class RemoveTask : Runnable
	{
		private readonly CursedWeapon _cursedWeapon;

		public RemoveTask(CursedWeapon cursedWeapon)
		{
			_cursedWeapon = cursedWeapon;
		}
		
		public void run()
		{
			if (DateTime.Now >= _cursedWeapon._endTime)
			{
				_cursedWeapon.endOfLife();
			}
		}
	}
	
	private void dropIt(Attackable attackable, Player player)
	{
		dropIt(attackable, player, null, true);
	}
	
	private void dropIt(Attackable attackable, Player player, Creature killer, bool fromMonster)
	{
		_isActivated = false;
		if (fromMonster)
		{
			_item = attackable.dropItem(player, _itemId, 1);
			_item.setDropTime(null); // Prevent item from being removed by ItemsAutoDestroy
			
			// RedSky and Earthquake
			ExRedSkyPacket rs = new ExRedSkyPacket(10);
			EarthquakePacket eq = new EarthquakePacket(player.getX(), player.getY(), player.getZ(), 14, 3);
			Broadcast.toAllOnlinePlayers(rs);
			Broadcast.toAllOnlinePlayers(eq);
		}
		else
		{
			_item = _player.getInventory().getItemByItemId(_itemId);
			_player.dropItem("DieDrop", _item, killer, true);
			_player.setReputation(_playerReputation);
			_player.setPkKills(_playerPkKills);
			_player.setCursedWeaponEquippedId(0);
			removeSkill();
			_player.abortAttack();
			// Item item = _player.getInventory().getItemByItemId(_itemId);
			// _player.getInventory().dropItem("DieDrop", item, _player, null);
			// _player.getInventory().getItemByItemId(_itemId).dropMe(_player, _player.getX(), _player.getY(), _player.getZ());
		}
		_isDropped = true;
		SystemMessagePacket sm = new SystemMessagePacket(SystemMessageId.S2_HAS_APPEARED_IN_S1_THE_TREASURE_CHEST_CONTAINS_S2_ADENA_FIXED_REWARD_S3_ADDITIONAL_REWARD_S4_THE_ADENA_WILL_BE_GIVEN_TO_THE_LAST_OWNER_AT_23_59);
		if (player != null)
		{
			sm.Params.addZoneName(player.getX(), player.getY(), player.getZ()); // Region Name
		}
		else if (_player != null)
		{
			sm.Params.addZoneName(_player.getX(), _player.getY(), _player.getZ()); // Region Name
		}
		else
		{
			sm.Params.addZoneName(killer.getX(), killer.getY(), killer.getZ()); // Region Name
		}
		sm.Params.addItemName(_itemId);
		CursedWeaponsManager.announce(sm); // in the Hot Spring region
	}
	
	public void cursedOnLogin()
	{
		doTransform();
		giveSkill();
		
		SystemMessagePacket msg = new SystemMessagePacket(SystemMessageId.THE_S2_S_OWNER_IS_IN_S1_THE_TREASURE_CHEST_CONTAINS_S2_ADENA_FIXED_REWARD_S3_ADDITIONAL_REWARD_S4_THE_ADENA_WILL_BE_GIVEN_TO_THE_LAST_OWNER_AT_23_59);
		msg.Params.addZoneName(_player.getX(), _player.getY(), _player.getZ());
		msg.Params.addItemName(_player.getCursedWeaponEquippedId());
		CursedWeaponsManager.announce(msg);
		
		CursedWeapon cw = CursedWeaponsManager.getInstance().getCursedWeapon(_player.getCursedWeaponEquippedId());
		SystemMessagePacket msg2 = new SystemMessagePacket(SystemMessageId.S1_HAS_S2_MIN_OF_USAGE_TIME_REMAINING);
		msg2.Params.addItemName(_player.getCursedWeaponEquippedId());
		msg2.Params.addInt((int)cw.getTimeLeft().TotalMinutes);
		_player.sendPacket(msg2);
	}
	
	/**
	 * Yesod:<br>
	 * Rebind the passive skill belonging to the CursedWeapon. Invoke this method if the weapon owner switches to a subclass.
	 */
	public void giveSkill()
	{
		int level = 1 + (_nbKills / _stageKills);
		if (level > _skillMaxLevel)
		{
			level = _skillMaxLevel;
		}
		
		Skill skill = SkillData.getInstance().getSkill(_skillId, level);
		_player.addSkill(skill, false);
		
		// Void Burst, Void Flow
		_player.addTransformSkill(CommonSkill.VOID_BURST.getSkill());
		_player.addTransformSkill(CommonSkill.VOID_FLOW.getSkill());
		_player.sendSkillList();
	}
	
	public void doTransform()
	{
		if (_itemId == 8689)
		{
			transformationId = 302;
		}
		else if (_itemId == 8190)
		{
			transformationId = 301;
		}
		
		if (_player.isTransformed())
		{
			_player.stopTransformation(true);
			
			ThreadPool.schedule(() => _player.transform(transformationId, true), 500);
		}
		else
		{
			_player.transform(transformationId, true);
		}
	}
	
	public void removeSkill()
	{
		_player.removeSkill(_skillId);
		_player.untransform();
		_player.sendSkillList();
	}
	
	public void reActivate()
	{
		_isActivated = true;
		if ((_endTime <= DateTime.Now))
		{
			endOfLife();
		}
		else
		{
			_removeTask = ThreadPool.scheduleAtFixedRate(new RemoveTask(this), _durationLost * 12000, _durationLost * 12000);
		}
	}
	
	public bool checkDrop(Attackable attackable, Player player)
	{
		if (Rnd.get(100000) < _dropRate)
		{
			// Drop the item
			dropIt(attackable, player);
			
			// Start the Life Task
			_endTime = DateTime.Now.AddMilliseconds((_duration * 60000));
			_removeTask = ThreadPool.scheduleAtFixedRate(new RemoveTask(this), _durationLost * 12000, _durationLost * 12000);
			return true;
		}
		return false;
	}
	
	public void activate(Player player, Item item)
	{
		// If the player is mounted, attempt to unmount first.
		// Only allow picking up the cursed weapon if unmounting is successful.
		if (player.isMounted() && !player.dismount())
		{
			// TODO: Verify the following system message, may still be custom.
			player.sendPacket(SystemMessageId.YOU_HAVE_FAILED_TO_PICK_UP_S1);
			player.dropItem("InvDrop", item, null, true);
			return;
		}
		
		_isActivated = true;
		
		// Player holding it data
		_player = player;
		_playerId = _player.getObjectId();
		_playerReputation = _player.getReputation();
		_playerPkKills = _player.getPkKills();
		saveData();
		
		// Change player stats
		_player.setCursedWeaponEquippedId(_itemId);
		_player.setReputation(-9999999);
		_player.setPkKills(0);
		if (_player.isInParty())
		{
			_player.getParty().removePartyMember(_player, PartyMessageType.EXPELLED);
		}
		
		// Disable All Skills
		// Do Transform
		doTransform();
		// Add skill
		giveSkill();
		
		// Equip with the weapon
		_item = item;
		_player.getInventory().equipItem(_item);
		SystemMessagePacket sm = new SystemMessagePacket(SystemMessageId.S1_EQUIPPED);
		sm.Params.addItemName(_item);
		_player.sendPacket(sm);
		
		// Fully heal player
		_player.setCurrentHpMp(_player.getMaxHp(), _player.getMaxMp());
		_player.setCurrentCp(_player.getMaxCp());
		
		// Refresh inventory
		_player.sendItemList();
		
		// Refresh player stats
		_player.broadcastUserInfo();
		
		SocialActionPacket atk = new SocialActionPacket(_player.getObjectId(), 17);
		_player.broadcastPacket(atk);
		
		sm = new SystemMessagePacket(SystemMessageId.THE_S2_S_OWNER_HAS_APPEARED_IN_S1_THE_TREASURE_CHEST_CONTAINS_S2_ADENA_FIXED_REWARD_S3_ADDITIONAL_REWARD_S4_THE_ADENA_WILL_BE_GIVEN_TO_THE_LAST_OWNER_AT_23_59);
		sm.Params.addZoneName(_player.getX(), _player.getY(), _player.getZ()); // Region Name
		sm.Params.addItemName(_item);
		CursedWeaponsManager.announce(sm);
	}
	
	public void saveData()
	{
		try 
		{
			using GameServerDbContext ctx = new();

			// Delete previous datas
			ctx.CursedWeapons.Where(r => r.ItemId == _itemId).ExecuteDelete();

			if (_isActivated)
			{
				ctx.CursedWeapons.Add(new DbCursedWeapon()
				{
					ItemId = _itemId,
					CharacterId = _playerId,
					PlayerReputation = _playerReputation,
					PlayerPkKills = _playerPkKills,
					NbKills = _nbKills,
					EndTime = _endTime
				});

				ctx.SaveChanges();
			}
		}
		catch (Exception e)
		{
			LOGGER.Error("CursedWeapon: Failed to save data. " + e);
		}
	}
	
	public void dropIt(Creature killer)
	{
		if (Rnd.get(100) <= _disapearChance)
		{
			// Remove it
			endOfLife();
		}
		else
		{
			// Unequip & Drop
			dropIt(null, null, killer, false);
			// Reset player stats
			_player.setReputation(_playerReputation);
			_player.setPkKills(_playerPkKills);
			_player.setCursedWeaponEquippedId(0);
			removeSkill();
			
			_player.abortAttack();
			
			_player.broadcastUserInfo();
		}
	}
	
	public void increaseKills()
	{
		_nbKills++;
		
		if ((_player != null) && _player.isOnline())
		{
			_player.setPkKills(_nbKills);
			_player.updateUserInfo();
			if (((_nbKills % _stageKills) == 0) && (_nbKills <= (_stageKills * (_skillMaxLevel - 1))))
			{
				giveSkill();
			}
		}
		
		// Reduce time-to-live
		_endTime = _endTime.AddMilliseconds(_durationLost * -60000);
		saveData();
	}
	
	public void setDisapearChance(int disapearChance)
	{
		_disapearChance = disapearChance;
	}
	
	public void setDropRate(int dropRate)
	{
		_dropRate = dropRate;
	}
	
	public void setDuration(int duration)
	{
		_duration = duration;
	}
	
	public void setDurationLost(int durationLost)
	{
		_durationLost = durationLost;
	}
	
	public void setStageKills(int stageKills)
	{
		_stageKills = stageKills;
	}
	
	public void setNbKills(int nbKills)
	{
		_nbKills = nbKills;
	}
	
	public void setPlayerId(int playerId)
	{
		_playerId = playerId;
	}
	
	public void setPlayerReputation(int playerReputation)
	{
		_playerReputation = playerReputation;
	}
	
	public void setPlayerPkKills(int playerPkKills)
	{
		_playerPkKills = playerPkKills;
	}
	
	public void setActivated(bool isActivated)
	{
		_isActivated = isActivated;
	}
	
	public void setDropped(bool isDropped)
	{
		_isDropped = isDropped;
	}
	
	public void setEndTime(DateTime endTime)
	{
		_endTime = endTime;
	}
	
	public void setPlayer(Player player)
	{
		_player = player;
	}
	
	public void setItem(Item item)
	{
		_item = item;
	}
	
	public bool isActivated()
	{
		return _isActivated;
	}
	
	public bool isDropped()
	{
		return _isDropped;
	}
	
	public DateTime getEndTime()
	{
		return _endTime;
	}
	
	public String getName()
	{
		return _name;
	}
	
	public int getItemId()
	{
		return _itemId;
	}
	
	public int getSkillId()
	{
		return _skillId;
	}
	
	public int getPlayerId()
	{
		return _playerId;
	}
	
	public Player getPlayer()
	{
		return _player;
	}
	
	public int getPlayerReputation()
	{
		return _playerReputation;
	}
	
	public int getPlayerPkKills()
	{
		return _playerPkKills;
	}
	
	public int getNbKills()
	{
		return _nbKills;
	}
	
	public int getStageKills()
	{
		return _stageKills;
	}
	
	public bool isActive()
	{
		return _isActivated || _isDropped;
	}
	
	public int getLevel()
	{
		if (_nbKills > (_stageKills * _skillMaxLevel))
		{
			return _skillMaxLevel;
		}
		return (_nbKills / _stageKills);
	}
	
	public TimeSpan getTimeLeft()
	{
		return _endTime - DateTime.UtcNow;
	}
	
	public void goTo(Player player)
	{
		if (player == null)
		{
			return;
		}
		
		if (_isActivated && (_player != null))
		{
			// Go to player holding the weapon
			player.teleToLocation(_player.getLocation(), true);
		}
		else if (_isDropped && (_item != null))
		{
			// Go to item on the ground
			player.teleToLocation(_item.getLocation(), true);
		}
		else
		{
			player.sendMessage(_name + " isn't in the World.");
		}
	}
	
	public Location getWorldPosition()
	{
		if (_isActivated && (_player != null))
		{
			return _player.getLocation();
		}
		
		if (_isDropped && (_item != null))
		{
			return _item.getLocation();
		}
		
		return null;
	}
	
	public long getDuration()
	{
		return _duration;
	}
}
