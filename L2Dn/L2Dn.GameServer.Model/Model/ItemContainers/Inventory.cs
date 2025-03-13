using System.Runtime.CompilerServices;
using L2Dn.Events;
using L2Dn.GameServer.Cache;
using L2Dn.GameServer.Data.Xml;
using L2Dn.GameServer.Db;
using L2Dn.GameServer.Enums;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Events.Impl.Items;
using L2Dn.GameServer.Model.Holders;
using L2Dn.GameServer.Model.Items;
using L2Dn.GameServer.Model.Items.Appearance;
using L2Dn.GameServer.Model.Items.Instances;
using L2Dn.GameServer.Model.Items.Types;
using L2Dn.GameServer.Model.Skills;
using L2Dn.GameServer.Network.OutgoingPackets;
using L2Dn.GameServer.StaticData;
using L2Dn.GameServer.Utilities;
using NLog;

namespace L2Dn.GameServer.Model.ItemContainers;

public abstract class Inventory: ItemContainer
{
	protected new static readonly Logger LOGGER = LogManager.GetLogger(nameof(Inventory));

	public interface PaperdollListener
	{
		void notifyEquiped(int slot, Item inst, Inventory inventory);
		void notifyUnequiped(int slot, Item inst, Inventory inventory);
	}

	// Common Items
	public const int ADENA_ID = 57;
	public const int ANCIENT_ADENA_ID = 5575;
	public const int BEAUTY_TICKET_ID = 36308;
	public const int AIR_STONE_ID = 39461;
	public const int TEMPEST_STONE_ID = 39592;
	public const int ELCYUM_CRYSTAL_ID = 36514;
	public const int LCOIN_ID = 91663;
	public static readonly long MAX_ADENA = Config.MAX_ADENA;
	public const int SP_POUCH = 98232;
	public const int SP_POINTS = 15624;

	public const int PAPERDOLL_UNDER = 0;
	public const int PAPERDOLL_HEAD = 1;
	public const int PAPERDOLL_HAIR = 2;
	public const int PAPERDOLL_HAIR2 = 3;
	public const int PAPERDOLL_NECK = 4;
	public const int PAPERDOLL_RHAND = 5;
	public const int PAPERDOLL_CHEST = 6;
	public const int PAPERDOLL_LHAND = 7;
	public const int PAPERDOLL_REAR = 8;
	public const int PAPERDOLL_LEAR = 9;
	public const int PAPERDOLL_GLOVES = 10;
	public const int PAPERDOLL_LEGS = 11;
	public const int PAPERDOLL_FEET = 12;
	public const int PAPERDOLL_RFINGER = 13;
	public const int PAPERDOLL_LFINGER = 14;
	public const int PAPERDOLL_LBRACELET = 15;
	public const int PAPERDOLL_RBRACELET = 16;
	public const int PAPERDOLL_AGATHION1 = 17;
	public const int PAPERDOLL_AGATHION2 = 18;
	public const int PAPERDOLL_AGATHION3 = 19;
	public const int PAPERDOLL_AGATHION4 = 20;
	public const int PAPERDOLL_AGATHION5 = 21;
	public const int PAPERDOLL_DECO1 = 22;
	public const int PAPERDOLL_DECO2 = 23;
	public const int PAPERDOLL_DECO3 = 24;
	public const int PAPERDOLL_DECO4 = 25;
	public const int PAPERDOLL_DECO5 = 26;
	public const int PAPERDOLL_DECO6 = 27;
	public const int PAPERDOLL_CLOAK = 28;
	public const int PAPERDOLL_BELT = 29;
	public const int PAPERDOLL_BROOCH = 30;
	public const int PAPERDOLL_BROOCH_JEWEL1 = 31;
	public const int PAPERDOLL_BROOCH_JEWEL2 = 32;
	public const int PAPERDOLL_BROOCH_JEWEL3 = 33;
	public const int PAPERDOLL_BROOCH_JEWEL4 = 34;
	public const int PAPERDOLL_BROOCH_JEWEL5 = 35;
	public const int PAPERDOLL_BROOCH_JEWEL6 = 36;
	public const int PAPERDOLL_ARTIFACT_BOOK = 37;
	public const int PAPERDOLL_ARTIFACT1 = 38; // Artifact Balance
	public const int PAPERDOLL_ARTIFACT2 = 39; // Artifact Balance
	public const int PAPERDOLL_ARTIFACT3 = 40; // Artifact Balance
	public const int PAPERDOLL_ARTIFACT4 = 41; // Artifact Balance
	public const int PAPERDOLL_ARTIFACT5 = 42; // Artifact Balance
	public const int PAPERDOLL_ARTIFACT6 = 43; // Artifact Balance
	public const int PAPERDOLL_ARTIFACT7 = 44; // Artifact Balance
	public const int PAPERDOLL_ARTIFACT8 = 45; // Artifact Balance
	public const int PAPERDOLL_ARTIFACT9 = 46; // Artifact Balance
	public const int PAPERDOLL_ARTIFACT10 = 47; // Artifact Balance
	public const int PAPERDOLL_ARTIFACT11 = 48; // Artifact Balance
	public const int PAPERDOLL_ARTIFACT12 = 49; // Artifact Balance
	public const int PAPERDOLL_ARTIFACT13 = 50; // Artifact Spirit
	public const int PAPERDOLL_ARTIFACT14 = 51; // Artifact Spirit
	public const int PAPERDOLL_ARTIFACT15 = 52; // Artifact Spirit
	public const int PAPERDOLL_ARTIFACT16 = 53; // Artifact Protection
	public const int PAPERDOLL_ARTIFACT17 = 54; // Artifact Protection
	public const int PAPERDOLL_ARTIFACT18 = 55; // Artifact Protection
	public const int PAPERDOLL_ARTIFACT19 = 56; // Artifact Support
	public const int PAPERDOLL_ARTIFACT20 = 57; // Artifact Support
	public const int PAPERDOLL_ARTIFACT21 = 58; // Artifact Support

	public const int PAPERDOLL_TOTALSLOTS = 59;

	// Speed percentage mods
	public const double MAX_ARMOR_WEIGHT = 12000;

	private readonly Item?[] _paperdoll;
	private readonly List<PaperdollListener> _paperdollListeners;
	private readonly PaperdollCache _paperdollCache = new();

	// protected to be accessed from child classes only
	protected int _totalWeight;

	// used to quickly check for using of items of special type
	private ItemTypeMask _wearedMask;

	private long _blockedItemSlotsMask;

	// Recorder of alterations in inventory
	private class ChangeRecorder: PaperdollListener
	{
		private readonly Inventory _inventory;
		private readonly List<Item> _changed = new();

		/**
		 * Constructor of the ChangeRecorder
		 * @param inventory
		 */
		public ChangeRecorder(Inventory inventory)
		{
			_inventory = inventory;
			_inventory.addPaperdollListener(this);
		}

		/**
		 * Add alteration in inventory when item equipped
		 * @param slot
		 * @param item
		 * @param inventory
		 */
		public void notifyEquiped(int slot, Item item, Inventory inventory)
		{
			_changed.Add(item);
		}

		/**
		 * Add alteration in inventory when item unequipped
		 * @param slot
		 * @param item
		 * @param inventory
		 */
		public void notifyUnequiped(int slot, Item item, Inventory inventory)
		{
			_changed.Add(item);
		}

		/**
		 * Returns alterations in inventory
		 * @return Item[] : array of altered items
		 */
		public List<Item> getChangedItems()
		{
			return _changed;
		}
	}

	private class BowCrossRodListener: PaperdollListener
	{
		private static readonly BowCrossRodListener instance = new BowCrossRodListener();

		public static BowCrossRodListener getInstance()
		{
			return instance;
		}

		public void notifyUnequiped(int slot, Item item, Inventory inventory)
        {
            Weapon? weapon = item.getWeaponItem();
			if (slot != PAPERDOLL_RHAND || !item.isWeapon() || weapon == null)
			{
				return;
			}

			switch (weapon.getItemType().AsWeaponType())
			{
				case WeaponType.BOW:
				case WeaponType.CROSSBOW:
				case WeaponType.TWOHANDCROSSBOW:
				{
					Item? leftHandItem = inventory.getPaperdollItem(PAPERDOLL_LHAND);
					if (leftHandItem != null && leftHandItem.getItemType() != ArmorType.SIGIL)
					{
						inventory.setPaperdollItem(PAPERDOLL_LHAND, null);
					}

					Player? owner = inventory.getOwner()?.getActingPlayer();
					if (owner != null)
					{
						owner.removeAmmunitionSkills();
					}
					break;
				}
				case WeaponType.PISTOLS:
				{
					Player? owner = inventory.getOwner()?.getActingPlayer();
					if (owner != null)
					{
						owner.removeAmmunitionSkills();
					}
					break;
				}
				case WeaponType.FISHINGROD:
				{
					Item? leftHandItem = inventory.getPaperdollItem(PAPERDOLL_LHAND);
					if (leftHandItem != null)
					{
						inventory.setPaperdollItem(PAPERDOLL_LHAND, null);
					}
					break;
				}
			}
		}

		public void notifyEquiped(int slot, Item item, Inventory inventory)
		{
		}
	}

	private class StatsListener: PaperdollListener
	{
		private static readonly StatsListener instance = new StatsListener();

		public static StatsListener getInstance()
		{
			return instance;
		}

		public void notifyUnequiped(int slot, Item item, Inventory inventory)
		{
			inventory.getOwner()?.getStat().recalculateStats(true);
		}

		public void notifyEquiped(int slot, Item item, Inventory inventory)
		{
			inventory.getOwner()?.getStat().recalculateStats(true);
		}
	}

	private class ItemSkillsListener: PaperdollListener
	{
		private static readonly ItemSkillsListener instance = new ItemSkillsListener();

		public static ItemSkillsListener getInstance()
		{
			return instance;
		}

