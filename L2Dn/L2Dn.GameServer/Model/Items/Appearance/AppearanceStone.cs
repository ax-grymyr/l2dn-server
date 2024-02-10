using L2Dn.GameServer.Data.Xml;
using L2Dn.GameServer.Enums;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Holders;
using L2Dn.GameServer.Model.Items.Instances;
using L2Dn.GameServer.Model.Items.Types;
using L2Dn.GameServer.Network.Enums;
using L2Dn.GameServer.Utilities;

namespace L2Dn.GameServer.Model.Items.Appearance;

/**
 * @author UnAfraid
 */
public class AppearanceStone
{
	private readonly int _id;
	private readonly int _cost;
	private readonly int _visualId;
	private readonly TimeSpan _lifeTime;
	private readonly AppearanceType _type;
	private readonly WeaponType _weaponType;
	private readonly ArmorType _armorType;
	private readonly AppearanceHandType _handType;
	private readonly AppearanceMagicType _magicType;
	private Set<CrystalType> _crystalTypes;
	private Set<AppearanceTargetType> _targetTypes;
	private Set<long> _bodyParts;
	private Set<Race> _races;
	private Set<Race> _racesNot;
	private Set<AppearanceHolder> _allVisualIds;

	public AppearanceStone(StatSet set)
	{
		_id = set.getInt("id");
		_visualId = set.getInt("visualId", 0);
		_cost = set.getInt("cost", 0);
		_lifeTime = set.getDuration("lifeTime", TimeSpan.Zero);
		_type = set.getEnum("type", AppearanceType.NONE);
		_weaponType = set.getEnum("weaponType", WeaponType.NONE);
		_armorType = set.getEnum("armorType", ArmorType.NONE);
		_handType = set.getEnum("handType", AppearanceHandType.NONE);
		_magicType = set.getEnum("magicType", AppearanceMagicType.NONE);

		AppearanceTargetType targetType = set.getEnum("targetType", AppearanceTargetType.NONE);
		if (targetType != AppearanceTargetType.NONE)
		{
			addTargetType(targetType);
		}

		// No grade items cannot change appearance, because client doesn't have No-Grade restoration stones.
		CrystalType crystalType = set.getEnum("grade", CrystalType.NONE);

		// If no crystal type is defined, we must add all defaults.
		if (crystalType == null)
		{
			switch (targetType)
			{
				case AppearanceTargetType.ACCESSORY:
				case AppearanceTargetType.ALL:
				{
					addCrystalType(CrystalType.NONE);
					// fallthrough
					goto case AppearanceTargetType.ARMOR;
				}
				case AppearanceTargetType.WEAPON:
				case AppearanceTargetType.ARMOR:
				{
					foreach (CrystalType cryType in Enum.GetValues<CrystalType>())
					{
						if ((cryType != CrystalType.NONE) && (cryType != CrystalType.EVENT))
						{
							addCrystalType(cryType);
						}
					}

					break;
				}
			}
		}
		else
		{
			addCrystalType(crystalType);
		}

		long bodyPart = ItemData.SLOTS.get(set.getString("bodyPart", "none"));
		if (bodyPart != ItemTemplate.SLOT_NONE)
		{
			addBodyPart(bodyPart);
		}

		Race race = set.getEnum("race", Race.NONE);
		if (race != Race.NONE)
		{
			addRace(race);
		}

		Race raceNot = set.getEnum("raceNot", Race.NONE);
		if (raceNot != Race.NONE)
		{
			addRaceNot(raceNot);
		}
	}

	public int getId()
	{
		return _id;
	}

	public int getVisualId()
	{
		return _visualId;
	}

	public int getCost()
	{
		return _cost;
	}

	public long getLifeTime()
	{
		return _lifeTime;
	}

	public AppearanceType getType()
	{
		return _type;
	}

	public WeaponType getWeaponType()
	{
		return _weaponType;
	}

	public ArmorType getArmorType()
	{
		return _armorType;
	}

	public AppearanceHandType getHandType()
	{
		return _handType;
	}

	public AppearanceMagicType getMagicType()
	{
		return _magicType;
	}

