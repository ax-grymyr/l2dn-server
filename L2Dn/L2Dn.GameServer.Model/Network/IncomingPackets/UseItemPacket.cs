using L2Dn.Events;
using L2Dn.GameServer.AI;
using L2Dn.GameServer.Data.Xml;
using L2Dn.GameServer.Enums;
using L2Dn.GameServer.Handlers;
using L2Dn.GameServer.InstanceManagers;
using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Actor.Request;
using L2Dn.GameServer.Model.Effects;
using L2Dn.GameServer.Model.Events.Impl.Items;
using L2Dn.GameServer.Model.Holders;
using L2Dn.GameServer.Model.Items;
using L2Dn.GameServer.Model.Items.Instances;
using L2Dn.GameServer.Model.Items.Types;
using L2Dn.GameServer.Model.Zones;
using L2Dn.GameServer.Network.Enums;
using L2Dn.GameServer.Network.OutgoingPackets;
using L2Dn.GameServer.Network.OutgoingPackets.Ensoul;
using L2Dn.GameServer.Network.OutgoingPackets.Variations;
using L2Dn.GameServer.Utilities;
using L2Dn.Model.Enums;
using L2Dn.Network;
using L2Dn.Packets;
using Config = L2Dn.GameServer.Configuration.Config;
using ThreadPool = L2Dn.GameServer.Utilities.ThreadPool;

namespace L2Dn.GameServer.Network.IncomingPackets;

public struct UseItemPacket: IIncomingPacket<GameSession>
{
    private int _objectId;
    private bool _ctrlPressed;

    public void ReadContent(PacketBitReader reader)
    {
        _objectId = reader.ReadInt32();
        _ctrlPressed = reader.ReadInt32() != 0;
    }