		public void notifyUnequiped(int slot, Item item, Inventory inventory)
		{
            Playable? playable = (Playable?)inventory.getOwner();
			if (playable == null)
				return;

            Player? player = playable.getActingPlayer();
            if (player is null)
                return;

			ItemTemplate it = item.getTemplate();
			Map<int, Skill> addedSkills = new();
			Map<int, Skill> removedSkills = new();
			bool update = false;
			bool updateTimestamp = false;

			// Remove augmentation bonuses on unequip
            VariationInstance? augmentation = item.getAugmentation();
			if (item.isAugmented() && augmentation != null)
			{
                augmentation.removeBonus(playable);
			}

			// Recalculate all stats
			playable.getStat().recalculateStats(true);

			// Clear enchant bonus
			item.clearEnchantStats();

			// Clear SA Bonus
			item.clearSpecialAbilities();

			if (it.hasSkills())
			{
				List<ItemSkillHolder>? onEnchantSkills = it.getSkills(ItemSkillType.ON_ENCHANT);
				if (onEnchantSkills != null)
				{
					foreach (ItemSkillHolder holder in onEnchantSkills)
					{
						if (item.getEnchantLevel() < holder.getValue())
						{
							continue;
						}

						Skill skill = holder.getSkill();
						if (skill != null)
						{
							removedSkills.TryAdd(skill.getId(), skill);
							update = true;
						}
					}
				}

				if (item.isBlessed())
				{
					List<ItemSkillHolder>? onBlessingSkills = it.getSkills(ItemSkillType.ON_BLESSING);
					if (onBlessingSkills != null)
					{
						foreach (ItemSkillHolder holder in onBlessingSkills)
						{
							Skill skill = holder.getSkill();
							if (skill != null)
							{
								removedSkills.TryAdd(skill.getId(), skill);
								update = true;
							}
						}
					}
				}

				List<ItemSkillHolder>? normalSkills = it.getSkills(ItemSkillType.NORMAL);
				if (normalSkills != null)
				{
					foreach (ItemSkillHolder holder in normalSkills)
					{
						Skill skill = holder.getSkill();
						if (skill != null)
						{
							removedSkills.TryAdd(skill.getId(), skill);
							update = true;
						}
					}
				}

				if (item.isArmor())
				{
					foreach (Item itm in inventory.getItems())
					{
						if (!itm.isEquipped() || itm.Equals(item))
						{
							continue;
						}

						List<ItemSkillHolder>? otherNormalSkills = itm.getTemplate().getSkills(ItemSkillType.NORMAL);
						if (otherNormalSkills == null)
						{
							continue;
						}

						foreach (ItemSkillHolder holder in otherNormalSkills)
						{
							if (playable.getSkillLevel(holder.getSkillId()) != 0)
							{
								continue;
							}

							Skill skill = holder.getSkill();
							if (skill == null)
							{
								continue;
							}

							Skill? existingSkill = addedSkills.get(skill.getId());
							if (existingSkill != null)
							{
								if (existingSkill.getLevel() < skill.getLevel())
								{
									addedSkills.put(skill.getId(), skill);
								}
							}
							else
							{
								addedSkills.put(skill.getId(), skill);
							}

							if (skill.isActive() && !playable.hasSkillReuse(skill.getReuseHashCode()))
							{
								TimeSpan equipDelay = item.getEquipReuseDelay();
								if (equipDelay > TimeSpan.Zero)
								{
									playable.addTimeStamp(skill, equipDelay);
									playable.disableSkill(skill, equipDelay);
								}
								updateTimestamp = true;
							}
							update = true;
						}
					}
				}
			}

			// Must check all equipped items for enchant conditions.
			foreach (Item equipped in inventory.getPaperdollItems())
			{
				if (!equipped.getTemplate().hasSkills())
				{
					continue;
				}

				List<ItemSkillHolder>? otherEnchantSkills = equipped.getTemplate().getSkills(ItemSkillType.ON_ENCHANT);
				List<ItemSkillHolder>? otherBlessingSkills = equipped.getTemplate().getSkills(ItemSkillType.ON_BLESSING);
				if (otherEnchantSkills == null && otherBlessingSkills == null)
				{
					continue;
				}

				if (otherEnchantSkills != null)
				{
					foreach (ItemSkillHolder holder in otherEnchantSkills)
					{
						if (equipped.getEnchantLevel() < holder.getValue())
						{
							continue;
						}

						Skill skill = holder.getSkill();
						if (skill == null)
						{
							continue;
						}

						// Check passive skill conditions.
						if (skill.isPassive() && !skill.checkConditions(SkillConditionScope.PASSIVE, playable, playable))
						{
							removedSkills.TryAdd(skill.getId(), skill);
							update = true;
						}
					}
				}

				if (otherBlessingSkills != null && equipped.isBlessed())
				{
					foreach (ItemSkillHolder holder in otherBlessingSkills)
					{
						Skill skill = holder.getSkill();
						if (skill == null)
						{
							continue;
						}

						// Check passive skill conditions.
						if (skill.isPassive() && !skill.checkConditions(SkillConditionScope.PASSIVE, playable, playable))
						{
							removedSkills.TryAdd(skill.getId(), skill);
							update = true;
						}
					}
				}
			}

			// Must check for toggle and isRemovedOnUnequipWeapon skill item conditions.
			foreach (Skill skill in playable.getAllSkills())
			{
				if ((skill.isToggle() && playable.isAffectedBySkill(skill.getId()) && !skill.checkConditions(SkillConditionScope.GENERAL, playable, playable)) //
					|| (it.isWeapon() && skill.isRemovedOnUnequipWeapon()))
				{
					playable.stopSkillEffects(SkillFinishType.REMOVED, skill.getId());
					update = true;
				}
			}

			// Apply skill, if item has "skills on unequip" and it is not a secondary agathion.
			if (slot < PAPERDOLL_AGATHION2 || slot > PAPERDOLL_AGATHION5)
			{
				it.forEachSkill(ItemSkillType.ON_UNEQUIP, holder => holder.getSkill().activateSkill(playable, [playable]));
			}

			if (update)
			{
				foreach (Skill skill in removedSkills.Values)
				{
					playable.removeSkill(skill, skill.isPassive());
				}

				foreach (Skill skill in addedSkills.Values)
				{
					playable.addSkill(skill);
				}

				if (playable.isPlayer())
				{
                    player.sendSkillList();
				}
			}

			if (updateTimestamp && playable.isPlayer())
			{
				playable.sendPacket(new SkillCoolTimePacket(player));
			}

			if (item.isWeapon())
			{
				playable.unchargeAllShots();
			}
		}

		public void notifyEquiped(int slot, Item item, Inventory inventory)
		{
            Playable? playable = (Playable?)inventory.getOwner();
			if (playable == null)
				return;

            Player? player = playable.getActingPlayer();
            if (player == null)
                return;

			Map<int, Skill> addedSkills = new();
			bool updateTimestamp = false;

			// Apply augmentation bonuses on equip
            VariationInstance? augmentation = item.getAugmentation();
			if (item.isAugmented() && augmentation != null)
			{
                augmentation.applyBonus(playable);
			}

			// Recalculate all stats
			playable.getStat().recalculateStats(true);

			// Apply enchant stats
			item.applyEnchantStats();

			// Apply SA skill
			item.applySpecialAbilities();

			if (item.getTemplate().hasSkills())
			{
				List<ItemSkillHolder>? onEnchantSkills = item.getTemplate().getSkills(ItemSkillType.ON_ENCHANT);
				if (onEnchantSkills != null)
				{
					foreach (ItemSkillHolder holder in onEnchantSkills)
					{
						if (playable.getSkillLevel(holder.getSkillId()) >= holder.getSkillLevel())
						{
							continue;
						}

						if (item.getEnchantLevel() < holder.getValue())
						{
							continue;
						}

						Skill skill = holder.getSkill();
						if (skill == null)
						{
							continue;
						}

						// Check passive skill conditions.
						if (skill.isPassive() && !skill.checkConditions(SkillConditionScope.PASSIVE, playable, playable))
						{
							continue;
						}

						Skill? existingSkill = addedSkills.get(skill.getId());
						if (existingSkill != null)
						{
							if (existingSkill.getLevel() < skill.getLevel())
							{
								addedSkills.put(skill.getId(), skill);
							}
						}
						else
						{
							addedSkills.put(skill.getId(), skill);
						}

						// Active, non-offensive, skills start with reuse on equip.
						if (skill.isActive() && !skill.isBad() && !skill.isTransformation() &&
						    Config.ITEM_EQUIP_ACTIVE_SKILL_REUSE > 0 && player.hasEnteredWorld())
						{
							playable.addTimeStamp(skill,
								skill.getReuseDelay() > TimeSpan.Zero
									? skill.getReuseDelay()
									: TimeSpan.FromMilliseconds(Config.ITEM_EQUIP_ACTIVE_SKILL_REUSE));

							updateTimestamp = true;
						}
					}
				}

				if (item.isBlessed())
				{
					List<ItemSkillHolder>? onBlessingSkills = item.getTemplate().getSkills(ItemSkillType.ON_BLESSING);
					if (onBlessingSkills != null)
					{
						foreach (ItemSkillHolder holder in onBlessingSkills)
						{
							if (playable.getSkillLevel(holder.getSkillId()) >= holder.getSkillLevel())
							{
								continue;
							}

							if (item.getEnchantLevel() < holder.getValue())
							{
								continue;
							}

							Skill skill = holder.getSkill();
							if (skill == null)
							{
								continue;
							}

							// Check passive skill conditions.
							if (skill.isPassive() && !skill.checkConditions(SkillConditionScope.PASSIVE, playable, playable))
							{
								continue;
							}

							Skill? existingSkill = addedSkills.get(skill.getId());
							if (existingSkill != null)
							{
								if (existingSkill.getLevel() < skill.getLevel())
								{
									addedSkills.put(skill.getId(), skill);
								}
							}
							else
							{
								addedSkills.put(skill.getId(), skill);
							}
						}
					}
				}

				List<ItemSkillHolder>? normalSkills = item.getTemplate().getSkills(ItemSkillType.NORMAL);
				if (normalSkills != null)
				{
					foreach (ItemSkillHolder holder in normalSkills)
					{
						if (playable.getSkillLevel(holder.getSkillId()) >= holder.getSkillLevel())
						{
							continue;
						}

						Skill skill = holder.getSkill();
						if (skill == null)
						{
							continue;
						}

						// Check passive skill conditions.
						if (skill.isPassive() && !skill.checkConditions(SkillConditionScope.PASSIVE, playable, playable))
						{
							continue;
						}

						Skill? existingSkill = addedSkills.get(skill.getId());
						if (existingSkill != null)
						{
							if (existingSkill.getLevel() < skill.getLevel())
							{
								addedSkills.put(skill.getId(), skill);
							}
						}
						else
						{
							addedSkills.put(skill.getId(), skill);
						}

						if (skill.isActive())
						{
							if (!playable.hasSkillReuse(skill.getReuseHashCode()))
							{
								TimeSpan equipDelay = item.getEquipReuseDelay();
								if (equipDelay > TimeSpan.Zero)
								{
									playable.addTimeStamp(skill, equipDelay);
									playable.disableSkill(skill, equipDelay);
								}
							}

							// Active, non-offensive, skills start with reuse on equip.
                            if (!skill.isBad() && !skill.isTransformation() &&
                                Config.ITEM_EQUIP_ACTIVE_SKILL_REUSE > 0 &&
                                player.hasEnteredWorld())
                            {
                                playable.addTimeStamp(skill,
                                    skill.getReuseDelay() > TimeSpan.Zero
                                        ? skill.getReuseDelay()
                                        : TimeSpan.FromMilliseconds(Config.ITEM_EQUIP_ACTIVE_SKILL_REUSE));
                            }

                            updateTimestamp = true;
						}
					}
				}
			}

			// Must check all equipped items for enchant conditions.
			foreach (Item equipped in inventory.getPaperdollItems())
			{
				if (!equipped.getTemplate().hasSkills())
				{
					continue;
				}

				List<ItemSkillHolder>? otherEnchantSkills = equipped.getTemplate().getSkills(ItemSkillType.ON_ENCHANT);
				List<ItemSkillHolder>? otherBlessingSkills = equipped.getTemplate().getSkills(ItemSkillType.ON_BLESSING);
				if (otherEnchantSkills == null && otherBlessingSkills == null)
				{
					continue;
				}

				if (otherEnchantSkills != null)
				{
					foreach (ItemSkillHolder holder in otherEnchantSkills)
					{
						if (playable.getSkillLevel(holder.getSkillId()) >= holder.getSkillLevel())
						{
							continue;
						}

						if (equipped.getEnchantLevel() < holder.getValue())
						{
							continue;
						}

						Skill skill = holder.getSkill();
						if (skill == null)
						{
							continue;
						}

						// Check passive skill conditions.
						if (skill.isPassive() && !skill.checkConditions(SkillConditionScope.PASSIVE, playable, playable))
						{
							continue;
						}

						Skill? existingSkill = addedSkills.get(skill.getId());
						if (existingSkill != null)
						{
							if (existingSkill.getLevel() < skill.getLevel())
							{
								addedSkills.put(skill.getId(), skill);
							}
						}
						else
						{
							addedSkills.put(skill.getId(), skill);
						}

						// Active, non offensive, skills start with reuse on equip.
                        if (skill.isActive() && !skill.isBad() && !skill.isTransformation() &&
                            Config.ITEM_EQUIP_ACTIVE_SKILL_REUSE > 0 && player.hasEnteredWorld())
                        {
                            playable.addTimeStamp(skill,
                                skill.getReuseDelay() > TimeSpan.Zero
                                    ? skill.getReuseDelay()
                                    : TimeSpan.FromMilliseconds(Config.ITEM_EQUIP_ACTIVE_SKILL_REUSE));

                            updateTimestamp = true;
                        }
                    }
				}

				if (otherBlessingSkills != null)
				{
					foreach (ItemSkillHolder holder in otherBlessingSkills)
					{
						if (playable.getSkillLevel(holder.getSkillId()) >= holder.getSkillLevel())
						{
							continue;
						}

						if (equipped.isBlessed())
						{
							Skill skill = holder.getSkill();
							if (skill == null)
							{
								continue;
							}

							// Check passive skill conditions.
							if (skill.isPassive() && !skill.checkConditions(SkillConditionScope.PASSIVE, playable, playable))
							{
								continue;
							}

							Skill? existingSkill = addedSkills.get(skill.getId());
							if (existingSkill != null)
							{
								if (existingSkill.getLevel() < skill.getLevel())
								{
									addedSkills.put(skill.getId(), skill);
								}
							}
							else
							{
								addedSkills.put(skill.getId(), skill);
							}
						}
					}
				}
			}

			// Apply skill, if item has "skills on equip" and it is not a secondary agathion.
			if (slot < PAPERDOLL_AGATHION2 || slot > PAPERDOLL_AGATHION5)
			{
				item.getTemplate().forEachSkill(ItemSkillType.ON_EQUIP, holder => holder.getSkill().activateSkill(playable, [playable]));
			}

			if (addedSkills.Count != 0)
			{
				foreach (Skill skill in addedSkills.Values)
				{
					playable.addSkill(skill);
				}

				if (playable.isPlayer())
				{
                    player.sendSkillList();
				}
			}

			if (updateTimestamp && playable.isPlayer())
			{
				playable.sendPacket(new SkillCoolTimePacket(player));
			}
		}
	}