	public void addCrystalType(CrystalType type)
	{
		if (_crystalTypes == null)
		{
			_crystalTypes = new();
		}

		_crystalTypes.add(type);
	}

	public Set<CrystalType> getCrystalTypes()
	{
		return _crystalTypes != null ? _crystalTypes : new();
	}

	public void addTargetType(AppearanceTargetType type)
	{
		if (_targetTypes == null)
		{
			_targetTypes = new();
		}

		_targetTypes.add(type);
	}

	public Set<AppearanceTargetType> getTargetTypes()
	{
		return _targetTypes != null ? _targetTypes : new();
	}

	public void addBodyPart(long part)
	{
		if (_bodyParts == null)
		{
			_bodyParts = new HashSet<>();
		}

		_bodyParts.add(part);
	}

	public void addVisualId(AppearanceHolder appearanceHolder)
	{
		if (_allVisualIds == null)
		{
			_allVisualIds = new();
		}

		_allVisualIds.add(appearanceHolder);
	}

	public Set<AppearanceHolder> getVisualIds()
	{
		return _allVisualIds != null ? _allVisualIds : new();
	}

	public Set<long> getBodyParts()
	{
		return _bodyParts != null ? _bodyParts : new();
	}

	public void addRace(Race race)
	{
		if (_races == null)
		{
			_races = new();
		}

		_races.add(race);
	}

	public Set<Race> getRaces()
	{
		return _races != null ? _races : new();
	}

	public void addRaceNot(Race race)
	{
		if (_racesNot == null)
		{
			_racesNot = new();
		}

		_racesNot.add(race);
	}

	public Set<Race> getRacesNot()
	{
		return _racesNot != null ? _racesNot : new();
	}

