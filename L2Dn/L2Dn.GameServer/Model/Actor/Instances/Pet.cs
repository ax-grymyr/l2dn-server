using System.Runtime.CompilerServices;
using L2Dn.GameServer.AI;
using L2Dn.GameServer.Data.Sql;
using L2Dn.GameServer.Data.Xml;
using L2Dn.GameServer.Db;
using L2Dn.GameServer.Enums;
using L2Dn.GameServer.Handlers;
using L2Dn.GameServer.InstanceManagers;
using L2Dn.GameServer.Model.Actor.Stats;
using L2Dn.GameServer.Model.Actor.Templates;
using L2Dn.GameServer.Model.Holders;
using L2Dn.GameServer.Model.ItemContainers;
using L2Dn.GameServer.Model.Items;
using L2Dn.GameServer.Model.Items.Instances;
using L2Dn.GameServer.Model.Skills;
using L2Dn.GameServer.Model.Zones;
using L2Dn.GameServer.Network.Enums;
using L2Dn.GameServer.Network.OutgoingPackets;
using L2Dn.GameServer.Network.OutgoingPackets.Pets;
using L2Dn.GameServer.TaskManagers;
using L2Dn.GameServer.Utilities;
using Microsoft.EntityFrameworkCore;
using NLog;
using ThreadPool = System.Threading.ThreadPool;

namespace L2Dn.GameServer.Model.Actor.Instances;

public class Pet: Summon
{
	protected static readonly Logger LOGGER_PET = LogManager.GetLogger(nameof(Pet));
	
	private const String ADD_SKILL_SAVE = "INSERT INTO character_pet_skills_save (petObjItemId,skill_id,skill_level,skill_sub_level,remaining_time,buff_index) VALUES (?,?,?,?,?,?)";
	private const String RESTORE_SKILL_SAVE = "SELECT petObjItemId,skill_id,skill_level,skill_sub_level,remaining_time,buff_index FROM character_pet_skills_save WHERE petObjItemId=? ORDER BY buff_index ASC";
	private const String DELETE_SKILL_SAVE = "DELETE FROM character_pet_skills_save WHERE petObjItemId=?";
	private const String SELECT_PET_SKILLS = "SELECT * FROM pet_skills WHERE petObjItemId=?";
	private const String INSERT_PET_SKILLS = "INSERT INTO pet_skills (petObjItemId, skillId, skillLevel) VALUES (?,?,?) ON DUPLICATE KEY UPDATE skillId=VALUES(skillId), skillLevel=VALUES(skillLevel), petObjItemId=VALUES(petObjItemId)";
	private const String DELETE_PET_SKILLS = "DELETE FROM pet_skills WHERE petObjItemId=?";
	
	protected int _curFed;
	protected readonly PetInventory _inventory;
	private readonly bool _mountable;
	private readonly int _controlObjectId;
	private bool _respawned;
	private int _petType = 0;
	private int _curWeightPenalty = 0;
	private long _expBeforeDeath = 0;
	private PetData _data;
	private PetLevelData _leveldata;
	private EvolveLevel _evolveLevel = EvolveLevel.None;
	private ScheduledFuture _feedTask;
	
	private void deletePetEvolved()
	{
		try 
		{
			using GameServerDbContext ctx = new();
			ctx.PetEvolves.Where(r => r.ItemObjectId == _controlObjectId).ExecuteDelete();
		}
		catch (Exception e)
		{
			LOGGER.Error("Could not delete pet evolve data " + _controlObjectId + ": " + e);
		}
	}
	
	public void restorePetEvolvesByItem()
	{
		try 
		{
			using GameServerDbContext ctx = new();
			foreach (PetEvolve record in ctx.PetEvolves.Where(r => r.ItemObjectId == _controlObjectId))
			{
				setEvolveLevel((EvolveLevel)record.Level);
			}
		}
		catch (Exception e)
		{
			LOGGER.Error("Could not restore pet evolve for playerId: " + getObjectId() + ": " + e);
		}
	}
	
	public void storeEvolvedPets(int evolveLevel, int index, int controlItemObjId)
	{
		deletePetEvolved();
		try 
		{
			using GameServerDbContext ctx = new();
			PetEvolve? record = ctx.PetEvolves.SingleOrDefault(r =>
				r.ItemObjectId == controlItemObjId && r.Index == index && r.Level == evolveLevel);

			if (record is null)
			{
				record = new PetEvolve()
				{
					ItemObjectId = controlItemObjId,
					Index = index,
					Level = evolveLevel
				};

				ctx.PetEvolves.Add(record);
				ctx.SaveChanges();
			}
		}
		catch (Exception e)
		{
			LOGGER.Error(e);
		}

		getOwner().setPetEvolve(controlItemObjId,
			new PetEvolveHolder(index, (EvolveLevel)evolveLevel, getName(), getLevel(), getExpForThisLevel()));
	}
	
	public void storePetSkills(int skillId, int skillLevel)
	{
		try 
		{
			using GameServerDbContext ctx = new();
			DbPetSkill? record = ctx.PetSkills.SingleOrDefault(r =>
				r.PetItemObjectId == _controlObjectId && r.SkillId == skillId && r.SkillLevel == skillLevel);

			if (record is null)
			{
				record = new DbPetSkill()
				{
					PetItemObjectId = _controlObjectId,
					SkillLevel = (short)skillLevel,
					SkillId = skillId
				};

				ctx.PetSkills.Add(record);
				ctx.SaveChanges();
			}
		}
		catch (Exception e)
		{
			LOGGER.Error("Could not store pet skill data: " + e);
		}
	}
	
	public void restoreSkills()
	{
		try 
		{
			using GameServerDbContext ctx = new();
			foreach (DbPetSkill record in ctx.PetSkills.Where(r => r.PetItemObjectId == _controlObjectId))
			{
				Skill skill = SkillData.getInstance().getSkill(record.SkillId, record.SkillLevel);
				if (skill == null)
				{
					continue;
				}

				addSkill(skill);
			}

			ctx.PetSkills.Where(r => r.PetItemObjectId == _controlObjectId).ExecuteDelete();
		}
		catch (Exception e)
		{
			LOGGER.Error("Could not restore " + this + " skill data: " + e);
		}
	}
	
	public PetLevelData getPetLevelData()
	{
		if (_leveldata == null)
		{
			_leveldata = PetDataTable.getInstance().getPetLevelData(getTemplate().getId(), getStat().getLevel());
		}
		return _leveldata;
	}
	