	private class ArmorSetListener: PaperdollListener
	{
		private static readonly ArmorSetListener instance = new();

		public static ArmorSetListener getInstance()
		{
			return instance;
		}

		public void notifyEquiped(int slot, Item item, Inventory inventory)
		{
            Playable? playable = (Playable?)inventory.getOwner();
			if (playable == null)
			{
				return;
			}

			bool update = verifyAndApply(playable, item, x => x.getId());

			// Verify and apply normal set

			// Verify and apply visual set
			int itemVisualId = item.getVisualId();
			if (itemVisualId > 0)
			{
				int appearanceStoneId = item.getAppearanceStoneId();
				AppearanceStone? stone = AppearanceItemData.getInstance().getStone(appearanceStoneId > 0 ? appearanceStoneId : itemVisualId);
				if (stone != null && stone.getType() == AppearanceType.FIXED && verifyAndApply(playable, item, x => x.getVisualId()))
				{
					update = true;
				}
			}

            Player? player = playable.getActingPlayer();
            if (playable.isPlayer() && player != null)
            {
                if (update)
                {
                    player.sendSkillList();
                }

                if (item.getTemplate().getBodyPart() == ItemTemplate.SLOT_BROOCH_JEWEL ||
                    item.getTemplate().getBodyPart() == ItemTemplate.SLOT_BROOCH)
                {
                    player.updateActiveBroochJewel();
                }
            }
        }

		private static bool applySkills(Playable playable, Item item, ArmorSet armorSet, Func<Item, int> idProvider)
        {
            Player player = playable.getActingPlayer() ??
                throw new InvalidOperationException("Playable player is null, should not happen");

			long piecesCount = armorSet.getPiecesCount(playable, idProvider);
			if (piecesCount >= armorSet.getMinimumPieces())
			{
				// Applying all skills that matching the conditions
				bool updateTimeStamp = false;
				bool update = false;
				foreach (ArmorsetSkillHolder holder in armorSet.getSkills())
				{
					if (playable.getSkillLevel(holder.getSkillId()) >= holder.getSkillLevel())
					{
						continue;
					}

					if (holder.validateConditions(playable, armorSet, idProvider))
					{
						Skill itemSkill = holder.getSkill();
						if (itemSkill == null)
						{
							LOGGER.Warn("Inventory.ArmorSetListener.addSkills: Incorrect skill: " + holder);
							continue;
						}

						if (itemSkill.isPassive() && !itemSkill.checkConditions(SkillConditionScope.PASSIVE, playable, playable))
						{
							continue;
						}

						playable.addSkill(itemSkill);
						if (itemSkill.isActive())
						{
							if (item != null && !playable.hasSkillReuse(itemSkill.getReuseHashCode()))
							{
								TimeSpan equipDelay = item.getEquipReuseDelay();
								if (equipDelay > TimeSpan.Zero)
								{
									playable.addTimeStamp(itemSkill, equipDelay);
									playable.disableSkill(itemSkill, equipDelay);
								}
							}

							// Active, non-offensive, skills start with reuse on equip.
                            if (!itemSkill.isBad() && !itemSkill.isTransformation() &&
                                Config.ARMOR_SET_EQUIP_ACTIVE_SKILL_REUSE > 0 &&
                                player.hasEnteredWorld())
                            {
                                playable.addTimeStamp(itemSkill,
                                    itemSkill.getReuseDelay() > TimeSpan.Zero
                                        ? itemSkill.getReuseDelay()
                                        : TimeSpan.FromMilliseconds(Config.ARMOR_SET_EQUIP_ACTIVE_SKILL_REUSE));
                            }

                            updateTimeStamp = true;
						}

						update = true;
					}
				}

				if (updateTimeStamp && playable.isPlayer())
				{
					playable.sendPacket(new SkillCoolTimePacket(player));
				}
				return update;
			}
			return false;
		}

		private static bool verifyAndApply(Playable playable, Item item, Func<Item, int> idProvider)
		{
			bool update = false;
			List<ArmorSet> armorSets = ArmorSetData.getInstance().getSets(idProvider(item));
			foreach (ArmorSet armorSet in armorSets)
			{
				if (applySkills(playable, item, armorSet, idProvider))
				{
					update = true;
				}
			}
			return update;
		}

		private static bool verifyAndRemove(Playable playable, Item item, Func<Item, int> idProvider)
		{
			bool update = false;
			List<ArmorSet> armorSets = ArmorSetData.getInstance().getSets(idProvider(item));
			foreach (ArmorSet armorSet in armorSets)
			{
				// Remove all skills that doesn't matches the conditions
				foreach (ArmorsetSkillHolder holder in armorSet.getSkills())
				{
					if (!holder.validateConditions(playable, armorSet, idProvider))
					{
						Skill itemSkill = holder.getSkill();
						if (itemSkill == null)
						{
							LOGGER.Warn("Inventory.ArmorSetListener.removeSkills: Incorrect skill: " + holder);
							continue;
						}

						// Update if a skill has been removed.
						if (playable.removeSkill(itemSkill, itemSkill.isPassive()) != null)
						{
							update = true;
						}
					}
				}

				// Attempt to apply lower level skills if possible
				if (applySkills(playable, item, armorSet, idProvider))
				{
					update = true;
				}
			}

			return update;
		}

		public void notifyUnequiped(int slot, Item item, Inventory inventory)
		{
            Playable? playable = (Playable?)inventory.getOwner();
			if (playable == null)
				return;

            Player player = playable.getActingPlayer() ??
                throw new InvalidOperationException("Playable player is null, should not happen");

			bool remove = verifyAndRemove(playable, item, x => x.getId());

			// Verify and remove normal set bonus

			// Verify and remove visual set bonus
			int itemVisualId = item.getVisualId();
			if (itemVisualId > 0)
			{
				int appearanceStoneId = item.getAppearanceStoneId();
				AppearanceStone? stone = AppearanceItemData.getInstance().getStone(appearanceStoneId > 0 ? appearanceStoneId : itemVisualId);
				if (stone != null && stone.getType() == AppearanceType.FIXED && verifyAndRemove(playable, item, x => x.getVisualId()))
				{
					remove = true;
				}
			}

			if (!playable.isPlayer())
			{
				return;
			}

			if (remove)
			{
                player.checkItemRestriction();
                player.sendSkillList();
			}

			if (item.getTemplate().getBodyPart() == ItemTemplate.SLOT_BROOCH_JEWEL || item.getTemplate().getBodyPart() == ItemTemplate.SLOT_BROOCH)
			{
                player.updateActiveBroochJewel();
			}
		}
	}

	private class BraceletListener: PaperdollListener
	{
		private static readonly BraceletListener instance = new();

		public static BraceletListener getInstance()
		{
			return instance;
		}