    public ValueTask ProcessAsync(Connection connection, GameSession session)
    {
		Player? player = session.Player;
		if (player == null)
			return ValueTask.CompletedTask;

		// TODO: Flood protect UseItem
		// if (!client.getFloodProtectors().canUseItem())
		// {
		// 	return ValueTask.CompletedTask;
		// }

		if (player.isInsideZone(ZoneId.JAIL))
		{
			player.sendMessage("You cannot use items while jailed.");
			return ValueTask.CompletedTask;
		}

		if (player.getActiveTradeList() != null)
		{
			player.cancelActiveTrade();
		}

		if (player.getPrivateStoreType() != PrivateStoreType.NONE)
		{
			player.sendPacket(SystemMessageId.WHILE_OPERATING_A_PRIVATE_STORE_OR_WORKSHOP_YOU_CANNOT_DISCARD_DESTROY_OR_TRADE_AN_ITEM);
			player.sendPacket(ActionFailedPacket.STATIC_PACKET);
			return ValueTask.CompletedTask;
		}

		Item? item = player.getInventory().getItemByObjectId(_objectId);
		if (item == null)
		{
			// GM can use other player item
			if (player.isGM())
			{
				WorldObject? obj = World.getInstance().findObject(_objectId);
				if (obj != null && obj.isItem())
				{
					AdminCommandHandler.getInstance().useAdminCommand(player, "admin_use_item " + _objectId, true);
				}
			}

			return ValueTask.CompletedTask;
		}

		if (item.isQuestItem() && item.getTemplate().getDefaultAction() != ActionType.NONE)
		{
			player.sendPacket(SystemMessageId.YOU_CANNOT_USE_QUEST_ITEMS);
			return ValueTask.CompletedTask;
		}

		// No UseItem is allowed while the player is in special conditions
		if (player.hasBlockActions() || player.isControlBlocked() || player.isAlikeDead())
		{
			return ValueTask.CompletedTask;
		}

		// Char cannot use item when dead
		if (player.isDead() || !player.getInventory().canManipulateWithItemId(item.getId()))
		{
			SystemMessagePacket sm = new SystemMessagePacket(SystemMessageId.S1_CANNOT_BE_USED_THE_REQUIREMENTS_ARE_NOT_MET);
			sm.Params.addItemName(item);
			player.sendPacket(sm);
			return ValueTask.CompletedTask;
		}

		if (!item.isEquipped() && !item.getTemplate().checkCondition(player, player, true))
		{
			return ValueTask.CompletedTask;
		}

		int itemId = item.getId();
		if (player.isFishing() && (itemId < 6535 || itemId > 6540))
		{
			// You cannot do anything else while fishing
			player.sendPacket(SystemMessageId.YOU_CANNOT_DO_THAT_WHILE_FISHING_3);
			return ValueTask.CompletedTask;
		}

		if (!Config.Character.ALT_GAME_KARMA_PLAYER_CAN_TELEPORT && player.getReputation() < 0)
		{
			List<ItemSkillHolder>? skills = item.getTemplate().getSkills(ItemSkillType.NORMAL);
			if (skills != null)
			{
				foreach (ItemSkillHolder holder in skills)
				{
					if (holder.getSkill().hasEffectType(EffectType.TELEPORT))
					{
						return ValueTask.CompletedTask;
					}
				}
			}
		}

		// If the item has reuse time and it has not passed.
		// Message from reuse delay must come from item.
		TimeSpan reuseDelay = item.getReuseDelay();
		int sharedReuseGroup = item.getSharedReuseGroup();
		if (reuseDelay > TimeSpan.Zero)
		{
			TimeSpan reuse = player.getItemRemainingReuseTime(item.ObjectId);
			if (reuse > TimeSpan.Zero)
			{
				reuseData(player, item, reuse);
				sendSharedGroupUpdate(player, itemId, sharedReuseGroup, reuse, reuseDelay);
				return ValueTask.CompletedTask;
			}

			TimeSpan? reuseOnGroup = player.getReuseDelayOnGroup(sharedReuseGroup);
			if (reuseOnGroup > TimeSpan.Zero)
			{
				reuseData(player, item, reuseOnGroup.Value);
				sendSharedGroupUpdate(player, itemId, sharedReuseGroup, reuseOnGroup.Value, reuseDelay);
				return ValueTask.CompletedTask;
			}
		}

		player.onActionRequest();

		if (item.isEquipable())
		{
			// Don't allow to put formal wear while a cursed weapon is equipped.
			if (player.isCursedWeaponEquipped() && itemId == 6408)
			{
				return ValueTask.CompletedTask;
			}

			// Equip or unEquip
			if (FortSiegeManager.getInstance().isCombat(itemId))
			{
				return ValueTask.CompletedTask; // no message
			}

			if (player.isCombatFlagEquipped())
			{
				return ValueTask.CompletedTask;
			}

			if (player.getInventory().isItemSlotBlocked(item.getTemplate().getBodyPart()))
			{
				player.sendPacket(SystemMessageId.YOU_DO_NOT_MEET_THE_REQUIRED_CONDITION_TO_EQUIP_THAT_ITEM);
				return ValueTask.CompletedTask;
			}

			if (item.isArmor())
			{
				// Prevent equip shields for Death Knight, Sylph, Vanguard or Assassin players.
				if (item.getItemType() == ArmorType.SHIELD && (player.isDeathKnight() || player.getRace() == Race.SYLPH || player.isVanguard() || player.isAssassin()))
				{
					player.sendPacket(SystemMessageId.YOU_DO_NOT_MEET_THE_REQUIRED_CONDITION_TO_EQUIP_THAT_ITEM);
					return ValueTask.CompletedTask;
				}
			}
			else if (item.isWeapon() && item.getItemType() != WeaponType.FISHINGROD) // Fishing rods are enabled for all players.
			{
				// Prevent equip pistols for non Sylph players.
				if (item.getItemType() == WeaponType.PISTOLS)
				{
					if (player.getRace() != Race.SYLPH)
					{
						player.sendPacket(SystemMessageId.YOU_DO_NOT_MEET_THE_REQUIRED_CONDITION_TO_EQUIP_THAT_ITEM);
						return ValueTask.CompletedTask;
					}
				}
				else
				{
					// Prevent Dwarf players equip rapiers.
					if (player.getRace() == Race.DWARF)
					{
						if (item.getItemType() == WeaponType.RAPIER)
						{
							player.sendPacket(SystemMessageId.YOU_DO_NOT_MEET_THE_REQUIRED_CONDITION_TO_EQUIP_THAT_ITEM);
							return ValueTask.CompletedTask;
						}
					}
					// Prevent Orc players equip rapiers.
					else if (player.getRace() == Race.ORC)
					{
						if (item.getItemType() == WeaponType.RAPIER)
						{
							player.sendPacket(SystemMessageId.YOU_DO_NOT_MEET_THE_REQUIRED_CONDITION_TO_EQUIP_THAT_ITEM);
							return ValueTask.CompletedTask;
						}
					}
					// Prevent Sylph players to equip other weapons than Pistols.
					else if (player.getRace() == Race.SYLPH)
					{
						if (item.getItemType() != WeaponType.PISTOLS)
						{
							player.sendPacket(SystemMessageId.YOU_DO_NOT_MEET_THE_REQUIRED_CONDITION_TO_EQUIP_THAT_ITEM);
							return ValueTask.CompletedTask;
						}
					}
					// Prevent Vanguard Rider players to equip other weapons than Pole.
					else if (player.isVanguard())
					{
						if (item.getItemType() != WeaponType.POLE)
						{
							player.sendPacket(SystemMessageId.YOU_DO_NOT_MEET_THE_REQUIRED_CONDITION_TO_EQUIP_THAT_ITEM);
							return ValueTask.CompletedTask;
						}
					}
					// Prevent Assassins players to equip other weapons than Dagger.
					else if (player.isAssassin())
					{
						if (item.getItemType() != WeaponType.DAGGER)
						{
							player.sendPacket(SystemMessageId.YOU_DO_NOT_MEET_THE_REQUIRED_CONDITION_TO_EQUIP_THAT_ITEM);
							return ValueTask.CompletedTask;
						}
					}
					// Prevent other races using Ancient swords.
					else if (player.getRace() != Race.KAMAEL)
					{
						if (item.getItemType() == WeaponType.ANCIENTSWORD)
						{
							player.sendPacket(SystemMessageId.YOU_DO_NOT_MEET_THE_REQUIRED_CONDITION_TO_EQUIP_THAT_ITEM);
							return ValueTask.CompletedTask;
						}
					}
				}
			}

			// Prevent players to equip weapon while wearing combat flag
			// Don't allow weapon/shield equipment if a cursed weapon is equipped.
			if (item.getTemplate().getBodyPart() == ItemTemplate.SLOT_LR_HAND || item.getTemplate().getBodyPart() == ItemTemplate.SLOT_L_HAND || item.getTemplate().getBodyPart() == ItemTemplate.SLOT_R_HAND)
			{
				if (player.getActiveWeaponItem() != null && player.getActiveWeaponItem().getId() == FortManager.ORC_FORTRESS_FLAG)
				{
					player.sendPacket(SystemMessageId.YOU_DO_NOT_MEET_THE_REQUIRED_CONDITION_TO_EQUIP_THAT_ITEM);
					return ValueTask.CompletedTask;
				}
				if (player.isMounted() || player.isDisarmed())
				{
					player.sendPacket(SystemMessageId.YOU_DO_NOT_MEET_THE_REQUIRED_CONDITION_TO_EQUIP_THAT_ITEM);
					return ValueTask.CompletedTask;
				}
				if (player.isCursedWeaponEquipped())
				{
					return ValueTask.CompletedTask;
				}
			}
			else if (item.getTemplate().getBodyPart() == ItemTemplate.SLOT_DECO)
			{
				if (!item.isEquipped() && player.getInventory().getTalismanSlots() == 0)
				{
					player.sendPacket(SystemMessageId.NO_EQUIPMENT_SLOT_AVAILABLE);
					return ValueTask.CompletedTask;
				}
			}
			else if (item.getTemplate().getBodyPart() == ItemTemplate.SLOT_BROOCH_JEWEL)
			{
				if (!item.isEquipped() && player.getInventory().getBroochJewelSlots() == 0)
				{
					SystemMessagePacket sm = new SystemMessagePacket(SystemMessageId.YOU_CANNOT_EQUIP_S1_WITHOUT_EQUIPPING_A_BROOCH);
					sm.Params.addItemName(item);
					player.sendPacket(sm);
					return ValueTask.CompletedTask;
				}
			}
			else if (item.getTemplate().getBodyPart() == ItemTemplate.SLOT_AGATHION)
			{
				if (!item.isEquipped() && player.getInventory().getAgathionSlots() == 0)
				{
					player.sendPacket(SystemMessageId.NO_EQUIPMENT_SLOT_AVAILABLE);
					return ValueTask.CompletedTask;
				}
			}
			else if (item.getTemplate().getBodyPart() == ItemTemplate.SLOT_ARTIFACT)
			{
				if (!item.isEquipped() && player.getInventory().getArtifactSlots() == 0)
				{
					SystemMessagePacket sm = new SystemMessagePacket(SystemMessageId.YOU_DO_NOT_MEET_THE_REQUIRED_CONDITION_TO_EQUIP_THAT_ITEM);
					sm.Params.addItemName(item);
					player.sendPacket(sm);
					return ValueTask.CompletedTask;
				}
			}

			// Over-enchant protection.
			if (Config.Character.OVER_ENCHANT_PROTECTION && !player.isGM() //
				&& ((item.isWeapon() && item.getEnchantLevel() > EnchantItemGroupsData.getInstance().getMaxWeaponEnchant()) //
					|| (item.getTemplate().getType2() == ItemTemplate.TYPE2_ACCESSORY && item.getEnchantLevel() > EnchantItemGroupsData.getInstance().getMaxAccessoryEnchant()) //
					|| (item.isArmor() && item.getTemplate().getType2() != ItemTemplate.TYPE2_ACCESSORY && item.getEnchantLevel() > EnchantItemGroupsData.getInstance().getMaxArmorEnchant())))
			{
				PacketLogger.Instance.Info("Over-enchanted (+" + item.getEnchantLevel() + ") " + item + " has been removed from " + player);
				player.getInventory().destroyItem("Over-enchant protection", item, player, null);
				if (Config.Character.OVER_ENCHANT_PUNISHMENT != IllegalActionPunishmentType.NONE)
				{
					player.sendMessage("[Server]: You have over-enchanted items!");
					player.sendMessage("[Server]: Respect our server rules.");
					player.sendPacket(new ExShowScreenMessagePacket("You have over-enchanted items!", 6000));
					Util.handleIllegalPlayerAction(player, player.getName() + " has over-enchanted items.", Config.Character.OVER_ENCHANT_PUNISHMENT);
				}

				return ValueTask.CompletedTask;
			}

			if (player.isCastingNow())
			{
				// Create and Bind the next action to the AI.
				player.getAI().setNextAction(new NextAction(CtrlEvent.EVT_FINISH_CASTING,
					CtrlIntention.AI_INTENTION_CAST, () => player.useEquippableItem(item, true)));
			}
			else // Equip or unEquip.
			{
				DateTime currentTime = DateTime.UtcNow;
				DateTime attackEndTime = player.getAttackEndTime();
				if (attackEndTime > currentTime)
				{
					ThreadPool.schedule(() => player.useEquippableItem(item, false),
						attackEndTime - currentTime);
				}
				else
				{
					player.useEquippableItem(item, true);
				}
			}
		}
		else
		{
			EtcItem? etcItem = item.getEtcItem();
			if (etcItem != null && etcItem.getExtractableItems() != null && player.hasRequest<AutoPeelRequest>())
			{
				return ValueTask.CompletedTask;
			}

			IItemHandler? handler = ItemHandler.getInstance().getHandler(etcItem);
			if (handler == null)
			{
				if (etcItem != null && etcItem.getHandlerName() != null)
				{
					PacketLogger.Instance.Warn("Unmanaged Item handler: " + etcItem.getHandlerName() + " for Item Id: " + itemId + "!");
				}
			}
			else if (handler.useItem(player, item, _ctrlPressed))
			{
				// Item reuse time should be added if the item is successfully used.
				// Skill reuse delay is done at handlers.itemhandlers.ItemSkillsTemplate;
				if (reuseDelay > TimeSpan.Zero)
				{
					player.addTimeStampItem(item, reuseDelay);
					sendSharedGroupUpdate(player, itemId, sharedReuseGroup, reuseDelay, reuseDelay);
				}

				// Notify events.
				EventContainer itemEvents = item.getTemplate().Events;
				if (itemEvents.HasSubscribers<OnItemUse>())
				{
					itemEvents.NotifyAsync(new OnItemUse(player, item));
				}
			}

			if (etcItem != null)
			{
				if (etcItem.isMineral())
				{
					player.sendPacket(ExShowVariationMakeWindowPacket.STATIC_PACKET);
					player.sendPacket(new ExPutIntensiveResultForVariationMakePacket(item.ObjectId));
				}
				else if (etcItem.isEnsoulStone())
				{
					player.sendPacket(ExShowEnsoulWindowPacket.STATIC_PACKET);
				}
			}
		}

		return ValueTask.CompletedTask;
	}