	public PetData getPetData()
	{
		if (_data == null)
		{
			_data = PetDataTable.getInstance().getPetData(getTemplate().getId());
		}
		setPetType(_data.getDefaultPetType());
		return _data;
	}
	
	public void setPetData(PetLevelData value)
	{
		_leveldata = value;
	}
	
	/**
	 * Manage Feeding Task.<br>
	 * Feed or kill the pet depending on hunger level.<br>
	 * If pet has food in inventory and feed level drops below 55% then consume food from inventory.<br>
	 * Send a broadcastStatusUpdate packet for this Pet
	 */
	private class FeedTask: Runnable
	{
		private readonly Pet _pet;

		public FeedTask(Pet pet)
		{
			_pet = pet;
		}
		
		public void run()
		{
			try
			{
				Summon pet = _pet.getOwner().getPet();
				BuffInfo buffInfo = _pet.getOwner() != null ? _pet.getOwner().getEffectList().getBuffInfoBySkillId(49300) : null;
				int buffLvl = buffInfo == null ? 0 : buffInfo.getSkill().getLevel();
				int feedCons = buffLvl != 0 ? getFeedConsume() + ((getFeedConsume() / 100) * (buffLvl * 50)) : getFeedConsume();
				if ((_pet.getOwner() == null) || (pet == null) || (pet.getObjectId() != _pet.getObjectId()))
				{
					_pet.stopFeed();
					return;
				}
				else if (_pet._curFed > feedCons)
				{
					_pet.setCurrentFed(_pet._curFed - feedCons);
				}
				else
				{
					_pet.setCurrentFed(0);
				}
				
				_pet.broadcastStatusUpdate();
				
				Set<int> foodIds = _pet.getPetData().getFood();
				if (foodIds.isEmpty())
				{
					if (_pet.isUncontrollable())
					{
						// Owl Monk remove PK
						if ((_pet.getTemplate().getId() == 16050) && (_pet.getOwner() != null))
						{
							_pet.getOwner().setPkKills(Math.Max(0, _pet.getOwner().getPkKills() - Rnd.get(1, 6)));
						}
						_pet.sendPacket(SystemMessageId.THE_PET_IS_NOW_LEAVING);
						_pet.deleteMe(_pet.getOwner());
					}
					else if (_pet.isHungry())
					{
						_pet.sendPacket(SystemMessageId.THERE_IS_NOT_MUCH_TIME_REMAINING_UNTIL_THE_PET_LEAVES);
					}
					return;
				}
				
				Item food = null;
				foreach (int id in foodIds)
				{
					food = _pet.getOwner().getInventory().getItemByItemId(id);
					if ((food != null) && _pet.getOwner().getAutoUseSettings().getAutoSupplyItems().Contains(id))
					{
						break;
					}
				}
				
				if ((food != null) && _pet.isHungry() && _pet.getOwner().getAutoUseSettings().getAutoSupplyItems().Contains(food.getId()) && 
				    !pet.isInsideZone(ZoneId.PEACE))
				{
					IItemHandler handler = ItemHandler.getInstance().getHandler(food.getEtcItem());
					if (handler != null)
					{
						SystemMessagePacket sm = new SystemMessagePacket(SystemMessageId.YOUR_PET_WAS_HUNGRY_SO_IT_ATE_S1);
						sm.Params.addItemName(food.getId());
						_pet.sendPacket(sm);
						handler.useItem(_pet.getOwner(), food, false);
					}
				}
				
				if (_pet.isUncontrollable())
				{
					_pet.sendPacket(SystemMessageId.YOUR_PET_IS_STARVING_AND_WILL_NOT_OBEY_UNTIL_IT_GETS_IT_S_FOOD_FEED_YOUR_PET);
				}
			}
			catch (Exception e)
			{
				LOGGER_PET.Error("Pet [ObjectId: " + _pet.getObjectId() + "] a feed task error has occurred: " + e);
			}
		}
		
		private int getFeedConsume()
		{
			// if pet is attacking
			if (_pet.isAttackingNow())
			{
				return _pet.getPetLevelData().getPetFeedBattle();
			}
			return _pet.getPetLevelData().getPetFeedNormal();
		}
	}
	
	[MethodImpl(MethodImplOptions.Synchronized)]
	public static Pet spawnPet(NpcTemplate template, Player owner, Item control)
	{
		Pet existingPet = World.getInstance().getPet(owner.getObjectId());
		if (existingPet != null) // owner has a pet listed in world
		{
			existingPet.unSummon(owner);
		}
		
		Pet pet = restore(control, template, owner);
		// add the pet instance to world
		if (pet != null)
		{
			pet.restoreSkills();
			pet.restorePetEvolvesByItem();
			pet.setTitle(owner.getName());
			World.getInstance().addPet(owner.getObjectId(), pet);
		}
		return pet;
	}
	
	public Pet upgrade(NpcTemplate template)
	{
		unSummon(getOwner());
		Pet pet = restore(getControlItem(), template, getOwner());
		// add the pet instance to world
		if (pet != null)
		{
			pet.restoreSkills();
			pet.restorePetEvolvesByItem();
			pet.setTitle(getOwner().getName());
			World.getInstance().addPet(getOwner().getObjectId(), pet);
		}
		return pet;
	}

	/**
	 * Constructor for new pet
	 * @param template
	 * @param owner
	 * @param control
	 */
	public Pet(NpcTemplate template, Player owner, Item control): this(template, owner, control,
		template.getDisplayId() == 12564 ? owner.getLevel() : 1)
	{
	}

	/**
	 * Constructor for restored pet
	 * @param template
	 * @param owner
	 * @param control
	 * @param level
	 */
	public Pet(NpcTemplate template, Player owner, Item control, int level): base(template, owner)
	{
		setInstanceType(InstanceType.Pet);
		
		_controlObjectId = control.getObjectId();
		getStat().setLevel(Math.Max(level, PetDataTable.getInstance().getPetMinLevel(template.getId())));
		_inventory = new PetInventory(this);
		_inventory.restore();
		
		int npcId = template.getId();
		_mountable = PetDataTable.isMountable(npcId);
		getPetData();
		getPetLevelData();
	}
	
	public override PetStat getStat()
	{
		return (PetStat)base.getStat();
	}
	