		public void notifyUnequiped(int slot, Item item, Inventory inventory)
		{
			Player? player = item.getActingPlayer();
			if (player != null && player.isChangingClass())
			{
				return;
			}

			if (item.getTemplate().getBodyPart() == ItemTemplate.SLOT_R_BRACELET)
			{
				inventory.unEquipItemInSlot(PAPERDOLL_DECO1);
				inventory.unEquipItemInSlot(PAPERDOLL_DECO2);
				inventory.unEquipItemInSlot(PAPERDOLL_DECO3);
				inventory.unEquipItemInSlot(PAPERDOLL_DECO4);
				inventory.unEquipItemInSlot(PAPERDOLL_DECO5);
				inventory.unEquipItemInSlot(PAPERDOLL_DECO6);
			}
		}

		// Note (April 3, 2009): Currently on equip, talismans do not display properly, do we need checks here to fix this?
		public void notifyEquiped(int slot, Item item, Inventory inventory)
		{
		}
	}

	private class BroochListener: PaperdollListener
	{
		private static readonly BroochListener instance = new BroochListener();

		public static BroochListener getInstance()
		{
			return instance;
		}

		public void notifyUnequiped(int slot, Item item, Inventory inventory)
		{
			Player? player = item.getActingPlayer();
			if (player != null && player.isChangingClass())
			{
				return;
			}

			if (item.getTemplate().getBodyPart() == ItemTemplate.SLOT_BROOCH)
			{
				inventory.unEquipItemInSlot(PAPERDOLL_BROOCH_JEWEL1);
				inventory.unEquipItemInSlot(PAPERDOLL_BROOCH_JEWEL2);
				inventory.unEquipItemInSlot(PAPERDOLL_BROOCH_JEWEL3);
				inventory.unEquipItemInSlot(PAPERDOLL_BROOCH_JEWEL4);
				inventory.unEquipItemInSlot(PAPERDOLL_BROOCH_JEWEL5);
				inventory.unEquipItemInSlot(PAPERDOLL_BROOCH_JEWEL6);
			}
		}

		// Note (April 3, 2009): Currently on equip, talismans do not display properly, do we need checks here to fix this?
		public void notifyEquiped(int slot, Item item, Inventory inventory)
		{
		}
	}

	private class AgathionBraceletListener: PaperdollListener
	{
		private static readonly AgathionBraceletListener instance = new AgathionBraceletListener();

		public static AgathionBraceletListener getInstance()
		{
			return instance;
		}

		public void notifyUnequiped(int slot, Item item, Inventory inventory)
		{
			Player? player = item.getActingPlayer();
			if (player != null && player.isChangingClass())
			{
				return;
			}

			if (item.getTemplate().getBodyPart() == ItemTemplate.SLOT_L_BRACELET)
			{
				inventory.unEquipItemInSlot(PAPERDOLL_AGATHION1);
				inventory.unEquipItemInSlot(PAPERDOLL_AGATHION2);
				inventory.unEquipItemInSlot(PAPERDOLL_AGATHION3);
				inventory.unEquipItemInSlot(PAPERDOLL_AGATHION4);
				inventory.unEquipItemInSlot(PAPERDOLL_AGATHION5);
			}
		}

		public void notifyEquiped(int slot, Item item, Inventory inventory)
		{
		}
	}

	private class ArtifactBookListener: PaperdollListener
	{
		private static readonly ArtifactBookListener instance = new ArtifactBookListener();

		public static ArtifactBookListener getInstance()
		{
			return instance;
		}

		public void notifyUnequiped(int slot, Item item, Inventory inventory)
		{
			Player? player = item.getActingPlayer();
			if (player != null && player.isChangingClass())
			{
				return;
			}

			if (item.getTemplate().getBodyPart() == ItemTemplate.SLOT_ARTIFACT_BOOK)
			{
				inventory.unEquipItemInSlot(PAPERDOLL_ARTIFACT1);
				inventory.unEquipItemInSlot(PAPERDOLL_ARTIFACT2);
				inventory.unEquipItemInSlot(PAPERDOLL_ARTIFACT3);
				inventory.unEquipItemInSlot(PAPERDOLL_ARTIFACT4);
				inventory.unEquipItemInSlot(PAPERDOLL_ARTIFACT5);
				inventory.unEquipItemInSlot(PAPERDOLL_ARTIFACT6);
				inventory.unEquipItemInSlot(PAPERDOLL_ARTIFACT7);
				inventory.unEquipItemInSlot(PAPERDOLL_ARTIFACT8);
				inventory.unEquipItemInSlot(PAPERDOLL_ARTIFACT9);
				inventory.unEquipItemInSlot(PAPERDOLL_ARTIFACT10);
				inventory.unEquipItemInSlot(PAPERDOLL_ARTIFACT11);
				inventory.unEquipItemInSlot(PAPERDOLL_ARTIFACT12);
				inventory.unEquipItemInSlot(PAPERDOLL_ARTIFACT13);
				inventory.unEquipItemInSlot(PAPERDOLL_ARTIFACT14);
				inventory.unEquipItemInSlot(PAPERDOLL_ARTIFACT15);
				inventory.unEquipItemInSlot(PAPERDOLL_ARTIFACT16);
				inventory.unEquipItemInSlot(PAPERDOLL_ARTIFACT17);
				inventory.unEquipItemInSlot(PAPERDOLL_ARTIFACT18);
				inventory.unEquipItemInSlot(PAPERDOLL_ARTIFACT19);
				inventory.unEquipItemInSlot(PAPERDOLL_ARTIFACT20);
				inventory.unEquipItemInSlot(PAPERDOLL_ARTIFACT21);
			}
		}

		public void notifyEquiped(int slot, Item item, Inventory inventory)
		{
		}
	}

	/**
	 * Constructor of the inventory
	 */
	protected Inventory()
	{
		_paperdoll = new Item[PAPERDOLL_TOTALSLOTS];
		_paperdollListeners = new();

		if (this is PlayerInventory)
		{
			addPaperdollListener(ArmorSetListener.getInstance());
			addPaperdollListener(BowCrossRodListener.getInstance());
			addPaperdollListener(ItemSkillsListener.getInstance());
			addPaperdollListener(BraceletListener.getInstance());
			addPaperdollListener(BroochListener.getInstance());
			addPaperdollListener(AgathionBraceletListener.getInstance());
			addPaperdollListener(ArtifactBookListener.getInstance());
		}
		else if (this is PetInventory)
		{
			addPaperdollListener(ArmorSetListener.getInstance());
			addPaperdollListener(ItemSkillsListener.getInstance());
		}

		// common
		addPaperdollListener(StatsListener.getInstance());
	}

	protected abstract ItemLocation getEquipLocation();

	/**
	 * Returns the instance of new ChangeRecorder
	 * @return ChangeRecorder
	 */
	private ChangeRecorder newRecorder()
	{
		return new ChangeRecorder(this);
	}

	/**
	 * Drop item from inventory and updates database
	 * @param process : String Identifier of process triggering this action
	 * @param item : Item to be dropped
	 * @param actor : Player Player requesting the item drop
	 * @param reference : Object Object referencing current action like NPC selling item or previous item in transformation
	 * @return Item corresponding to the destroyed item or the updated item in inventory
	 */
	public virtual Item? dropItem(string process, Item item, Player actor, object? reference)
	{
		if (item == null)
		{
			return null;
		}

		lock (item)
		{
			if (!_items.Contains(item))
			{
				return null;
			}

			removeItem(item);
			item.setOwnerId(process, 0, actor, reference);
			item.setItemLocation(ItemLocation.VOID);
			item.setLastChange(ItemChangeType.REMOVED);

			item.updateDatabase();
			refreshWeight();
		}
		return item;
	}

	/**
	 * Drop item from inventory by using its <b>objectID</b> and updates database
	 * @param process : String Identifier of process triggering this action
	 * @param objectId : int Item Instance identifier of the item to be dropped
	 * @param count : int Quantity of items to be dropped
	 * @param actor : Player Player requesting the item drop
	 * @param reference : Object Object referencing current action like NPC selling item or previous item in transformation
	 * @return Item corresponding to the destroyed item or the updated item in inventory
	 */
	public virtual Item? dropItem(string process, int objectId, long count, Player actor, object? reference)
	{
		Item? item = getItemByObjectId(objectId);
		if (item == null)
		{
			return null;
		}

		lock (item)
		{
			if (!_items.Contains(item))
			{
				return null;
			}

			// Adjust item quantity and create new instance to drop
			// Directly drop entire item
			if (item.getCount() > count)
			{
				item.changeCount(process, -count, actor, reference);
				item.setLastChange(ItemChangeType.MODIFIED);
				item.updateDatabase();

				Item newItem = ItemData.getInstance().createItem(process, item.getId(), count, actor, reference);
				newItem.updateDatabase();
				refreshWeight();
				return newItem;
			}
		}

		return dropItem(process, item, actor, reference);
	}

	/**
	 * Adds item to inventory for further adjustments and Equip it if necessary (itemlocation defined)
	 * @param item : Item to be added from inventory
	 */
	protected override void addItem(Item item)
	{
		base.addItem(item);
		if (item.isEquipped())
		{
			equipItem(item);
		}
	}

	/**
	 * Removes item from inventory for further adjustments.
	 * @param item : Item to be removed from inventory
	 */
	protected override bool removeItem(Item item)
	{
		// Unequip item if equiped
		for (int i = 0; i < _paperdoll.Length; i++)
		{
			if (_paperdoll[i] == item)
			{
				unEquipItemInSlot(i);
			}
		}
		return base.removeItem(item);
	}

	/**
	 * @param slot the slot.
	 * @return the item in the paperdoll slot
	 */
	public Item? getPaperdollItem(int slot)
	{
		return _paperdoll[slot];
	}

	/**
	 * @param slot the slot.
	 * @return {@code true} if specified paperdoll slot is empty, {@code false} otherwise
	 */
	public bool isPaperdollSlotEmpty(int slot)
	{
		return _paperdoll[slot] == null;
	}

	public bool isPaperdollSlotNotEmpty(int slot)
	{
		return _paperdoll[slot] != null;
	}

	public bool isItemEquipped(int itemId)
	{
		foreach (Item? item in _paperdoll)
		{
			if (item != null && item.getId() == itemId)
			{
				return true;
			}
		}
		return false;
	}

