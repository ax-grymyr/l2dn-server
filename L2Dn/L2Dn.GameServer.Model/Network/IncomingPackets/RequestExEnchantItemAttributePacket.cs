using L2Dn.GameServer.Data.Xml;
using L2Dn.GameServer.Enums;
using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Actor.Request;
using L2Dn.GameServer.Model.Holders;
using L2Dn.GameServer.Model.Items.Enchant.Attributes;
using L2Dn.GameServer.Model.Items.Instances;
using L2Dn.GameServer.Network.Enums;
using L2Dn.GameServer.Network.OutgoingPackets;
using L2Dn.GameServer.Utilities;
using L2Dn.Model.Enums;
using L2Dn.Network;
using L2Dn.Packets;

namespace L2Dn.GameServer.Network.IncomingPackets;

public struct RequestExEnchantItemAttributePacket: IIncomingPacket<GameSession>
{
    private int _objectId;
    private long _count;

    public void ReadContent(PacketBitReader reader)
    {
        _objectId = reader.ReadInt32();
        _count = reader.ReadInt64();
    }

    public ValueTask ProcessAsync(Connection connection, GameSession session)
    {
        Player? player = session.Player;
        if (player == null)
            return ValueTask.CompletedTask;

		EnchantItemAttributeRequest? request = player.getRequest<EnchantItemAttributeRequest>();
		if (request == null)
			return ValueTask.CompletedTask;

		request.setProcessing(true);

		if (_objectId == -1)
		{
			// Player canceled enchant
			player.removeRequest<EnchantItemAttributeRequest>();
			player.sendPacket(SystemMessageId.ATTRIBUTE_ITEM_USAGE_HAS_BEEN_CANCELLED);
			return ValueTask.CompletedTask;
		}

		if (!player.isOnline())
		{
			player.removeRequest<EnchantItemAttributeRequest>();
			return ValueTask.CompletedTask;
		}

		if (player.getPrivateStoreType() != PrivateStoreType.NONE)
		{
			player.sendPacket(SystemMessageId.YOU_CANNOT_ADD_ELEMENTAL_POWER_WHILE_OPERATING_A_PRIVATE_STORE_OR_PRIVATE_WORKSHOP);
			player.removeRequest<EnchantItemAttributeRequest>();
			return ValueTask.CompletedTask;
		}

		// Restrict enchant during a trade (bug if enchant fails)
		if (player.getActiveRequester() != null)
		{
			// Cancel trade
			player.cancelActiveTrade();
			player.removeRequest<EnchantItemAttributeRequest>();
			player.sendPacket(SystemMessageId.YOU_CANNOT_DO_THAT_WHILE_TRADING);
			return ValueTask.CompletedTask;
		}

		Item? item = player.getInventory().getItemByObjectId(_objectId);
		Item stone = request.getEnchantingStone();
		if (item == null || stone == null)
		{
			player.removeRequest<EnchantItemAttributeRequest>();
			player.sendPacket(SystemMessageId.ATTRIBUTE_ITEM_USAGE_HAS_BEEN_CANCELLED);
			return ValueTask.CompletedTask;
		}

		if (!item.isElementable())
		{
			player.sendPacket(SystemMessageId.ELEMENTAL_POWER_ENHANCER_USAGE_REQUIREMENT_IS_NOT_SUFFICIENT);
			player.removeRequest<EnchantItemAttributeRequest>();
			return ValueTask.CompletedTask;
		}

		switch (item.getItemLocation())
		{
			case ItemLocation.INVENTORY:
			case ItemLocation.PAPERDOLL:
			{
				if (item.getOwnerId() != player.ObjectId)
				{
					player.removeRequest<EnchantItemAttributeRequest>();
					return ValueTask.CompletedTask;
				}

				break;
			}

			default:
			{
				player.removeRequest<EnchantItemAttributeRequest>();
				Util.handleIllegalPlayerAction(player, player + " tried to use enchant Exploit!", Config.DEFAULT_PUNISH);
				return ValueTask.CompletedTask;
			}
		}

		int stoneId = stone.getId();
		long count = Math.Min(stone.getCount(), _count);
		AttributeType elementToAdd = ElementalAttributeData.getInstance().getItemElement(stoneId);
		// Armors have the opposite element
		if (item.isArmor())
		{
			elementToAdd = elementToAdd.getOpposite();
		}
		AttributeType opositeElement = elementToAdd.getOpposite();
		AttributeHolder? oldElement = item.getAttribute(elementToAdd);
		int elementValue = oldElement == null ? 0 : oldElement.getValue();
		int limit = getLimit(item, stoneId);
		int powerToAdd = getPowerToAdd(stoneId, elementValue, item);
		if ((item.isWeapon() && oldElement != null && oldElement.getType() != elementToAdd &&
		     oldElement.getType() != AttributeType.NONE) || (item.isArmor() && item.getAttribute(elementToAdd) == null && item.getAttributes() != null && item.getAttributes().Count >= 3))
		{
			player.sendPacket(SystemMessageId.ANOTHER_ELEMENTAL_POWER_HAS_ALREADY_BEEN_ADDED_THIS_ELEMENTAL_POWER_CANNOT_BE_ADDED);
			player.removeRequest<EnchantItemAttributeRequest>();
			return ValueTask.CompletedTask;
		}

		if (item.isArmor() && item.getAttributes() != null)
		{
			// can't add opposite element
			foreach (AttributeHolder attribute in item.getAttributes())
			{
				if (attribute.getType() == opositeElement)
				{
					player.removeRequest<EnchantItemAttributeRequest>();
					Util.handleIllegalPlayerAction(player, player + " tried to add oposite attribute to item!", Config.DEFAULT_PUNISH);
					return ValueTask.CompletedTask;
				}
			}
		}

		int newPower = elementValue + powerToAdd;
		if (newPower > limit)
		{
			newPower = limit;
			powerToAdd = limit - elementValue;
		}

		if (powerToAdd <= 0)
		{
			player.sendPacket(SystemMessageId.ATTRIBUTE_ITEM_USAGE_HAS_BEEN_CANCELLED);
			player.removeRequest<EnchantItemAttributeRequest>();
			return ValueTask.CompletedTask;
		}

		int usedStones = 0;
		int successfulAttempts = 0;
		int failedAttempts = 0;
		int result;
		for (int i = 0; i < count; i++)
		{
			usedStones++;
			result = addElement(player, stone, item, elementToAdd);
			if (result == 1)
			{
				successfulAttempts++;
			}
			else if (result == 0)
			{
				failedAttempts++;
			}
			else
			{
				break;
			}
		}

		item.updateItemElementals();
		player.destroyItem("AttrEnchant", stone, usedStones, player, true);
		AttributeHolder? newElement = item.getAttribute(elementToAdd);
		int newValue = newElement != null ? newElement.getValue() : 0;
		AttributeType realElement = item.isArmor() ? opositeElement : elementToAdd;
		List<ItemInfo> itemsToUpdate = new List<ItemInfo>();
		if (successfulAttempts > 0)
		{
			SystemMessagePacket sm;
			if (item.getEnchantLevel() == 0)
			{
				if (item.isArmor())
				{
					sm = new SystemMessagePacket(SystemMessageId.THE_S2_S_ATTRIBUTE_WAS_SUCCESSFULLY_BESTOWED_ON_S1_AND_RESISTANCE_TO_S3_WAS_INCREASED);
				}
				else
				{
					sm = new SystemMessagePacket(SystemMessageId.S2_ATTRIBUTE_HAS_BEEN_ADDED_TO_S1);
				}

				sm.Params.addItemName(item);
				sm.Params.addAttribute(realElement);
				if (item.isArmor())
				{
					sm.Params.addAttribute(realElement.getOpposite());
				}
			}
			else
			{
				if (item.isArmor())
				{
					sm = new SystemMessagePacket(SystemMessageId.S3_POWER_HAS_BEEN_ADDED_TO_S1_S2_S4_RESISTANCE_IS_INCREASED);
				}
				else
				{
					sm = new SystemMessagePacket(SystemMessageId.S3_POWER_HAS_BEEN_ADDED_TO_S1_S2);
				}

				sm.Params.addInt(item.getEnchantLevel());
				sm.Params.addItemName(item);
				sm.Params.addAttribute(realElement);
				if (item.isArmor())
				{
					sm.Params.addAttribute(realElement.getOpposite());
				}
			}

			player.sendPacket(sm);

			// send packets
			itemsToUpdate.Add(new ItemInfo(item, ItemChangeType.MODIFIED));
		}
		else
		{
			player.sendPacket(SystemMessageId.YOU_HAVE_FAILED_TO_ADD_ELEMENTAL_POWER);
		}

		result = 0;
		if (successfulAttempts == 0)
		{
			// Failed
			result = 2;
		}

		// Stone must be removed
		if (stone.getCount() == 0)
		{
			itemsToUpdate.Add(new ItemInfo(stone, ItemChangeType.REMOVED));
		}
		else
		{
			itemsToUpdate.Add(new ItemInfo(stone, ItemChangeType.MODIFIED));
		}

		player.removeRequest<EnchantItemAttributeRequest>();
		player.sendPacket(new ExAttributeEnchantResultPacket(result, item.isWeapon(), elementToAdd, elementValue,
			newValue, successfulAttempts, failedAttempts));

		player.updateUserInfo();

		InventoryUpdatePacket iu = new InventoryUpdatePacket(itemsToUpdate);
		player.sendInventoryUpdate(iu);

		return ValueTask.CompletedTask;
	}