	public override void initCharStat()
	{
		setStat(new PetStat(this));
	}
	
	public bool isRespawned()
	{
		return _respawned;
	}
	
	public override int getSummonType()
	{
		return 2;
	}
	
	public override int getControlObjectId()
	{
		return _controlObjectId;
	}
	
	public Item getControlItem()
	{
		return getOwner().getInventory().getItemByObjectId(_controlObjectId);
	}
	
	public int getCurrentFed()
	{
		return _curFed;
	}
	
	public void setCurrentFed(int num)
	{
		if (num <= 0)
		{
			sendPacket(new ExChangeNpcStatePacket(getObjectId(), 0x64)); // TODO: what numbers mean?
		}
		else if ((_curFed <= 0) && (num > 0))
		{
			sendPacket(new ExChangeNpcStatePacket(getObjectId(), 0x65));
		}
		_curFed = num > getMaxFed() ? getMaxFed() : num;
	}
	
	/**
	 * Returns the pet's currently equipped weapon instance (if any).
	 */
	public override Item getActiveWeaponInstance()
	{
		if (_inventory != null)
		{
			foreach (Item item in _inventory.getItems())
			{
				if ((item.getItemLocation() == ItemLocation.PET_EQUIP) &&
				    ((item.getTemplate().getBodyPart() == ItemTemplate.SLOT_R_HAND) ||
				     (item.getTemplate().getBodyPart() == ItemTemplate.SLOT_LR_HAND)))
				{
					return item;
				}
			}
		}
		return null;
	}
	
	/**
	 * Returns the pet's currently equipped weapon (if any).
	 */
	public override Weapon getActiveWeaponItem()
	{
		Item weapon = getActiveWeaponInstance();
		if (weapon == null)
		{
			return null;
		}
		return (Weapon) weapon.getTemplate();
	}
	
	public override Item getSecondaryWeaponInstance()
	{
		// temporary? unavailable
		return null;
	}
	
	public override Weapon getSecondaryWeaponItem()
	{
		// temporary? unavailable
		return null;
	}
	
	public override PetInventory getInventory()
	{
		return _inventory;
	}
	
	/**
	 * Destroys item from inventory and send a Server->Client InventoryUpdate packet to the Player.
	 * @param process : String Identifier of process triggering this action
	 * @param objectId : int Item Instance identifier of the item to be destroyed
	 * @param count : int Quantity of items to be destroyed
	 * @param reference : WorldObject Object referencing current action like NPC selling item or previous item in transformation
	 * @param sendMessage : bool Specifies whether to send message to Client about this action
	 * @return bool informing if the action was successfull
	 */
	public override bool destroyItem(String process, int objectId, long count, WorldObject reference, bool sendMessage)
	{
		Item item = _inventory.destroyItem(process, objectId, count, getOwner(), reference);
		if (item == null)
		{
			if (sendMessage)
			{
				sendPacket(SystemMessageId.INCORRECT_ITEM_COUNT_2);
			}
			return false;
		}
		
		// Send Pet inventory update packet
		PetInventoryUpdatePacket petIU = new PetInventoryUpdatePacket(item);
		sendPacket(petIU);
		
		if (sendMessage)
		{
			if (count > 1)
			{
				SystemMessagePacket sm = new SystemMessagePacket(SystemMessageId.S1_X_S2_DISAPPEARED);
				sm.Params.addItemName(item.getId());
				sm.Params.addLong(count);
				sendPacket(sm);
			}
			else
			{
				SystemMessagePacket sm = new SystemMessagePacket(SystemMessageId.S1_DISAPPEARED);
				sm.Params.addItemName(item.getId());
				sendPacket(sm);
			}
		}
		return true;
	}
	
	/**
	 * Destroy item from inventory by using its <b>itemId</b> and send a Server->Client InventoryUpdate packet to the Player.
	 * @param process : String Identifier of process triggering this action
	 * @param itemId : int Item identifier of the item to be destroyed
	 * @param count : int Quantity of items to be destroyed
	 * @param reference : WorldObject Object referencing current action like NPC selling item or previous item in transformation
	 * @param sendMessage : bool Specifies whether to send message to Client about this action
	 * @return bool informing if the action was successfull
	 */
	public override bool destroyItemByItemId(String process, int itemId, long count, WorldObject reference, bool sendMessage)
	{
		Item item = _inventory.destroyItemByItemId(process, itemId, count, getOwner(), reference);
		if (item == null)
		{
			if (sendMessage)
			{
				sendPacket(SystemMessageId.INCORRECT_ITEM_COUNT_2);
			}
			return false;
		}
		
		// Send Pet inventory update packet
		PetInventoryUpdatePacket petIU = new PetInventoryUpdatePacket(item);
		sendPacket(petIU);
		
		if (sendMessage)
		{
			if (count > 1)
			{
				SystemMessagePacket sm = new SystemMessagePacket(SystemMessageId.S1_X_S2_DISAPPEARED);
				sm.Params.addItemName(item.getId());
				sm.Params.addLong(count);
				sendPacket(sm);
			}
			else
			{
				SystemMessagePacket sm = new SystemMessagePacket(SystemMessageId.S1_DISAPPEARED);
				sm.Params.addItemName(item.getId());
				sendPacket(sm);
			}
		}
		
		return true;
	}
	