	public static int getPaperdollIndex(long slot)
	{
		if (slot == ItemTemplate.SLOT_UNDERWEAR)
		{
			return PAPERDOLL_UNDER;
		}
		else if (slot == ItemTemplate.SLOT_LR_EAR || slot == ItemTemplate.SLOT_R_EAR)
		{
			return PAPERDOLL_REAR;
		}
		else if (slot == ItemTemplate.SLOT_L_EAR)
		{
			return PAPERDOLL_LEAR;
		}
		else if (slot == ItemTemplate.SLOT_NECK)
		{
			return PAPERDOLL_NECK;
		}
		else if (slot == ItemTemplate.SLOT_LR_FINGER || slot == ItemTemplate.SLOT_R_FINGER)
		{
			return PAPERDOLL_RFINGER;
		}
		else if (slot == ItemTemplate.SLOT_L_FINGER)
		{
			return PAPERDOLL_LFINGER;
		}
		else if (slot == ItemTemplate.SLOT_HEAD)
		{
			return PAPERDOLL_HEAD;
		}
		else if (slot == ItemTemplate.SLOT_R_HAND || slot == ItemTemplate.SLOT_LR_HAND)
		{
			return PAPERDOLL_RHAND;
		}
		else if (slot == ItemTemplate.SLOT_L_HAND)
		{
			return PAPERDOLL_LHAND;
		}
		else if (slot == ItemTemplate.SLOT_GLOVES)
		{
			return PAPERDOLL_GLOVES;
		}
		else if (slot == ItemTemplate.SLOT_CHEST || slot == ItemTemplate.SLOT_FULL_ARMOR || slot == ItemTemplate.SLOT_ALLDRESS)
		{
			return PAPERDOLL_CHEST;
		}
		else if (slot == ItemTemplate.SLOT_LEGS)
		{
			return PAPERDOLL_LEGS;
		}
		else if (slot == ItemTemplate.SLOT_FEET)
		{
			return PAPERDOLL_FEET;
		}
		else if (slot == ItemTemplate.SLOT_BACK)
		{
			return PAPERDOLL_CLOAK;
		}
		else if (slot == ItemTemplate.SLOT_HAIR || slot == ItemTemplate.SLOT_HAIRALL)
		{
			return PAPERDOLL_HAIR;
		}
		else if (slot == ItemTemplate.SLOT_HAIR2)
		{
			return PAPERDOLL_HAIR2;
		}
		else if (slot == ItemTemplate.SLOT_R_BRACELET)
		{
			return PAPERDOLL_RBRACELET;
		}
		else if (slot == ItemTemplate.SLOT_L_BRACELET)
		{
			return PAPERDOLL_LBRACELET;
		}
		else if (slot == ItemTemplate.SLOT_DECO)
		{
			return PAPERDOLL_DECO1; // return first we deal with it later
		}
		else if (slot == ItemTemplate.SLOT_BELT)
		{
			return PAPERDOLL_BELT;
		}
		else if (slot == ItemTemplate.SLOT_BROOCH)
		{
			return PAPERDOLL_BROOCH;
		}
		else if (slot == ItemTemplate.SLOT_BROOCH_JEWEL)
		{
			return PAPERDOLL_BROOCH_JEWEL1;
		}
		else if (slot == ItemTemplate.SLOT_AGATHION)
		{
			return PAPERDOLL_AGATHION1;
		}
		else if (slot == ItemTemplate.SLOT_ARTIFACT_BOOK)
		{
			return PAPERDOLL_ARTIFACT_BOOK;
		}
		else if (slot == ItemTemplate.SLOT_ARTIFACT)
		{
			return PAPERDOLL_ARTIFACT1;
		}
		return -1;
	}

	/**
	 * Returns the item in the paperdoll Item slot
	 * @param slot identifier
	 * @return Item
	 */
	public Item? getPaperdollItemByItemId(int slot)
	{
		int index = getPaperdollIndex(slot);
		if (index == -1)
		{
			return null;
		}
		return _paperdoll[index];
	}

	/**
	 * Returns the ID of the item in the paperdoll slot
	 * @param slot : int designating the slot
	 * @return int designating the ID of the item
	 */
	public int getPaperdollItemId(int slot)
	{
		Item? item = _paperdoll[slot];
		if (item != null)
		{
			return item.getId();
		}
		return 0;
	}

	/**
	 * Returns the ID of the item in the paperdoll slot
	 * @param slot : int designating the slot
	 * @return int designating the ID of the item
	 */
	public int getPaperdollItemDisplayId(int slot)
	{
		Item? item = _paperdoll[slot];
		return item != null ? item.getDisplayId() : 0;
	}

	/**
	 * Returns the visual id of the item in the paperdoll slot
	 * @param slot : int designating the slot
	 * @return int designating the ID of the item
	 */
	public int getPaperdollItemVisualId(int slot)
	{
		Item? item = _paperdoll[slot];
		return item != null ? item.getVisualId() : 0;
	}

	public VariationInstance? getPaperdollAugmentation(int slot)
	{
		Item? item = _paperdoll[slot];
		return item != null ? item.getAugmentation() : null;
	}

	/**
	 * Returns the objectID associated to the item in the paperdoll slot
	 * @param slot : int pointing out the slot
	 * @return int designating the objectID
	 */
	public int getPaperdollObjectId(int slot)
	{
		Item? item = _paperdoll[slot];
		return item != null ? item.ObjectId : 0;
	}

	/**
	 * Adds new inventory's paperdoll listener.
	 * @param listener the new listener
	 */
	[MethodImpl(MethodImplOptions.Synchronized)]
	public void addPaperdollListener(PaperdollListener listener)
	{
		if (!_paperdollListeners.Contains(listener))
		{
			_paperdollListeners.Add(listener);
		}
	}

	/**
	 * Removes a paperdoll listener.
	 * @param listener the listener to be deleted
	 */
	[MethodImpl(MethodImplOptions.Synchronized)]
	public void removePaperdollListener(PaperdollListener listener)
	{
		_paperdollListeners.Remove(listener);
	}

	/**
	 * Equips an item in the given slot of the paperdoll.<br>
	 * <u><i>Remark :</i></u> The item <b>must be</b> in the inventory already.
	 * @param slot : int pointing out the slot of the paperdoll
	 * @param item : Item pointing out the item to add in slot
	 * @return Item designating the item placed in the slot before
	 */
	[MethodImpl(MethodImplOptions.Synchronized)]
	public Item? setPaperdollItem(int slot, Item? item)
	{
		Creature? owner = getOwner();
		Item? old = _paperdoll[slot];
		if (old != item)
		{
			if (old != null)
			{
				_paperdoll[slot] = null;
				_paperdollCache.getPaperdollItems().remove(old);

				// Put old item from paperdoll slot to base location
				old.setItemLocation(getBaseLocation());
				old.setLastChange(ItemChangeType.MODIFIED);

				// Get the mask for paperdoll
				ItemTypeMask mask = ItemTypeMask.Zero;
				for (int i = 0; i < PAPERDOLL_TOTALSLOTS; i++)
				{
					Item? pi = _paperdoll[i];
					if (pi != null)
					{
						mask |= pi.getTemplate().getItemMask();
					}
				}
				_wearedMask = mask;

				// Notify all paperdoll listener in order to unequip old item in slot
				foreach (PaperdollListener listener in _paperdollListeners)
				{
					if (listener == null)
					{
						continue;
					}

					listener.notifyUnequiped(slot, old, this);
				}
				old.updateDatabase();

				// Remove agathion skills.
                Player? player = owner?.getActingPlayer();
				if (slot >= PAPERDOLL_AGATHION1 && slot <= PAPERDOLL_AGATHION5 && owner != null && owner.isPlayer() && player != null)
				{
					AgathionSkillHolder? agathionSkills = AgathionData.getInstance().getSkills(old.getId());
					if (agathionSkills != null)
					{
						bool update = false;
						foreach (Skill skill in agathionSkills.getMainSkills(old.getEnchantLevel()))
						{
							player.removeSkill(skill, false, skill.isPassive());
							update = true;
						}
						foreach (Skill skill in agathionSkills.getSubSkills(old.getEnchantLevel()))
						{
							player.removeSkill(skill, false, skill.isPassive());
							update = true;
						}
						if (update)
						{
							player.sendSkillList();
						}
					}
				}
			}

			// Add new item in slot of paperdoll
			if (item != null)
			{
				_paperdoll[slot] = item;
				_paperdollCache.getPaperdollItems().add(item);

				// Put item to equip location
				item.setItemLocation(getEquipLocation(), slot);
				item.setLastChange(ItemChangeType.MODIFIED);

				// Notify all paperdoll listener in order to equip item in slot
				_wearedMask |= item.getTemplate().getItemMask();
				foreach (PaperdollListener listener in _paperdollListeners)
				{
					if (listener == null)
					{
						continue;
					}

					listener.notifyEquiped(slot, item, this);
				}
				item.updateDatabase();

				// Add agathion skills.
                Player? player = owner?.getActingPlayer();
				if (slot >= PAPERDOLL_AGATHION1 && slot <= PAPERDOLL_AGATHION5 && owner != null && owner.isPlayer() && player != null)
				{
					AgathionSkillHolder? agathionSkills = AgathionData.getInstance().getSkills(item.getId());
					if (agathionSkills != null)
					{
						bool update = false;
						if (slot == PAPERDOLL_AGATHION1)
						{
							foreach (Skill skill in agathionSkills.getMainSkills(item.getEnchantLevel()))
							{
								if (skill.isPassive() && !skill.checkConditions(SkillConditionScope.PASSIVE, player, player))
								{
									continue;
								}
								player.addSkill(skill, false);
								update = true;
							}
						}
						foreach (Skill skill in agathionSkills.getSubSkills(item.getEnchantLevel()))
						{
							if (skill.isPassive() && !skill.checkConditions(SkillConditionScope.PASSIVE, player, player))
							{
								continue;
							}
							player.addSkill(skill, false);
							update = true;
						}
						if (update)
						{
							player.sendSkillList();
						}
					}
				}
			}

			_paperdollCache.clearCachedStats();
			owner?.getStat().recalculateStats(!owner.isPlayer());

            Player? ownerPlayer = owner?.getActingPlayer();
			if (owner != null && owner.isPlayer() && ownerPlayer != null)
			{
				owner.sendPacket(new ExUserInfoEquipSlotPacket(ownerPlayer));
			}
		}

		// Notify to scripts
		if (old != null)
		{
            Player? player = owner?.getActingPlayer();
			if (owner != null && owner.isPlayer() && player != null)
			{
				// Proper talisman display on login.
				if (slot == PAPERDOLL_RBRACELET && !player.hasEnteredWorld())
				{
					foreach (ItemSkillHolder skill in old.getTemplate().getAllSkills())
					{
						player.addSkill(skill.getSkill(), false);
					}
				}

				EventContainer itemEvents = old.getTemplate().Events;
				if (itemEvents.HasSubscribers<OnPlayerItemUnequip>())
				{
					itemEvents.NotifyAsync(new OnPlayerItemUnequip(player, old));
				}
			}
		}

		return old;
	}

	/**
	 * @return the mask of wore item
	 */
	public ItemTypeMask getWearedMask()
	{
		return _wearedMask;
	}