	/**
	 * @param player the actor requesting to use this appearance.
	 * @param targetItem the item to be modified with this appearance.
	 * @return {@code true} if the item is valid for appearance change, {@code false} otherwise.
	 */
	public bool checkConditions(Player player, Item targetItem)
	{
		if (targetItem == null)
		{
			return false;
		}

		if (getTargetTypes().isEmpty())
		{
			return false;
		}

		if (targetItem.isEquipped() && (getRacesNot().Contains(player.getRace()) ||
		                                (!getRaces().isEmpty() && !getRaces().Contains(player.getRace()))))
		{
			player.sendPacket(SystemMessageId
				.YOU_CANNOT_MODIFY_AN_EQUIPPED_ITEM_INTO_THE_APPEARANCE_OF_AN_UNEQUIPPABLE_ITEM_PLEASE_CHECK_RACE_GENDER_RESTRICTIONS_YOU_CAN_MODIFY_THE_APPEARANCE_IF_YOU_UNEQUIP_THE_ITEM);
			return false;
		}

		switch (_type)
		{
			case AppearanceType.RESTORE:
			{
				if (targetItem.getVisualId() == 0)
				{
					player.sendPacket(SystemMessageId.YOU_CANNOT_RESTORE_ITEMS_THAT_HAVE_NOT_BEEN_MODIFIED);
					return false;
				}

				if ((targetItem.isWeapon() && !getTargetTypes().Contains(AppearanceTargetType.WEAPON)) ||
				    (targetItem.isArmor() && !getTargetTypes().Contains(AppearanceTargetType.ARMOR) &&
				     !((targetItem.getTemplate().getBodyPart() == ItemTemplate.SLOT_HAIR) ||
				       (targetItem.getTemplate().getBodyPart() == ItemTemplate.SLOT_HAIR2) ||
				       (targetItem.getTemplate().getBodyPart() == ItemTemplate.SLOT_HAIRALL))) ||
				    (targetItem.isEtcItem() && !getTargetTypes().Contains(AppearanceTargetType.ACCESSORY)))
				{
					player.sendPacket(SystemMessageId.THIS_ITEM_DOES_NOT_MEET_REQUIREMENTS);
					return false;
				}

				if (((targetItem.getTemplate().getBodyPart() == ItemTemplate.SLOT_HAIR) ||
				     (targetItem.getTemplate().getBodyPart() == ItemTemplate.SLOT_HAIR2) ||
				     (targetItem.getTemplate().getBodyPart() == ItemTemplate.SLOT_HAIRALL)) &&
				    !getTargetTypes().Contains(AppearanceTargetType.ACCESSORY))
				{
					player.sendPacket(SystemMessageId.THIS_ITEM_DOES_NOT_MEET_REQUIREMENTS);
					return false;
				}

				break;
			}
			default:
			{
				// Seems like in retail item with already changed appearance, can be changed again without being restored.

				AppearanceTargetType targetType = getTargetTypes().stream().findFirst().get();
				switch (targetType)
				{
					case AppearanceTargetType.NONE:
					{
						return false;
					}
					case AppearanceTargetType.WEAPON:
					{
						if (!targetItem.isWeapon())
						{
							player.sendPacket(SystemMessageId.WEAPONS_ONLY);
							return false;
						}

						if (targetItem.getTemplate().getCrystalType() == CrystalType.NONE)
						{
							player.sendPacket(SystemMessageId.YOU_CANNOT_MODIFY_OR_RESTORE_NO_GRADE_ITEMS);
							return false;
						}

						break;
					}
					case AppearanceTargetType.ARMOR:
					{
						if (!targetItem.isArmor())
						{
							player.sendPacket(SystemMessageId.ARMOR_ONLY);
							return false;
						}

						if (targetItem.getTemplate().getCrystalType() == CrystalType.NONE)
						{
							player.sendPacket(SystemMessageId.YOU_CANNOT_MODIFY_OR_RESTORE_NO_GRADE_ITEMS);
							return false;
						}

						break;
					}
					case AppearanceTargetType.ACCESSORY:
					{
						if ((targetItem.getTemplate().getBodyPart() != ItemTemplate.SLOT_HAIR) &&
						    (targetItem.getTemplate().getBodyPart() != ItemTemplate.SLOT_HAIR2) &&
						    (targetItem.getTemplate().getBodyPart() != ItemTemplate.SLOT_HAIRALL))
						{
							player.sendPacket(SystemMessageId.HEAD_ACCESSORIES_ONLY);
							return false;
						}

						break;
					}
					case AppearanceTargetType.ALL:
					{
						if (!getCrystalTypes().isEmpty() &&
						    !getCrystalTypes().Contains(targetItem.getTemplate().getCrystalType()))
						{
							player.sendPacket(SystemMessageId.THIS_ITEM_DOES_NOT_MEET_REQUIREMENTS);
							return false;
						}

						if (findVisualChange(targetItem) == null)
						{
							player.sendPacket(SystemMessageId.THIS_ITEM_DOES_NOT_MEET_REQUIREMENTS);
							return false;
						}

						return true;
					}
				}

				break;
			}
		}

		if (!getCrystalTypes().isEmpty() && !getCrystalTypes().Contains(targetItem.getTemplate().getCrystalType()))
		{
			player.sendPacket(SystemMessageId.THIS_ITEM_DOES_NOT_MEET_REQUIREMENTS);
			return false;
		}

		if (targetItem.isArmor() && !getBodyParts().isEmpty() &&
		    !getBodyParts().Contains(targetItem.getTemplate().getBodyPart()))
		{
			player.sendPacket(SystemMessageId.THIS_ITEM_DOES_NOT_MEET_REQUIREMENTS);
			return false;
		}

		if (_weaponType != WeaponType.NONE)
		{
			if (!targetItem.isWeapon() || (targetItem.getItemType() != _weaponType))
			{
				if (_weaponType != WeaponType.CROSSBOW)
				{
					player.sendPacket(SystemMessageId.THIS_ITEM_CANNOT_BE_EXTRACTED);
					return false;
				}
				else if ((targetItem.getItemType() != WeaponType.CROSSBOW) &&
				         (targetItem.getItemType() != WeaponType.TWOHANDCROSSBOW))
				{
					player.sendPacket(SystemMessageId.THIS_ITEM_CANNOT_BE_EXTRACTED);
					return false;
				}
			}

			switch (_handType)
			{
				case AppearanceHandType.ONE_HANDED:
				{
					if ((targetItem.getTemplate().getBodyPart() & ItemTemplate.SLOT_R_HAND) != ItemTemplate.SLOT_R_HAND)
					{
						player.sendPacket(SystemMessageId.THIS_ITEM_DOES_NOT_MEET_REQUIREMENTS);
						return false;
					}

					break;
				}
				case AppearanceHandType.TWO_HANDED:
				{
					if ((targetItem.getTemplate().getBodyPart() & ItemTemplate.SLOT_LR_HAND) !=
					    ItemTemplate.SLOT_LR_HAND)
					{
						player.sendPacket(SystemMessageId.THIS_ITEM_DOES_NOT_MEET_REQUIREMENTS);
						return false;
					}

					break;
				}
			}

			switch (_magicType)
			{
				case AppearanceMagicType.MAGICAL:
				{
					if (!targetItem.getTemplate().isMagicWeapon())
					{
						player.sendPacket(SystemMessageId.THIS_ITEM_DOES_NOT_MEET_REQUIREMENTS);
						return false;
					}

					break;
				}
				case AppearanceMagicType.PHYISICAL:
				{
					if (targetItem.getTemplate().isMagicWeapon())
					{
						player.sendPacket(SystemMessageId.THIS_ITEM_DOES_NOT_MEET_REQUIREMENTS);
						return false;
					}
				}
			}
		}

		if (_armorType != ArmorType.NONE)
		{
			switch (_armorType)
			{
				case ArmorType.SHIELD:
				{
					if (!targetItem.isArmor() || (targetItem.getItemType() != ArmorType.SHIELD))
					{
						player.sendPacket(SystemMessageId.THIS_ITEM_DOES_NOT_MEET_REQUIREMENTS);
						return false;
					}

					break;
				}
				case ArmorType.SIGIL:
				{
					if (!targetItem.isArmor() || (targetItem.getItemType() != ArmorType.SIGIL))
					{
						player.sendPacket(SystemMessageId.THIS_ITEM_DOES_NOT_MEET_REQUIREMENTS);
						return false;
					}
				}
			}
		}

		return true;
	}