	public override void doPickupItem(WorldObject @object)
	{
		if (isDead())
		{
			return;
		}
		
		getAI().setIntention(CtrlIntention.AI_INTENTION_IDLE);
		broadcastPacket(new StopMovePacket(this));
		if (!@object.isItem())
		{
			// dont try to pickup anything that is not an item :)
			LOGGER_PET.Warn(this + " trying to pickup wrong target." + @object);
			sendPacket(ActionFailedPacket.STATIC_PACKET);
			return;
		}
		
		bool follow = getFollowStatus();
		Item target = (Item) @object;
		
		SystemMessagePacket smsg;

		// Cursed weapons
		if (CursedWeaponsManager.getInstance().isCursed(target.getId()))
		{
			smsg = new SystemMessagePacket(SystemMessageId.YOU_HAVE_FAILED_TO_PICK_UP_S1);
			smsg.Params.addItemName(target.getId());
			sendPacket(smsg);
			return;
		}
		else if (FortSiegeManager.getInstance().isCombat(target.getId()))
		{
			return;
		}
		
		lock (target)
		{
			// Check if the target to pick up is visible
			if (!target.isSpawned())
			{
				// Send a Server->Client packet ActionFailed to this Player
				sendPacket(ActionFailedPacket.STATIC_PACKET);
				return;
			}
			
			if (!target.getDropProtection().tryPickUp(this))
			{
				sendPacket(ActionFailedPacket.STATIC_PACKET);
				smsg = new SystemMessagePacket(SystemMessageId.YOU_HAVE_FAILED_TO_PICK_UP_S1);
				smsg.Params.addItemName(target);
				sendPacket(smsg);
				return;
			}
			
			if (((isInParty() && (getParty().getDistributionType() == PartyDistributionType.FINDERS_KEEPERS)) || !isInParty()) && !getOwner().getInventory().validateCapacity(target))
			{
				sendPacket(ActionFailedPacket.STATIC_PACKET);
				sendPacket(SystemMessageId.YOUR_PET_CANNOT_CARRY_ANY_MORE_ITEMS);
				return;
			}
			
			if ((target.getOwnerId() != 0) && (target.getOwnerId() != getOwner().getObjectId()) && !getOwner().isInLooterParty(target.getOwnerId()))
			{
				if (target.getId() == Inventory.ADENA_ID)
				{
					smsg = new SystemMessagePacket(SystemMessageId.YOU_HAVE_FAILED_TO_PICK_UP_S1_ADENA);
					smsg.Params.addLong(target.getCount());
				}
				else if (target.getCount() > 1)
				{
					smsg = new SystemMessagePacket(SystemMessageId.YOU_HAVE_FAILED_TO_PICK_UP_S2_S1_S);
					smsg.Params.addItemName(target);
					smsg.Params.addLong(target.getCount());
				}
				else
				{
					smsg = new SystemMessagePacket(SystemMessageId.YOU_HAVE_FAILED_TO_PICK_UP_S1);
					smsg.Params.addItemName(target);
				}
				sendPacket(ActionFailedPacket.STATIC_PACKET);
				sendPacket(smsg);
				return;
			}
			
			if ((target.getItemLootShedule() != null) && ((target.getOwnerId() == getOwner().getObjectId()) || getOwner().isInLooterParty(target.getOwnerId())))
			{
				target.resetOwnerTimer();
			}
			
			// Remove from the ground!
			target.pickupMe(this);
			
			if (Config.SAVE_DROPPED_ITEM)
			{
				ItemsOnGroundManager.getInstance().removeObject(target);
			}
		}
		
		// Herbs
		if (target.getTemplate().hasExImmediateEffect())
		{
			IItemHandler handler = ItemHandler.getInstance().getHandler(target.getEtcItem());
			if (handler == null)
			{
				LOGGER.Warn("No item handler registered for item ID: " + target.getId() + ".");
			}
			else
			{
				handler.useItem(this, target, false);
			}
			
			ItemData.getInstance().destroyItem("Consume", target, getOwner(), null);
			broadcastStatusUpdate();
		}
		else
		{
			if (target.getId() == Inventory.ADENA_ID)
			{
				smsg = new SystemMessagePacket(SystemMessageId.YOUR_PET_PICKED_UP_S1_ADENA);
				smsg.Params.addLong(target.getCount());
				sendPacket(smsg);
			}
			else if (target.getEnchantLevel() > 0)
			{
				smsg = new SystemMessagePacket(SystemMessageId.YOUR_PET_HAS_PICKED_UP_S1_S2);
				smsg.Params.addInt(target.getEnchantLevel());
				smsg.Params.addItemName(target);
				sendPacket(smsg);
			}
			else if (target.getCount() > 1)
			{
				smsg = new SystemMessagePacket(SystemMessageId.YOUR_PET_PICKED_UP_S2_S1_S);
				smsg.Params.addLong(target.getCount());
				smsg.Params.addItemName(target);
				sendPacket(smsg);
			}
			else
			{
				smsg = new SystemMessagePacket(SystemMessageId.YOUR_PET_PICKED_UP_S1);
				smsg.Params.addItemName(target);
				sendPacket(smsg);
			}
			
			// If owner is in party and it isnt finders keepers, distribute the item instead of stealing it -.-
			if (getOwner().isInParty() && (getOwner().getParty().getDistributionType() != PartyDistributionType.FINDERS_KEEPERS))
			{
				getOwner().getParty().distributeItem(getOwner(), target);
			}
			else
			{
				Item item = getOwner().getInventory().addItem("Pet Pickup", target, getOwner(), this);
				if (item != null)
				{
					getOwner().sendPacket(new PetItemListPacket(getInventory().getItems()));
				}
			}
		}
		
		getAI().setIntention(CtrlIntention.AI_INTENTION_IDLE);
		
		if (follow)
		{
			followOwner();
		}
	}
	
	public override void deleteMe(Player owner)
	{
		// Pet related - Removed on Essence.
		// _inventory.transferItemsToOwner();
		
		base.deleteMe(owner);
		
		// Pet related - Removed on Essence.
		// destroyControlItem(owner, false); // this should also delete the pet from the db
		// CharSummonTable.getInstance().getPets().remove(getOwner().getObjectId());
	}

	public override bool doDie(Creature killer)
	{
		Player owner = getOwner();
		if ((owner != null) && !owner.isInDuel() && (!isInsideZone(ZoneId.PVP) || isInsideZone(ZoneId.SIEGE)))
		{
			deathPenalty();
		}

		if (!base.doDie(killer, true))
		{
			return false;
		}

		stopFeed();

		// Pet related - Removed on Essence.
		// sendPacket(SystemMessageId.THE_PET_HAS_BEEN_KILLED_IF_YOU_DON_T_RESURRECT_IT_WITHIN_24_H_THE_PET_S_BODY_WILL_DISAPPEAR_ALONG_WITH_ALL_THE_PET_S_ITEMS);
		// Pet related - Added the following.
		storeMe();
		foreach (Skill skill in getAllSkills())
		{
			storePetSkills(skill.getId(), skill.getLevel());
		}

		DecayTaskManager.getInstance().add(this);
		if (owner != null)
		{
			BuffInfo buffInfo = owner.getEffectList().getBuffInfoBySkillId(49300);
			owner.getEffectList().add(new BuffInfo(owner, owner,
				SkillData.getInstance().getSkill(49300,
					buffInfo == null ? 1 : Math.Min(buffInfo.getSkill().getLevel() + 1, 10)), false, null, null));
		}

		// do not decrease exp if is in duel, arena
		return true;
	}