	public long getSlotFromItem(Item item)
	{
		long slot = -1;
		int location = item.getLocationSlot();
		switch (location)
		{
			case PAPERDOLL_UNDER:
			{
				slot = ItemTemplate.SLOT_UNDERWEAR;
				break;
			}
			case PAPERDOLL_LEAR:
			{
				slot = ItemTemplate.SLOT_L_EAR;
				break;
			}
			case PAPERDOLL_REAR:
			{
				slot = ItemTemplate.SLOT_R_EAR;
				break;
			}
			case PAPERDOLL_NECK:
			{
				slot = ItemTemplate.SLOT_NECK;
				break;
			}
			case PAPERDOLL_RFINGER:
			{
				slot = ItemTemplate.SLOT_R_FINGER;
				break;
			}
			case PAPERDOLL_LFINGER:
			{
				slot = ItemTemplate.SLOT_L_FINGER;
				break;
			}
			case PAPERDOLL_HAIR:
			{
				slot = ItemTemplate.SLOT_HAIR;
				break;
			}
			case PAPERDOLL_HAIR2:
			{
				slot = ItemTemplate.SLOT_HAIR2;
				break;
			}
			case PAPERDOLL_HEAD:
			{
				slot = ItemTemplate.SLOT_HEAD;
				break;
			}
			case PAPERDOLL_RHAND:
			{
				slot = ItemTemplate.SLOT_R_HAND;
				break;
			}
			case PAPERDOLL_LHAND:
			{
				slot = ItemTemplate.SLOT_L_HAND;
				break;
			}
			case PAPERDOLL_GLOVES:
			{
				slot = ItemTemplate.SLOT_GLOVES;
				break;
			}
			case PAPERDOLL_CHEST:
			{
				slot = item.getTemplate().getBodyPart();
				break;
			}
			case PAPERDOLL_LEGS:
			{
				slot = ItemTemplate.SLOT_LEGS;
				break;
			}
			case PAPERDOLL_CLOAK:
			{
				slot = ItemTemplate.SLOT_BACK;
				break;
			}
			case PAPERDOLL_FEET:
			{
				slot = ItemTemplate.SLOT_FEET;
				break;
			}
			case PAPERDOLL_LBRACELET:
			{
				slot = ItemTemplate.SLOT_L_BRACELET;
				break;
			}
			case PAPERDOLL_RBRACELET:
			{
				slot = ItemTemplate.SLOT_R_BRACELET;
				break;
			}
			case PAPERDOLL_DECO1:
			case PAPERDOLL_DECO2:
			case PAPERDOLL_DECO3:
			case PAPERDOLL_DECO4:
			case PAPERDOLL_DECO5:
			case PAPERDOLL_DECO6:
			{
				slot = ItemTemplate.SLOT_DECO;
				break;
			}
			case PAPERDOLL_BELT:
			{
				slot = ItemTemplate.SLOT_BELT;
				break;
			}
			case PAPERDOLL_BROOCH:
			{
				slot = ItemTemplate.SLOT_BROOCH;
				break;
			}
			case PAPERDOLL_BROOCH_JEWEL1:
			case PAPERDOLL_BROOCH_JEWEL2:
			case PAPERDOLL_BROOCH_JEWEL3:
			case PAPERDOLL_BROOCH_JEWEL4:
			case PAPERDOLL_BROOCH_JEWEL5:
			case PAPERDOLL_BROOCH_JEWEL6:
			{
				slot = ItemTemplate.SLOT_BROOCH_JEWEL;
				break;
			}
			case PAPERDOLL_AGATHION1:
			case PAPERDOLL_AGATHION2:
			case PAPERDOLL_AGATHION3:
			case PAPERDOLL_AGATHION4:
			case PAPERDOLL_AGATHION5:
			{
				slot = ItemTemplate.SLOT_AGATHION;
				break;
			}
			case PAPERDOLL_ARTIFACT_BOOK:
			{
				slot = ItemTemplate.SLOT_ARTIFACT_BOOK;
				break;
			}
			case PAPERDOLL_ARTIFACT1:
			case PAPERDOLL_ARTIFACT2:
			case PAPERDOLL_ARTIFACT3:
			case PAPERDOLL_ARTIFACT4:
			case PAPERDOLL_ARTIFACT5:
			case PAPERDOLL_ARTIFACT6:
			case PAPERDOLL_ARTIFACT7:
			case PAPERDOLL_ARTIFACT8:
			case PAPERDOLL_ARTIFACT9:
			case PAPERDOLL_ARTIFACT10:
			case PAPERDOLL_ARTIFACT11:
			case PAPERDOLL_ARTIFACT12:
			case PAPERDOLL_ARTIFACT13:
			case PAPERDOLL_ARTIFACT14:
			case PAPERDOLL_ARTIFACT15:
			case PAPERDOLL_ARTIFACT16:
			case PAPERDOLL_ARTIFACT17:
			case PAPERDOLL_ARTIFACT18:
			case PAPERDOLL_ARTIFACT19:
			case PAPERDOLL_ARTIFACT20:
			case PAPERDOLL_ARTIFACT21:
			{
				slot = ItemTemplate.SLOT_ARTIFACT;
				break;
			}
		}
		return slot;
	}

	/**
	 * Unequips item in body slot and returns alterations.<br>
	 * <b>If you dont need return value use {@link Inventory#unEquipItemInBodySlot(long)} instead</b>
	 * @param slot : int designating the slot of the paperdoll
	 * @return List<Item> : List of changes
	 */
	public List<Item> unEquipItemInBodySlotAndRecord(long slot)
	{
		ChangeRecorder recorder = newRecorder();
		try
		{
			unEquipItemInBodySlot(slot);
		}
		finally
		{
			removePaperdollListener(recorder);
		}
		return recorder.getChangedItems();
	}

	/**
	 * Sets item in slot of the paperdoll to null value
	 * @param pdollSlot : int designating the slot
	 * @return Item designating the item in slot before change
	 */
	public Item? unEquipItemInSlot(int pdollSlot)
	{
		return setPaperdollItem(pdollSlot, null);
	}

	/**
	 * Unequips item in slot and returns alterations<br>
	 * <b>If you dont need return value use {@link Inventory#unEquipItemInSlot(int)} instead</b>
	 * @param slot : int designating the slot
	 * @return List<Item> : List of items altered
	 */
	public List<Item> unEquipItemInSlotAndRecord(int slot)
	{
		ChangeRecorder recorder = newRecorder();
		try
		{
			unEquipItemInSlot(slot);
		}
		finally
		{
			removePaperdollListener(recorder);
		}
		return recorder.getChangedItems();
	}

	/**
	 * Unequips item in slot (i.e. equips with default value)
	 * @param slot : int designating the slot
	 * @return {@link Item} designating the item placed in the slot
	 */
	public Item? unEquipItemInBodySlot(long slot)
	{
		int pdollSlot = -1;
		if (slot == ItemTemplate.SLOT_L_EAR)
		{
			pdollSlot = PAPERDOLL_LEAR;
		}
		else if (slot == ItemTemplate.SLOT_R_EAR)
		{
			pdollSlot = PAPERDOLL_REAR;
		}
		else if (slot == ItemTemplate.SLOT_NECK)
		{
			pdollSlot = PAPERDOLL_NECK;
		}
		else if (slot == ItemTemplate.SLOT_R_FINGER)
		{
			pdollSlot = PAPERDOLL_RFINGER;
		}
		else if (slot == ItemTemplate.SLOT_L_FINGER)
		{
			pdollSlot = PAPERDOLL_LFINGER;
		}
		else if (slot == ItemTemplate.SLOT_HAIR)
		{
			pdollSlot = PAPERDOLL_HAIR;
		}
		else if (slot == ItemTemplate.SLOT_HAIR2)
		{
			pdollSlot = PAPERDOLL_HAIR2;
		}
		else if (slot == ItemTemplate.SLOT_HAIRALL)
		{
			setPaperdollItem(PAPERDOLL_HAIR, null);
			pdollSlot = PAPERDOLL_HAIR;
		}
		else if (slot == ItemTemplate.SLOT_HEAD)
		{
			pdollSlot = PAPERDOLL_HEAD;
		}
		else if (slot == ItemTemplate.SLOT_R_HAND || slot == ItemTemplate.SLOT_LR_HAND)
		{
			pdollSlot = PAPERDOLL_RHAND;
		}
		else if (slot == ItemTemplate.SLOT_L_HAND)
		{
			pdollSlot = PAPERDOLL_LHAND;
		}
		else if (slot == ItemTemplate.SLOT_GLOVES)
		{
			pdollSlot = PAPERDOLL_GLOVES;
		}
		else if (slot == ItemTemplate.SLOT_CHEST || slot == ItemTemplate.SLOT_ALLDRESS || slot == ItemTemplate.SLOT_FULL_ARMOR)
		{
			pdollSlot = PAPERDOLL_CHEST;
		}
		else if (slot == ItemTemplate.SLOT_LEGS)
		{
			pdollSlot = PAPERDOLL_LEGS;
		}
		else if (slot == ItemTemplate.SLOT_BACK)
		{
			pdollSlot = PAPERDOLL_CLOAK;
		}
		else if (slot == ItemTemplate.SLOT_FEET)
		{
			pdollSlot = PAPERDOLL_FEET;
		}
		else if (slot == ItemTemplate.SLOT_UNDERWEAR)
		{
			pdollSlot = PAPERDOLL_UNDER;
		}
		else if (slot == ItemTemplate.SLOT_L_BRACELET)
		{
			pdollSlot = PAPERDOLL_LBRACELET;
		}
		else if (slot == ItemTemplate.SLOT_R_BRACELET)
		{
			pdollSlot = PAPERDOLL_RBRACELET;
		}
		else if (slot == ItemTemplate.SLOT_DECO)
		{
			pdollSlot = PAPERDOLL_DECO1;
		}
		else if (slot == ItemTemplate.SLOT_BELT)
		{
			pdollSlot = PAPERDOLL_BELT;
		}
		else if (slot == ItemTemplate.SLOT_BROOCH)
		{
			pdollSlot = PAPERDOLL_BROOCH;
		}
		else if (slot == ItemTemplate.SLOT_BROOCH_JEWEL)
		{
			pdollSlot = PAPERDOLL_BROOCH_JEWEL1;
		}
		else if (slot == ItemTemplate.SLOT_AGATHION)
		{
			pdollSlot = PAPERDOLL_AGATHION1;
		}
		else if (slot == ItemTemplate.SLOT_ARTIFACT_BOOK)
		{
			pdollSlot = PAPERDOLL_ARTIFACT_BOOK;
		}
		else if (slot == ItemTemplate.SLOT_ARTIFACT)
		{
			pdollSlot = PAPERDOLL_ARTIFACT1;
		}
		else
		{
			LOGGER.Info("Unhandled slot type: " + slot);
		}
		if (pdollSlot >= 0)
		{
			return setPaperdollItem(pdollSlot, null);
		}
		return null;
	}

