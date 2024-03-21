using L2Dn.GameServer.Enums;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Actor.Request;
using L2Dn.GameServer.Model.Items;
using L2Dn.GameServer.Model.Items.Instances;
using L2Dn.GameServer.Model.Options;
using L2Dn.GameServer.Model.Skills;

namespace L2Dn.GameServer.Network.Enums;

public static class RefinePacketHelper
{
	/**
	 * Checks player, source item, lifestone and gemstone validity for augmentation process
	 * @param player
	 * @param item
	 * @param mineralItem
	 * @param feeItem
	 * @param fee
	 * @return
	 */
	public static bool isValid(Player player, Item item, Item mineralItem, Item feeItem, VariationFee fee)
	{
		if (fee == null)
		{
			return false;
		}
		
		if (!isValid(player, item, mineralItem))
		{
			return false;
		}
		
		if (feeItem != null)
		{
			// GemStones must belong to owner
			if (feeItem.getOwnerId() != player.getObjectId())
			{
				return false;
			}
			// .. and located in inventory
			if (feeItem.getItemLocation() != ItemLocation.INVENTORY)
			{
				return false;
			}
		}
		
		return true;
	}
	
	/**
	 * Checks player, source item and lifestone validity for augmentation process
	 * @param player
	 * @param item
	 * @param mineralItem
	 * @return
	 */
	public static bool isValid(Player player, Item item, Item mineralItem)
	{
		if (!isValid(player, item))
		{
			return false;
		}
		
		// Item must belong to owner
		if (mineralItem.getOwnerId() != player.getObjectId())
		{
			return false;
		}
		
		// Lifestone must be located in inventory
		if (mineralItem.getItemLocation() != ItemLocation.INVENTORY)
		{
			return false;
		}
		
		return true;
	}
	
	/**
	 * Check both player and source item conditions for augmentation process
	 * @param player
	 * @param item
	 * @return
	 */
	public static bool isValid(Player player, Item item)
	{
		if (!isValid(player))
		{
			return false;
		}
		
		// Item must belong to owner
		if (item.getOwnerId() != player.getObjectId())
		{
			return false;
		}
		
		if (item.isHeroItem())
		{
			return false;
		}
		if (item.isShadowItem())
		{
			return false;
		}
		if (item.isCommonItem())
		{
			return false;
		}
		if (item.isEtcItem())
		{
			return false;
		}
		if (item.isTimeLimitedItem())
		{
			return false;
		}
		if (item.isPvp() && !Config.ALT_ALLOW_AUGMENT_PVP_ITEMS)
		{
			return false;
		}
		
		// Source item can be equipped or in inventory
		switch (item.getItemLocation())
		{
			case ItemLocation.INVENTORY:
			case ItemLocation.PAPERDOLL:
			{
				break;
			}
			default:
			{
				return false;
			}
		}
		
		if (!(item.getTemplate() is Weapon) && !(item.getTemplate() is Armor))
		{
			return false; // neither weapon nor armor ?
		}
		
		// blacklist check
		if (Config.AUGMENTATION_BLACKLIST.Contains(item.getId()))
		{
			return false;
		}
		
		return true;
	}
	
	/**
	 * Check if player's conditions valid for augmentation process
	 * @param player
	 * @return
	 */
	public static bool isValid(Player player)
	{
		if (player.getPrivateStoreType() != PrivateStoreType.NONE)
		{
			player.sendPacket(SystemMessageId.YOU_CANNOT_AUGMENT_ITEMS_WHILE_A_PRIVATE_STORE_OR_PRIVATE_WORKSHOP_IS_IN_OPERATION);
			return false;
		}
		if (player.getActiveTradeList() != null)
		{
			player.sendPacket(SystemMessageId.YOU_CANNOT_AUGMENT_ITEMS_WHILE_ENGAGED_IN_TRADE_ACTIVITIES);
			return false;
		}
		if (player.isDead())
		{
			player.sendPacket(SystemMessageId.YOU_CANNOT_AUGMENT_ITEMS_WHILE_DEAD);
			return false;
		}
		if (player.hasBlockActions() && player.hasAbnormalType(AbnormalType.PARALYZE))
		{
			player.sendPacket(SystemMessageId.YOU_CANNOT_AUGMENT_ITEMS_WHILE_PARALYZED);
			return false;
		}
		if (player.isFishing())
		{
			player.sendPacket(SystemMessageId.YOU_CANNOT_AUGMENT_ITEMS_WHILE_FISHING);
			return false;
		}
		if (player.isSitting())
		{
			player.sendPacket(SystemMessageId.YOU_CANNOT_AUGMENT_ITEMS_WHILE_SITTING_DOWN);
			return false;
		}
		if (player.isCursedWeaponEquipped())
		{
			return false;
		}

		if (player.hasRequest<EnchantItemRequest>() || player.hasRequest<EnchantItemAttributeRequest>() ||
		    player.isProcessingTransaction())
		{
			return false;
		}

		return true;
	}
}