	private static void reuseData(Player player, Item item, TimeSpan remainingTime)
	{
		SystemMessagePacket sm;
		if (remainingTime >= TimeSpan.FromHours(1))
		{
			sm = new SystemMessagePacket(SystemMessageId.S1_WILL_BE_AVAILABLE_AGAIN_IN_S2_H_S3_MIN_S4_SEC);
			sm.Params.addItemName(item);
			sm.Params.addInt((int)remainingTime.TotalHours);
			sm.Params.addInt(remainingTime.Minutes);
		}
		else if (remainingTime >= TimeSpan.FromMinutes(1))
		{
			sm = new SystemMessagePacket(SystemMessageId.S1_WILL_BE_AVAILABLE_AGAIN_IN_S2_MIN_S3_SEC);
			sm.Params.addItemName(item);
			sm.Params.addInt(remainingTime.Minutes);
		}
		else
		{
			sm = new SystemMessagePacket(SystemMessageId.S1_WILL_BE_AVAILABLE_AGAIN_IN_S2_SEC);
			sm.Params.addItemName(item);
		}

		sm.Params.addInt(remainingTime.Seconds);
		player.sendPacket(sm);
	}

	private static void sendSharedGroupUpdate(Player player, int itemId, int group, TimeSpan remaining, TimeSpan reuse)
	{
		if (group > 0)
		{
			player.sendPacket(new ExUseSharedGroupItemPacket(itemId, group, remaining, reuse));
		}
	}
}