	/**
	 * Equips item and returns list of alterations<br>
	 * <b>If you don't need return value use {@link Inventory#equipItem(Item)} instead</b>
	 * @param item : Item corresponding to the item
	 * @return List<Item> : List of alterations
	 */
	public List<Item> equipItemAndRecord(Item item)
	{
		ChangeRecorder recorder = newRecorder();
		try
		{
			equipItem(item);
		}
		finally
		{
			removePaperdollListener(recorder);
		}
		return recorder.getChangedItems();
	}

	/**
	 * Equips item in slot of paperdoll.
	 * @param item : Item designating the item and slot used.
	 */
	public void equipItem(Item item)
	{
        Player? player = (Player?)getOwner();
		if (player != null)
		{
			if (player.getPrivateStoreType() != PrivateStoreType.NONE)
			{
				return;
			}

			if (!player.canOverrideCond(PlayerCondOverride.ITEM_CONDITIONS) && !player.isHero() && item.isHeroItem())
			{
				return;
			}
		}

		long targetSlot = item.getTemplate().getBodyPart();

		// Check if player is using Formal Wear and item isn't Wedding Bouquet.
		Item? formal = getPaperdollItem(PAPERDOLL_CHEST);
		if (item.getId() != 21163 && formal != null && formal.getTemplate().getBodyPart() == ItemTemplate.SLOT_ALLDRESS)
		{
			// only chest target can pass this
			if (targetSlot == ItemTemplate.SLOT_LR_HAND || targetSlot == ItemTemplate.SLOT_L_HAND || targetSlot == ItemTemplate.SLOT_R_HAND || targetSlot == ItemTemplate.SLOT_LEGS || targetSlot == ItemTemplate.SLOT_FEET || targetSlot == ItemTemplate.SLOT_GLOVES || targetSlot == ItemTemplate.SLOT_HEAD)
			{
				return;
			}
		}

		// handle full armor
		// formal dress
		if (targetSlot == ItemTemplate.SLOT_LR_HAND)
		{
			Item? lh = getPaperdollItem(PAPERDOLL_LHAND);
            Armor? armor = lh?.getArmorItem();
			if (lh != null && lh.isArmor() && armor != null && armor.getItemType() == ArmorType.SHIELD)
			{
				setPaperdollItem(PAPERDOLL_LHAND, null);
			}
			setPaperdollItem(PAPERDOLL_RHAND, item);
		}
		else if (targetSlot == ItemTemplate.SLOT_L_HAND)
		{
			Item? rh = getPaperdollItem(PAPERDOLL_RHAND);
			if (rh != null && rh.getTemplate().getBodyPart() == ItemTemplate.SLOT_LR_HAND && !(rh.getItemType() == WeaponType.FISHINGROD && item.getItemType() == EtcItemType.LURE))
            {
                Armor? armor = item.getArmorItem();
				if (!item.isArmor() || armor == null || armor.getItemType() != ArmorType.SIGIL)
				{
					setPaperdollItem(PAPERDOLL_RHAND, null);
				}
			}
			setPaperdollItem(PAPERDOLL_LHAND, item);
		}
		else if (targetSlot == ItemTemplate.SLOT_R_HAND)
		{
			setPaperdollItem(PAPERDOLL_RHAND, item);
		}
		else if (targetSlot == ItemTemplate.SLOT_L_EAR || targetSlot == ItemTemplate.SLOT_R_EAR || targetSlot == ItemTemplate.SLOT_LR_EAR)
		{
			if (_paperdoll[PAPERDOLL_LEAR] == null)
			{
				setPaperdollItem(PAPERDOLL_LEAR, item);
			}
			else if (_paperdoll[PAPERDOLL_REAR] == null)
			{
				setPaperdollItem(PAPERDOLL_REAR, item);
			}
			else
			{
				setPaperdollItem(PAPERDOLL_LEAR, item);
			}
		}
		else if (targetSlot == ItemTemplate.SLOT_L_FINGER || targetSlot == ItemTemplate.SLOT_R_FINGER || targetSlot == ItemTemplate.SLOT_LR_FINGER)
		{
			if (_paperdoll[PAPERDOLL_LFINGER] == null)
			{
				setPaperdollItem(PAPERDOLL_LFINGER, item);
			}
			else if (_paperdoll[PAPERDOLL_RFINGER] == null)
			{
				setPaperdollItem(PAPERDOLL_RFINGER, item);
			}
			else
			{
				setPaperdollItem(PAPERDOLL_LFINGER, item);
			}
		}
		else if (targetSlot == ItemTemplate.SLOT_NECK)
		{
			setPaperdollItem(PAPERDOLL_NECK, item);
		}
		else if (targetSlot == ItemTemplate.SLOT_FULL_ARMOR)
		{
			setPaperdollItem(PAPERDOLL_LEGS, null);
			setPaperdollItem(PAPERDOLL_CHEST, item);
		}
		else if (targetSlot == ItemTemplate.SLOT_CHEST)
		{
			setPaperdollItem(PAPERDOLL_CHEST, item);
		}
		else if (targetSlot == ItemTemplate.SLOT_LEGS)
		{
			Item? chest = getPaperdollItem(PAPERDOLL_CHEST);
			if (chest != null && chest.getTemplate().getBodyPart() == ItemTemplate.SLOT_FULL_ARMOR)
			{
				setPaperdollItem(PAPERDOLL_CHEST, null);
			}
			setPaperdollItem(PAPERDOLL_LEGS, item);
		}
		else if (targetSlot == ItemTemplate.SLOT_FEET)
		{
			setPaperdollItem(PAPERDOLL_FEET, item);
		}
		else if (targetSlot == ItemTemplate.SLOT_GLOVES)
		{
			setPaperdollItem(PAPERDOLL_GLOVES, item);
		}
		else if (targetSlot == ItemTemplate.SLOT_HEAD)
		{
			setPaperdollItem(PAPERDOLL_HEAD, item);
		}
		else if (targetSlot == ItemTemplate.SLOT_HAIR)
		{
			Item? hair = getPaperdollItem(PAPERDOLL_HAIR);
			if (hair != null && hair.getTemplate().getBodyPart() == ItemTemplate.SLOT_HAIRALL)
			{
				setPaperdollItem(PAPERDOLL_HAIR2, null);
			}
			else
			{
				setPaperdollItem(PAPERDOLL_HAIR, null);
			}
			setPaperdollItem(PAPERDOLL_HAIR, item);
		}
		else if (targetSlot == ItemTemplate.SLOT_HAIR2)
		{
			Item? hair2 = getPaperdollItem(PAPERDOLL_HAIR);
			if (hair2 != null && hair2.getTemplate().getBodyPart() == ItemTemplate.SLOT_HAIRALL)
			{
				setPaperdollItem(PAPERDOLL_HAIR, null);
			}
			else
			{
				setPaperdollItem(PAPERDOLL_HAIR2, null);
			}
			setPaperdollItem(PAPERDOLL_HAIR2, item);
		}
		else if (targetSlot == ItemTemplate.SLOT_HAIRALL)
		{
			setPaperdollItem(PAPERDOLL_HAIR2, null);
			setPaperdollItem(PAPERDOLL_HAIR, item);
		}
		else if (targetSlot == ItemTemplate.SLOT_UNDERWEAR)
		{
			setPaperdollItem(PAPERDOLL_UNDER, item);
		}
		else if (targetSlot == ItemTemplate.SLOT_BACK)
		{
			setPaperdollItem(PAPERDOLL_CLOAK, item);
		}
		else if (targetSlot == ItemTemplate.SLOT_L_BRACELET)
		{
			setPaperdollItem(PAPERDOLL_LBRACELET, item);
		}
		else if (targetSlot == ItemTemplate.SLOT_R_BRACELET)
		{
			setPaperdollItem(PAPERDOLL_RBRACELET, item);
		}
		else if (targetSlot == ItemTemplate.SLOT_DECO)
		{
			equipTalisman(item);
		}
		else if (targetSlot == ItemTemplate.SLOT_BELT)
		{
			setPaperdollItem(PAPERDOLL_BELT, item);
		}
		else if (targetSlot == ItemTemplate.SLOT_ALLDRESS)
		{
			setPaperdollItem(PAPERDOLL_LEGS, null);
			setPaperdollItem(PAPERDOLL_LHAND, null);
			setPaperdollItem(PAPERDOLL_RHAND, null);
			setPaperdollItem(PAPERDOLL_HEAD, null);
			setPaperdollItem(PAPERDOLL_FEET, null);
			setPaperdollItem(PAPERDOLL_GLOVES, null);
			setPaperdollItem(PAPERDOLL_CHEST, item);
		}
		else if (targetSlot == ItemTemplate.SLOT_BROOCH)
		{
			setPaperdollItem(PAPERDOLL_BROOCH, item);
		}
		else if (targetSlot == ItemTemplate.SLOT_BROOCH_JEWEL)
		{
			equipBroochJewel(item);
		}
		else if (targetSlot == ItemTemplate.SLOT_AGATHION)
		{
			equipAgathion(item);
		}
		else if (targetSlot == ItemTemplate.SLOT_ARTIFACT_BOOK)
		{
			setPaperdollItem(PAPERDOLL_ARTIFACT_BOOK, item);
		}
		else if (targetSlot == ItemTemplate.SLOT_ARTIFACT)
		{
			equipArtifact(item);
		}
		else
		{
			LOGGER.Warn("Unknown body slot " + targetSlot + " for Item ID: " + item.getId());
		}
	}

	/**
	 * Refresh the weight of equipment loaded
	 */
	protected override void refreshWeight()
	{
		long weight = 0;
		foreach (Item item in _items)
		{
			if (item != null && item.getTemplate() != null)
			{
				weight += item.getTemplate().getWeight() * item.getCount();
			}
		}
		_totalWeight = (int) Math.Min(weight, int.MaxValue);
	}

	/**
	 * @return the totalWeight.
	 */
	public int getTotalWeight()
	{
		return _totalWeight;
	}

	/**
	 * Reduce the arrow number of the Creature.<br>
	 * <br>
	 * <b><u>Overridden in</u>:</b>
	 * <li>Player</li><br>
	 * @param type
	 */
	public virtual void reduceAmmunitionCount(EtcItemType type)
	{
		// Default is to do nothing.
	}

	/**
	 * Return the Item of the arrows needed for this bow.
	 * @param bow : Item designating the bow
	 * @return Item pointing out arrows for bow
	 */
	public Item? findArrowForBow(ItemTemplate bow)
	{
		if (bow == null)
		{
			return null;
		}

		Item? arrow = null;
		foreach (Item item in _items)
        {
            EtcItem? etcItem = item.getEtcItem();
			if (item.isEtcItem() && etcItem != null && etcItem.getItemType() == EtcItemType.ARROW && item.getTemplate().getCrystalTypePlus() == bow.getCrystalTypePlus())
			{
				arrow = item;
				break;
			}
		}

		// Get the Item corresponding to the item identifier and return it
		return arrow;
	}