	private static int addElement(Player player, Item stone, Item item, AttributeType elementToAdd)
	{
		AttributeHolder? oldElement = item.getAttribute(elementToAdd);
		int elementValue = oldElement == null ? 0 : oldElement.getValue();
		int limit = getLimit(item, stone.getId());
		int powerToAdd = getPowerToAdd(stone.getId(), elementValue, item);
		int newPower = elementValue + powerToAdd;
		if (newPower > limit)
		{
			newPower = limit;
			powerToAdd = limit - elementValue;
		}

		if (powerToAdd <= 0)
		{
			player.sendPacket(SystemMessageId.ATTRIBUTE_ITEM_USAGE_HAS_BEEN_CANCELLED);
			player.removeRequest<EnchantItemAttributeRequest>();
			return -1;
		}

		bool success = ElementalAttributeData.getInstance().isSuccess(item, stone.getId());
		if (success)
		{
			item.setAttribute(new AttributeHolder(elementToAdd, newPower), false);
		}

		return success ? 1 : 0;
	}

	public static int getLimit(Item item, int sotneId)
	{
		ElementalItemHolder elementItem = ElementalAttributeData.getInstance().getItemElemental(sotneId);
		if (elementItem == null)
		{
			return 0;
		}

		if (item.isWeapon())
		{
			return ElementalAttributeData.WeaponValues[elementItem.getType().GetMaxLevel()];
		}

		return ElementalAttributeData.ArmorValues[elementItem.getType().GetMaxLevel()];
	}

	public static int getPowerToAdd(int stoneId, int oldValue, Item item)
    {
        ElementalItemHolder? elemental = ElementalAttributeData.getInstance().getItemElemental(stoneId);
		if (ElementalAttributeData.getInstance().getItemElement(stoneId) != AttributeType.NONE && elemental != null)
		{
			if (elemental.getPower() > 0)
			{
				return elemental.getPower();
			}
			if (item.isWeapon())
			{
				if (oldValue == 0)
				{
					return ElementalAttributeData.FirstWeaponBonus;
				}

				return ElementalAttributeData.NextWeaponBonus;
			}

			if (item.isArmor())
			{
				return ElementalAttributeData.ArmorBonus;
			}
		}
		return 0;
	}
}