	public AppearanceHolder findVisualChange(Item targetItem)
	{
		foreach (AppearanceHolder holder in _allVisualIds)
		{
			if (targetItem.isArmor() && (holder.getBodyPart() != 0) &&
			    (targetItem.getTemplate().getBodyPart() != holder.getBodyPart()))
			{
				continue;
			}

			if (holder.getWeaponType() != WeaponType.NONE)
			{
				if (!targetItem.isWeapon() || (targetItem.getItemType() != holder.getWeaponType()))
				{
					if (holder.getWeaponType() != WeaponType.CROSSBOW)
					{
						continue;
					}
					else if ((targetItem.getItemType() != WeaponType.CROSSBOW) &&
					         (targetItem.getItemType() != WeaponType.TWOHANDCROSSBOW))
					{
						continue;
					}
				}

				switch (holder.getHandType())
				{
					case AppearanceHandType.ONE_HANDED:
					{
						if ((targetItem.getTemplate().getBodyPart() & ItemTemplate.SLOT_R_HAND) !=
						    ItemTemplate.SLOT_R_HAND)
						{
							continue;
						}

						break;
					}
					case AppearanceHandType.TWO_HANDED:
					{
						if ((targetItem.getTemplate().getBodyPart() & ItemTemplate.SLOT_LR_HAND) !=
						    ItemTemplate.SLOT_LR_HAND)
						{
							continue;
						}

						break;
					}
				}

				switch (holder.getMagicType())
				{
					case AppearanceMagicType.MAGICAL:
					{
						if (!targetItem.getTemplate().isMagicWeapon())
						{
							continue;
						}

						break;
					}
					case AppearanceMagicType.PHYISICAL:
					{
						if (targetItem.getTemplate().isMagicWeapon())
						{
							continue;
						}
					}
				}
			}

			if (holder.getArmorType() != ArmorType.NONE)
			{
				switch (holder.getArmorType())
				{
					case ArmorType.SHIELD:
					{
						if (!targetItem.isArmor() || (targetItem.getItemType() != ArmorType.SHIELD))
						{
							continue;
						}

						break;
					}
					case ArmorType.SIGIL:
					{
						if (!targetItem.isArmor() || (targetItem.getItemType() != ArmorType.SIGIL))
						{
							continue;
						}
					}
				}
			}

			return holder;
		}

		return null;
	}
}