	public override void doRevive()
	{
		getOwner().removeReviving();
		
		base.doRevive();
		
		// stopDecay
		DecayTaskManager.getInstance().cancel(this);
		startFeed();
		if (!isHungry())
		{
			setRunning();
		}
		getAI().setIntention(CtrlIntention.AI_INTENTION_ACTIVE);
	}
	
	public override void doRevive(double revivePower)
	{
		// Restore the pet's lost experience,
		// depending on the % return of the skill used (based on its power).
		restoreExp(revivePower);
		doRevive();
	}
	
	/**
	 * Transfers item to another inventory
	 * @param process string identifier of process triggering this action
	 * @param objectId Item Identifier of the item to be transfered
	 * @param count Quantity of items to be transfered
	 * @param target
	 * @param actor the player requesting the item transfer
	 * @param reference Object referencing current action like NPC selling item or previous item in transformation
	 * @return Item corresponding to the new item or the updated item in inventory
	 */
	public Item transferItem(String process, int objectId, long count, Inventory target, Player actor, WorldObject reference)
	{
		Item oldItem = _inventory.getItemByObjectId(objectId);
		Item playerOldItem = target.getItemByItemId(oldItem.getId());
		Item newItem = _inventory.transferItem(process, objectId, count, target, actor, reference);
		if (newItem == null)
		{
			return null;
		}
		
		// Send inventory update packet
		PetInventoryUpdatePacket petIU;
		if ((oldItem.getCount() > 0) && (oldItem != newItem))
		{
			petIU = new PetInventoryUpdatePacket(new ItemInfo(oldItem, ItemChangeType.MODIFIED));
		}
		else
		{
			petIU = new PetInventoryUpdatePacket(new ItemInfo(oldItem, ItemChangeType.REMOVED));
		}
		
		sendInventoryUpdate(petIU);
		
		// Send target update packet
		if ((playerOldItem != null) && newItem.isStackable())
		{
			InventoryUpdatePacket iu = new InventoryUpdatePacket(new ItemInfo(newItem, ItemChangeType.MODIFIED));
			getOwner().sendInventoryUpdate(iu);
		}
		
		return newItem;
	}
	
	/**
	 * Remove the Pet from DB and its associated item from the player inventory
	 * @param owner The owner from whose inventory we should delete the item
	 * @param evolve
	 */
	public void destroyControlItem(Player owner, bool evolve)
	{
		// remove the pet instance from world
		World.getInstance().removePet(owner.getObjectId());
		
		// delete from inventory
		try
		{
			Item removedItem;
			if (evolve)
			{
				removedItem = owner.getInventory().destroyItem("Evolve", _controlObjectId, 1, getOwner(), this);
			}
			else
			{
				removedItem = owner.getInventory().destroyItem("PetDestroy", _controlObjectId, 1, getOwner(), this);
				if (removedItem != null)
				{
					SystemMessagePacket sm = new SystemMessagePacket(SystemMessageId.S1_DISAPPEARED);
					sm.Params.addItemName(removedItem);
					owner.sendPacket(sm);
				}
			}
			
			if (removedItem == null)
			{
				LOGGER.Warn("Couldn't destroy pet control item for " + owner + " pet: " + this + " evolve: " + evolve);
			}
			else
			{
				InventoryUpdatePacket iu = new InventoryUpdatePacket(new ItemInfo(removedItem, ItemChangeType.REMOVED));
				owner.sendInventoryUpdate(iu);
				owner.broadcastUserInfo();
			}
		}
		catch (Exception e)
		{
			LOGGER_PET.Warn("Error while destroying control item: " + e);
		}
		
		// pet control item no longer exists, delete the pet from the db
		try 
		{
			using GameServerDbContext ctx = new();
			ctx.Pets.Where(r => r.ItemObjectId == _controlObjectId).ExecuteDelete();
		}
		catch (Exception e)
		{
			LOGGER_PET.Error("Failed to delete Pet [ObjectId: " + getObjectId() + "]: " + e);
		}
	}
	
	public void dropAllItems()
	{
		try
		{
			foreach (Item item in _inventory.getItems())
			{
				dropItemHere(item);
			}
		}
		catch (Exception e)
		{
			LOGGER_PET.Warn("Pet Drop Error: " + e);
		}
	}
	
	public void dropItemHere(Item item, bool protect)
	{
		Item dropit = _inventory.dropItem("Drop", item.getObjectId(), item.getCount(), getOwner(), this);
		if (dropit != null)
		{
			if (protect)
			{
				dropit.getDropProtection().protect(getOwner());
			}
			LOGGER_PET.Trace("Item id to drop: " + dropit.getId() + " amount: " + dropit.getCount());
			dropit.dropMe(this, getX(), getY(), getZ() + 100);
		}
	}
	
	public void dropItemHere(Item dropit)
	{
		dropItemHere(dropit, false);
	}
	
	/**
	 * @return Returns the mount able.
	 */
	public override bool isMountable()
	{
		return _mountable;
	}
	
	public static Pet restore(Item control, NpcTemplate template, Player owner)
	{
		try 
		{
			using GameServerDbContext ctx = new();
			PreparedStatement statement =
				con.prepareStatement(
					"SELECT item_obj_id, name, level, curHp, curMp, exp, sp, fed FROM pets WHERE item_obj_id=?");
			Pet pet;
			statement.setInt(1, control.getObjectId());
			ResultSet rset = statement.executeQuery();
			{
				if (!rset.next())
				{
					return new Pet(template, owner, control);
				}
				
				pet = new Pet(template, owner, control, rset.getInt("level"));
				pet._respawned = true;
				pet.setName(rset.getString("name"));
				
				long exp = rset.getLong("exp");
				PetLevelData info = PetDataTable.getInstance().getPetLevelData(pet.getId(), pet.getLevel());
				// DS: update experience based by level
				// Avoiding pet delevels due to exp per level values changed.
				if ((info != null) && (exp < info.getPetMaxExp()))
				{
					exp = info.getPetMaxExp();
				}
				
				pet.getStat().setExp(exp);
				pet.getStat().setLevel(rset.getInt("level"));
				pet.getStat().setSp(rset.getInt("sp"));
				
				pet.getStatus().setCurrentHp(rset.getInt("curHp"));
				pet.getStatus().setCurrentMp(rset.getInt("curMp"));
				pet.getStatus().setCurrentCp(pet.getMaxCp());
				if (rset.getDouble("curHp") < 1)
				{
					// Pet related - Removed on Essence.
					// pet.setDead(true);
					// pet.stopHpMpRegeneration();
					// Pet related - Added the following.
					pet.setCurrentHpMp(pet.getMaxHp(), pet.getMaxMp());
				}
				pet.setEvolveLevel(pet.getPetData().getEvolveLevel());
				pet.setCurrentFed(rset.getInt("fed"));
			}
			return pet;
		}
		catch (Exception e)
		{
			LOGGER_PET.Warn("Could not restore pet data for owner: " + owner + " - " + e);
		}
		return null;
	}
	