    /**
     * Return the Item of the bolts needed for this crossbow.
     * @param crossbow : Item designating the crossbow
     * @return Item pointing out bolts for crossbow
     */
    public Item? findBoltForCrossBow(ItemTemplate crossbow)
    {
        Item? bolt = null;
        foreach (Item item in _items)
        {
            EtcItem? etcItem = item.getEtcItem();
            if (item.isEtcItem() && etcItem != null && etcItem.getItemType() == EtcItemType.BOLT &&
                item.getTemplate().getCrystalTypePlus() == crossbow.getCrystalTypePlus())
            {
                bolt = item;
                break;
            }
        }

        // Get the Item corresponding to the item identifier and return it
        return bolt;
    }

    /**
     * Return the Item of the bolts needed for these pistols.
     * @param pistols : Item designating the pistols
     * @return Item pointing out elemental orb for pistols
     */
	public Item? findElementalOrbForPistols(ItemTemplate pistols)
	{
		Item? orb = null;
        foreach (Item item in _items)
        {
            EtcItem? etcItem = item.getEtcItem();
            if (item.isEtcItem() && etcItem != null && etcItem.getItemType() == EtcItemType.ELEMENTAL_ORB &&
                item.getTemplate().getCrystalTypePlus() == pistols.getCrystalTypePlus())
            {
                orb = item;
                break;
            }
        }

        // Get the Item corresponding to the item identifier and return it
		return orb;
	}

	/**
	 * Get back items in inventory from database
	 */
	public override void restore()
	{
		try
		{
			using GameServerDbContext ctx = DbFactory.Instance.CreateDbContext();
			int ownerId = getOwnerId();
			ItemLocation baseLocation = getBaseLocation();
			ItemLocation equipLocation = getEquipLocation();

			var query = ctx.Items.Where(r =>
					r.OwnerId == ownerId && (r.Location == (int)baseLocation || r.Location == (int)equipLocation))
				.OrderBy(r => r.LocationData);

			foreach (var record in query)
			{
				try
				{
					Item item = new Item(record);
                    Player? player = (Player?)getOwner();
					if (player != null)
					{
						if (!player.canOverrideCond(PlayerCondOverride.ITEM_CONDITIONS) && !player.isHero() &&
						    item.isHeroItem())
						{
							item.setItemLocation(ItemLocation.INVENTORY);
						}
					}

					World.getInstance().addObject(item);

					// If stackable item is found in inventory just add to current quantity
					if (item.isStackable() && getItemByItemId(item.getId()) != null)
					{
						addItem("Restore", item, getOwner()?.getActingPlayer(), null);
					}
					else
					{
						addItem(item);
					}
				}
				catch (Exception e)
				{
					LOGGER.Warn("Could not restore item " + record.ItemId + " for " + getOwner() + ": " + e);
				}
			}

			refreshWeight();
		}
		catch (Exception e)
		{
			LOGGER.Warn("Could not restore inventory: " + e);
		}
	}

	public int getTalismanSlots()
	{
		return getOwner()?.getActingPlayer()?.getStat().getTalismanSlots() ?? 0;
	}

	private void equipTalisman(Item item)
	{
		if (getTalismanSlots() == 0)
		{
			return;
		}

		// find same (or incompatible) talisman type
		for (int i = PAPERDOLL_DECO1; i < PAPERDOLL_DECO1 + getTalismanSlots(); i++)
		{
			if (_paperdoll[i] != null && getPaperdollItemId(i) == item.getId())
			{
				// overwrite
				setPaperdollItem(i, item);
				return;
			}
		}

		// no free slot found - put on first free
		for (int i = PAPERDOLL_DECO1; i < PAPERDOLL_DECO1 + getTalismanSlots(); i++)
		{
			if (_paperdoll[i] == null)
			{
				setPaperdollItem(i, item);
				return;
			}
		}
	}

	public int getArtifactSlots()
	{
		return getOwner()?.getActingPlayer()?.getStat().getArtifactSlots() ?? 0;
	}

	private void equipArtifact(Item item)
	{
		int slotNumber = getArtifactSlots();
		if (slotNumber == 0)
		{
			return;
		}

		switch (item.getTemplate().getArtifactSlot())
		{
			case 1: // Attack
			{
				for (int i = PAPERDOLL_ARTIFACT13; i < PAPERDOLL_ARTIFACT13 + slotNumber; i++)
				{
					if (i <= PAPERDOLL_ARTIFACT15 && _paperdoll[i] == null)
					{
						setPaperdollItem(i, item);
						return;
					}
				}
				break;
			}
			case 2: // Protection
			{
				for (int i = PAPERDOLL_ARTIFACT16; i < PAPERDOLL_ARTIFACT16 + slotNumber; i++)
				{
					if (i <= PAPERDOLL_ARTIFACT18 && _paperdoll[i] == null)
					{
						setPaperdollItem(i, item);
						return;
					}
				}
				break;
			}
			case 3: // Support
			{
				for (int i = PAPERDOLL_ARTIFACT19; i < PAPERDOLL_ARTIFACT19 + slotNumber; i++)
				{
					if (i <= PAPERDOLL_ARTIFACT21 && _paperdoll[i] == null)
					{
						setPaperdollItem(i, item);
						return;
					}
				}
				break;
			}
			case 4: // Balance
			{
				for (int i = PAPERDOLL_ARTIFACT1; i < PAPERDOLL_ARTIFACT1 + 4 * slotNumber; i++)
				{
					if (i <= PAPERDOLL_ARTIFACT12 && _paperdoll[i] == null)
					{
						setPaperdollItem(i, item);
						return;
					}
				}
				break;
			}
		}
	}

	public int getBroochJewelSlots()
	{
		return getOwner()?.getActingPlayer()?.getStat().getBroochJewelSlots() ?? 0;
	}

	private void equipBroochJewel(Item item)
	{
		if (getBroochJewelSlots() == 0)
		{
			return;
		}

		// find same (or incompatible) brooch jewel type
		for (int i = PAPERDOLL_BROOCH_JEWEL1; i < PAPERDOLL_BROOCH_JEWEL1 + getBroochJewelSlots(); i++)
		{
			if (_paperdoll[i] != null && getPaperdollItemId(i) == item.getId())
			{
				// overwrite
				setPaperdollItem(i, item);
				return;
			}
		}

		// no free slot found - put on first free
		for (int i = PAPERDOLL_BROOCH_JEWEL1; i < PAPERDOLL_BROOCH_JEWEL1 + getBroochJewelSlots(); i++)
		{
			if (_paperdoll[i] == null)
			{
				setPaperdollItem(i, item);
				return;
			}
		}
	}

	public int getAgathionSlots()
	{
		return getOwner()?.getActingPlayer()?.getStat().getAgathionSlots() ?? 0;
	}

	private void equipAgathion(Item item)
	{
		if (getAgathionSlots() == 0)
		{
			return;
		}

		// find same (or incompatible) agathion type
		for (int i = PAPERDOLL_AGATHION1; i < PAPERDOLL_AGATHION1 + getAgathionSlots(); i++)
		{
			if (_paperdoll[i] != null && getPaperdollItemId(i) == item.getId())
			{
				// overwrite
				setPaperdollItem(i, item);
				return;
			}
		}

		// no free slot found - put on first free
		for (int i = PAPERDOLL_AGATHION1; i < PAPERDOLL_AGATHION1 + getAgathionSlots(); i++)
		{
			if (_paperdoll[i] == null)
			{
				setPaperdollItem(i, item);
				return;
			}
		}
	}

	public bool canEquipCloak()
	{
		return getOwner()?.getActingPlayer()?.getStat().canEquipCloak() == true;
	}

	/**
	 * Re-notify to paperdoll listeners every equipped item.<br>
	 * Only used by player ClassId set methods.
	 */
	public void reloadEquippedItems()
	{
		int slot;
		foreach (Item? item in _paperdoll)
		{
			if (item == null)
			{
				continue;
			}

			slot = item.getLocationSlot();
			foreach (PaperdollListener listener in _paperdollListeners)
			{
				if (listener == null)
				{
					continue;
				}

				listener.notifyUnequiped(slot, item, this);
				listener.notifyEquiped(slot, item, this);
			}
		}

        Player? player = (Player?)getOwner();
		if (player != null)
		{
            player.sendPacket(new ExUserInfoEquipSlotPacket(player));
		}
	}

	public int getArmorMinEnchant()
	{
        Playable? player = (Playable?)getOwner();
		if (player == null)
		{
			return 0;
		}

		return _paperdollCache.getMaxSetEnchant(player);
	}

	public int getWeaponEnchant()
	{
		Item? item = getPaperdollItem(PAPERDOLL_RHAND);
		return item != null ? item.getEnchantLevel() : 0;
	}

	/**
	 * Blocks the given item slot from being equipped.
	 * @param itemSlot mask from Item
	 */
	public void blockItemSlot(long itemSlot)
	{
		_blockedItemSlotsMask |= itemSlot;
	}

	/**
	 * Unblocks the given item slot so it can be equipped.
	 * @param itemSlot mask from Item
	 */
	public void unblockItemSlot(long itemSlot)
	{
		_blockedItemSlotsMask &= ~itemSlot;
	}

	/**
	 * @param itemSlot mask from Item
	 * @return if the given item slot is blocked or not.
	 */
	public bool isItemSlotBlocked(long itemSlot)
	{
		return (_blockedItemSlotsMask & itemSlot) == itemSlot;
	}

	/**
	 * @param itemSlotsMask use 0 to unset all blocked item slots.
	 */
	public void setBlockedItemSlotsMask(long itemSlotsMask)
	{
		_blockedItemSlotsMask = itemSlotsMask;
	}

	/**
	 * Gets the items in paperdoll slots filtered by filter.
	 * @param filters multiple filters
	 * @return the filtered items in inventory
	 */
	public ICollection<Item> getPaperdollItems(params Predicate<Item>[] filters)
	{
		if (filters.Length == 0)
		{
			return _paperdollCache.getPaperdollItems();
		}

		Predicate<Item> filter = x => filters.All(f => f(x));
		List<Item> items = [];
		foreach (Item? item in _paperdoll)
		{
			if (item != null && filter(item))
			{
				items.Add(item);
			}
		}
		return items;
	}

	public int getPaperdollItemCount(params Predicate<Item>[] filters)
	{
		if (filters.Length == 0)
		{
			return _paperdollCache.getPaperdollItems().size();
		}

		Predicate<Item?> filter = x => x is not null && filters.All(f => f(x));
		int count = 0;
		foreach (Item? item in _paperdoll)
		{
			if (filter(item))
			{
				count++;
			}
		}
		return count;
	}

	public PaperdollCache getPaperdollCache()
	{
		return _paperdollCache;
	}
}