	public override void setRestoreSummon(bool value)
	{
		_restoreSummon = value;
	}
	
	public override void stopSkillEffects(SkillFinishType type, int skillId)
	{
		base.stopSkillEffects(type, skillId);
		ICollection<SummonEffectTable.SummonEffect> effects = SummonEffectTable.getInstance().getPetEffects().get(getControlObjectId());
		if ((effects != null) && !effects.isEmpty())
		{
			foreach (SummonEffectTable.SummonEffect effect in effects)
			{
				if (effect.getSkill().getId() == skillId)
				{
					SummonEffectTable.getInstance().getPetEffects().get(getControlObjectId()).Remove(effect);
				}
			}
		}
	}
	
	public override void storeMe()
	{
		if (_controlObjectId == 0)
		{
			// this is a summon, not a pet, don't store anything
			return;
		}
		
		if (!Config.RESTORE_PET_ON_RECONNECT)
		{
			_restoreSummon = false;
		}
		
		String req;
		if (!_respawned)
		{
			req = "INSERT INTO pets (name,level,curHp,curMp,exp,sp,fed,ownerId,restore,item_obj_id) VALUES (?,?,?,?,?,?,?,?,?,?)";
		}
		else
		{
			req = "UPDATE pets SET name=?,level=?,curHp=?,curMp=?,exp=?,sp=?,fed=?,ownerId=?,restore=? WHERE item_obj_id = ?";
		}
		
		try 
		{
			using GameServerDbContext ctx = new();
			PreparedStatement statement = con.prepareStatement(req);
			statement.setString(1, getName());
			statement.setInt(2, getStat().getLevel());
			statement.setDouble(3, getStatus().getCurrentHp());
			statement.setDouble(4, getStatus().getCurrentMp());
			statement.setLong(5, getStat().getExp());
			statement.setLong(6, getStat().getSp());
			statement.setInt(7, _curFed);
			statement.setInt(8, getOwner().getObjectId());
			statement.setString(9, String.valueOf(_restoreSummon)); // True restores pet on login
			statement.setInt(10, _controlObjectId);
			statement.executeUpdate();
			
			_respawned = true;
			if (_restoreSummon)
			{
				CharSummonTable.getInstance().getPets().put(getOwner().getObjectId(), getControlObjectId());
			}
			else
			{
				CharSummonTable.getInstance().getPets().remove(getOwner().getObjectId());
			}
		}
		catch (Exception e)
		{
			LOGGER_PET.Error("Failed to store Pet [ObjectId: " + getObjectId() + "] data: " + e);
		}
		
		Item itemInst = getControlItem();
		if ((itemInst != null) && (itemInst.getEnchantLevel() != getStat().getLevel()))
		{
			itemInst.setEnchantLevel(getStat().getLevel());
			itemInst.updateDatabase();
		}
	}
	
	public override void storeEffect(bool storeEffects)
	{
		if (!Config.SUMMON_STORE_SKILL_COOLTIME)
		{
			return;
		}
		
		// Clear list for overwrite
		SummonEffectTable.getInstance().getPetEffects().getOrDefault(getControlObjectId(), Collections.emptyList()).clear();
		
		try 
		{
			using GameServerDbContext ctx = new();
			PreparedStatement ps1 = con.prepareStatement(DELETE_SKILL_SAVE);
			PreparedStatement ps2 = con.prepareStatement(ADD_SKILL_SAVE);
			
			// Delete all current stored effects for summon to avoid dupe
			ps1.setInt(1, _controlObjectId);
			ps1.execute();
			
			int buffIndex = 0;
			
			Set<long> storedSkills = new();
			
			// Store all effect data along with calculated remaining
			if (storeEffects)
			{
				foreach (BuffInfo info in getEffectList().getEffects())
				{
					if (info == null)
					{
						continue;
					}
					
					Skill skill = info.getSkill();
					
					// Do not store those effects.
					if (skill.isDeleteAbnormalOnLeave())
					{
						continue;
					}
					
					// Do not save heals.
					if (skill.getAbnormalType() == AbnormalType.LIFE_FORCE_OTHERS)
					{
						continue;
					}
					
					// Toggles are skipped, unless they are necessary to be always on.
					if (skill.isToggle() && !skill.isNecessaryToggle())
					{
						continue;
					}
					
					// Dances and songs are not kept in retail.
					if (skill.isDance() && !Config.ALT_STORE_DANCES)
					{
						continue;
					}
					
					if (!storedSkills.add(skill.getReuseHashCode()))
					{
						continue;
					}
					
					ps2.setInt(1, _controlObjectId);
					ps2.setInt(2, skill.getId());
					ps2.setInt(3, skill.getLevel());
					ps2.setInt(4, skill.getSubLevel());
					ps2.setInt(5, info.getTime());
					ps2.setInt(6, ++buffIndex);
					ps2.addBatch();
					
					SummonEffectTable.getInstance().getPetEffects().computeIfAbsent(getControlObjectId(), k => ConcurrentHashMap.newKeySet()).add(new SummonEffectTable.SummonEffect(skill, info.getTime()));
				}
				ps2.executeBatch();
			}
		}
		catch (Exception e)
		{
			LOGGER.Warn("Could not store pet effect data: " + e);
		}
	}
	
	public override void restoreEffects()
	{
		try 
		{
			using GameServerDbContext ctx = new();
			PreparedStatement ps1 = con.prepareStatement(RESTORE_SKILL_SAVE);
			PreparedStatement ps2 = con.prepareStatement(DELETE_SKILL_SAVE);
			if (!SummonEffectTable.getInstance().getPetEffects().containsKey(getControlObjectId()))
			{
				ps1.setInt(1, _controlObjectId);
				ResultSet rset = ps1.executeQuery();
				{
					while (rset.next())
					{
						int effectCurTime = rset.getInt("remaining_time");
						Skill skill = SkillData.getInstance().getSkill(rset.getInt("skill_id"), rset.getInt("skill_level"));
						if (skill == null)
						{
							continue;
						}
						
						if (skill.hasEffects(EffectScope.GENERAL))
						{
							SummonEffectTable.getInstance().getPetEffects().computeIfAbsent(getControlObjectId(), k -> ConcurrentHashMap.newKeySet()).add(new SummonEffect(skill, effectCurTime));
						}
					}
				}
			}
			
			ps2.setInt(1, _controlObjectId);
			ps2.executeUpdate();
		}
		catch (Exception e)
		{
			LOGGER.Warn("Could not restore " + this + " active effect data: " + e);
		}
		finally
		{
			if (SummonEffectTable.getInstance().getPetEffects().get(getControlObjectId()) != null)
			{
				foreach (SummonEffectTable.SummonEffect se in SummonEffectTable.getInstance().getPetEffects().get(getControlObjectId()))
				{
					if (se != null)
					{
						se.getSkill().applyEffects(this, this, false, se.getEffectCurTime());
					}
				}
			}
		}
	}
	
	[MethodImpl(MethodImplOptions.Synchronized)]
	public void stopFeed()
	{
		if (_feedTask != null)
		{
			_feedTask.cancel(false);
			_feedTask = null;
		}
	}
	
	[MethodImpl(MethodImplOptions.Synchronized)]
	public void startFeed()
	{
		// stop feeding task if its active
		stopFeed();
		if (!isDead() && (getOwner().getPet() == this))
		{
			_feedTask = ThreadPool.scheduleAtFixedRate(new FeedTask(), 10000, 10000);
		}
	}
	
	[MethodImpl(MethodImplOptions.Synchronized)]
	public override void unSummon(Player owner)
	{
		stopFeed();
		stopHpMpRegeneration();
		base.unSummon(owner);
		
		if (!isDead())
		{
			if (_inventory != null)
			{
				_inventory.deleteMe();
			}
			World.getInstance().removePet(owner.getObjectId());
		}
	}
	
	/**
	 * Restore the specified % of experience this Pet has lost.
	 * @param restorePercent
	 */
	public void restoreExp(double restorePercent)
	{
		if (_expBeforeDeath > 0)
		{
			// Restore the specified % of lost experience.
			getStat().addExp(Math.Round(((_expBeforeDeath - getStat().getExp()) * restorePercent) / 100));
			_expBeforeDeath = 0;
		}
	}
	
	private void deathPenalty()
	{
		// TODO: Need Correct Penalty
		
		int level = getStat().getLevel();
		double percentLost = (-0.07 * level) + 6.5;
		
		// Calculate the Experience loss
		long lostExp = (long)(((getStat().getExpForLevel(level + 1) - getStat().getExpForLevel(level)) * percentLost) / 100);
		
		// Get the Experience before applying penalty
		_expBeforeDeath = getStat().getExp();
		
		// Set the new Experience value of the Pet
		getStat().addExp(-lostExp);
	}
	
	[MethodImpl(MethodImplOptions.Synchronized)]
	public override void addExpAndSp(double addToExp, double addToSp)
	{
		if (getId() == 12564) // TODO: Remove this stupid hardcode.
		{
			getStat().addExpAndSp(addToExp * Config.SINEATER_XP_RATE);
		}
		else
		{
			getStat().addExpAndSp(addToExp * Config.PET_XP_RATE);
		}
	}
	
	public override long getExpForThisLevel()
	{
		if (getLevel() >= ExperienceData.getInstance().getMaxPetLevel())
		{
			return 0;
		}
		return getStat().getExpForLevel(getLevel());
	}
	
	public override long getExpForNextLevel()
	{
		if (getLevel() >= (ExperienceData.getInstance().getMaxPetLevel() - 1))
		{
			return 0;
		}
		return getStat().getExpForLevel(getLevel() + 1);
	}
	
	public override int getLevel()
	{
		return getStat().getLevel();
	}
	
	public int getMaxFed()
	{
		return getStat().getMaxFeed();
	}
	
	public override int getCriticalHit()
	{
		return getStat().getCriticalHit();
	}
	
	public override int getMAtk()
	{
		return getStat().getMAtk();
	}
	
	public override int getMDef()
	{
		return getStat().getMDef();
	}
	
	public override int getSkillLevel(int skillId)
	{
		if (getKnownSkill(skillId) == null)
		{
			return 0;
		}
		
		int level = getLevel();
		return level > 70 ? 7 + ((level - 70) / 5) : level / 10;
	}
	
	public void updateRefOwner(Player owner)
	{
		int oldOwnerId = getOwner().getObjectId();
		setOwner(owner);
		World.getInstance().removePet(oldOwnerId);
		World.getInstance().addPet(oldOwnerId, this);
	}
	
	public int getInventoryLimit()
	{
		return Config.INVENTORY_MAXIMUM_PET;
	}
	
	public void refreshOverloaded()
	{
		int maxLoad = getMaxLoad();
		if (maxLoad > 0)
		{
			long weightproc = (((getCurrentLoad() - getBonusWeightPenalty()) * 1000) / maxLoad);
			int newWeightPenalty;
			if ((weightproc < 500) || getOwner().getDietMode())
			{
				newWeightPenalty = 0;
			}
			else if (weightproc < 666)
			{
				newWeightPenalty = 1;
			}
			else if (weightproc < 800)
			{
				newWeightPenalty = 2;
			}
			else if (weightproc < 1000)
			{
				newWeightPenalty = 3;
			}
			else
			{
				newWeightPenalty = 4;
			}
			
			if (_curWeightPenalty != newWeightPenalty)
			{
				_curWeightPenalty = newWeightPenalty;
				if (newWeightPenalty > 0)
				{
					addSkill(SkillData.getInstance().getSkill(4270, newWeightPenalty));
					setOverloaded(getCurrentLoad() >= maxLoad);
				}
				else
				{
					removeSkill(getKnownSkill(4270), true);
					setOverloaded(false);
				}
			}
		}
	}
	
	public override void updateAndBroadcastStatus(int value)
	{
		refreshOverloaded();
		base.updateAndBroadcastStatus(value);
	}
	
	public override bool isHungry()
	{
		return _curFed < ((getPetData().getHungryLimit() / 100f) * getPetLevelData().getPetMaxFeed());
	}
	
	/**
	 * Verifies if a pet can be controlled by it's owner.<br>
	 * Starving pets cannot be controlled.
	 * @return {@code true} if the per cannot be controlled
	 */
	public bool isUncontrollable()
	{
		return _curFed <= 0;
	}
	
	public override int getWeapon()
	{
		Item weapon = _inventory.getPaperdollItem(Inventory.PAPERDOLL_RHAND);
		if (weapon != null)
		{
			return weapon.getId();
		}
		return 0;
	}
	
	public override int getArmor()
	{
		Item weapon = _inventory.getPaperdollItem(Inventory.PAPERDOLL_CHEST);
		if (weapon != null)
		{
			return weapon.getId();
		}
		return 0;
	}
	
	public int getJewel()
	{
		Item weapon = _inventory.getPaperdollItem(Inventory.PAPERDOLL_NECK);
		if (weapon != null)
		{
			return weapon.getId();
		}
		return 0;
	}
	
	public override short getSoulShotsPerHit()
	{
		return getPetLevelData().getPetSoulShot();
	}
	
	public override short getSpiritShotsPerHit()
	{
		return getPetLevelData().getPetSpiritShot();
	}
	
	public override void setName(String name)
	{
		Item controlItem = getControlItem();
		if (controlItem != null)
		{
			if (controlItem.getCustomType2() == (name == null ? 1 : 0))
			{
				// name not set yet
				controlItem.setCustomType2(name != null ? 1 : 0);
				controlItem.updateDatabase();
				InventoryUpdate iu = new InventoryUpdate();
				iu.addModifiedItem(controlItem);
				getOwner().sendInventoryUpdate(iu);
			}
		}
		else
		{
			LOGGER.Warn("Pet control item null, for pet: " + ToString());
		}
		base.setName(name);
	}
	
	public bool canEatFoodId(int itemId)
	{
		return _data.getFood().Contains(itemId);
	}
	
	public override bool isPet()
	{
		return true;
	}
	
	public override double getRunSpeed()
	{
		return base.getRunSpeed() * (isUncontrollable() ? 0.5d : 1.0d);
	}
	
	public override double getWalkSpeed()
	{
		return base.getWalkSpeed() * (isUncontrollable() ? 0.5d : 1.0d);
	}
	
	public override double getMovementSpeedMultiplier()
	{
		return base.getMovementSpeedMultiplier() * (isUncontrollable() ? 0.5d : 1.0d);
	}
	
	public override double getMoveSpeed()
	{
		if (isInsideZone(ZoneId.WATER))
		{
			return isRunning() ? getSwimRunSpeed() : getSwimWalkSpeed();
		}
		return isRunning() ? getRunSpeed() : getWalkSpeed();
	}
	
	public int getPetType()
	{
		return _petType;
	}
	
	public void setPetType(int petType)
	{
		_petType = petType;
	}
	
	public EvolveLevel getEvolveLevel()
	{
		return _evolveLevel;
	}
	
	public void setEvolveLevel(EvolveLevel evolveLevel)
	{
		_evolveLevel = evolveLevel;
	}
	
	public void useEquippableItem(Item item, bool abortAttack)
	{
		// Check if the item is null.
		if (item == null)
		{
			return;
		}
		
		// Check if the item is in the inventory.
		ItemLocation itemLocation = item.getItemLocation();
		if ((itemLocation != ItemLocation.INVENTORY) && (itemLocation != ItemLocation.PAPERDOLL) && (itemLocation != ItemLocation.PET) && (itemLocation != ItemLocation.PET_EQUIP))
		{
			return;
		}
		
		// Equip or unEquip
		List<Item> items;
		bool isEquiped = item.isEquipped();
		int oldInvLimit = getInventoryLimit();
		SystemMessagePacket sm;
		if (isEquiped)
		{
			if (item.getEnchantLevel() > 0)
			{
				sm = new SystemMessagePacket(SystemMessageId.S1_S2_UNEQUIPPED);
				sm.Params.addInt(item.getEnchantLevel());
				sm.Params.addItemName(item);
			}
			else
			{
				sm = new SystemMessagePacket(SystemMessageId.S1_UNEQUIPPED);
				sm.Params.addItemName(item);
			}
			sendPacket(sm);
			
			long slot = _inventory.getSlotFromItem(item);
			// we can't unequip talisman by body slot
			if ((slot == ItemTemplate.SLOT_DECO) || (slot == ItemTemplate.SLOT_BROOCH_JEWEL) || (slot == ItemTemplate.SLOT_AGATHION) || (slot == ItemTemplate.SLOT_ARTIFACT))
			{
				items = _inventory.unEquipItemInSlotAndRecord(item.getLocationSlot());
			}
			else
			{
				items = _inventory.unEquipItemInBodySlotAndRecord(slot);
			}
		}
		else
		{
			items = _inventory.equipItemAndRecord(item);
			if (item.isEquipped())
			{
				if (item.getEnchantLevel() > 0)
				{
					sm = new SystemMessagePacket(SystemMessageId.S1_S2_EQUIPPED);
					sm.Params.addInt(item.getEnchantLevel());
					sm.Params.addItemName(item);
				}
				else
				{
					sm = new SystemMessagePacket(SystemMessageId.S1_EQUIPPED);
					sm.Params.addItemName(item);
				}
				sendPacket(sm);
				// Consume mana - will start a task if required; returns if item is not a shadow item
				item.decreaseMana(false);
				
				if ((item.getTemplate().getBodyPart() & ItemTemplate.SLOT_MULTI_ALLWEAPON) != 0)
				{
					rechargeShots(true, true, false);
				}
			}
			else
			{
				sendPacket(SystemMessageId.YOU_DO_NOT_MEET_THE_REQUIRED_CONDITION_TO_EQUIP_THAT_ITEM);
			}
		}
		
		PetInventoryUpdate petIU = new PetInventoryUpdate();
		petIU.addItems(items);
		sendInventoryUpdate(petIU);
		
		if (abortAttack)
		{
			this.abortAttack();
		}
		
		if (getInventoryLimit() != oldInvLimit)
		{
			getOwner().sendPacket(new ExStorageMaxCountPacket(getOwner()));
		}
	}
}