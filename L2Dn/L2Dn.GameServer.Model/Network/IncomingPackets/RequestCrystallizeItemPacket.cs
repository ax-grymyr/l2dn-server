using L2Dn.GameServer.Data.Xml;
using L2Dn.GameServer.Enums;
using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Holders;
using L2Dn.GameServer.Model.ItemContainers;
using L2Dn.GameServer.Model.Items.Instances;
using L2Dn.GameServer.Model.Items.Types;
using L2Dn.GameServer.Model.Skills;
using L2Dn.GameServer.Network.Enums;
using L2Dn.GameServer.Network.OutgoingPackets;
using L2Dn.GameServer.StaticData;
using L2Dn.GameServer.Utilities;
using L2Dn.Model;
using L2Dn.Model.Enums;
using L2Dn.Network;
using L2Dn.Packets;
using L2Dn.Utilities;

namespace L2Dn.GameServer.Network.IncomingPackets;

public struct RequestCrystallizeItemPacket: IIncomingPacket<GameSession>
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
		{
			// PacketLogger.finer("RequestCrystalizeItem: activeChar was null.");
			return ValueTask.CompletedTask;
		}

		// if (!client.getFloodProtectors().canPerformTransaction())
		// {
		// player.sendMessage("You are crystallizing too fast.");
		// return;
		// }

		if (_count <= 0)
		{
			Util.handleIllegalPlayerAction(player,
				"[RequestCrystallizeItem] count <= 0! ban! oid: " + _objectId + " owner: " + player.getName(),
				Config.DEFAULT_PUNISH);

			return ValueTask.CompletedTask;
		}

		if (player.getPrivateStoreType() != PrivateStoreType.NONE || !player.isInCrystallize())
		{
			player.sendPacket(SystemMessageId.WHILE_OPERATING_A_PRIVATE_STORE_OR_WORKSHOP_YOU_CANNOT_DISCARD_DESTROY_OR_TRADE_AN_ITEM);
			return ValueTask.CompletedTask;
		}

		int skillLevel = player.getSkillLevel((int)CommonSkill.CRYSTALLIZE);
		if (skillLevel <= 0)
		{
			player.sendPacket(SystemMessageId.YOU_MAY_NOT_CRYSTALLIZE_THIS_ITEM_YOUR_CRYSTALLIZATION_SKILL_LEVEL_IS_TOO_LOW);
			player.sendPacket(ActionFailedPacket.STATIC_PACKET);
			if (player.getRace() != Race.DWARF && player.getClassId() != CharacterClass.FORTUNE_SEEKER &&
			    player.getClassId() != CharacterClass.BOUNTY_HUNTER)
			{
				PacketLogger.Instance.Info(player + " used crystalize with classid: " + player.getClassId());
			}

			return ValueTask.CompletedTask;
		}

		PlayerInventory inventory = player.getInventory();
		if (inventory != null)
		{
			Item? item = inventory.getItemByObjectId(_objectId);
			if (item == null || item.isHeroItem() || (!Config.ALT_ALLOW_AUGMENT_DESTROY && item.isAugmented()))
			{
				player.sendPacket(ActionFailedPacket.STATIC_PACKET);
				return ValueTask.CompletedTask;
			}

			if (_count > item.getCount())
			{
				_count = item.getCount();
			}
		}

		Item? itemToRemove = player.getInventory().getItemByObjectId(_objectId);
		if (itemToRemove == null || itemToRemove.isShadowItem() || itemToRemove.isTimeLimitedItem())
		{
			player.sendPacket(ActionFailedPacket.STATIC_PACKET);
			return ValueTask.CompletedTask;
		}

        if (!itemToRemove.getTemplate().isCrystallizable() || itemToRemove.getTemplate().getCrystalCount() <= 0 ||
            itemToRemove.getTemplate().getCrystalType() == CrystalType.NONE)
        {
            player.sendPacket(SystemMessageId.THIS_ITEM_CANNOT_BE_CRYSTALLIZED);
            return ValueTask.CompletedTask;
        }

        if (!player.getInventory().canManipulateWithItemId(itemToRemove.getId()))
		{
			player.sendPacket(SystemMessageId.THIS_ITEM_CANNOT_BE_CRYSTALLIZED);
			return ValueTask.CompletedTask;
		}

		// Check if the char can crystallize items and return if false;
		bool canCrystallize = true;

		switch (itemToRemove.getTemplate().getCrystalTypePlus())
		{
			case CrystalType.D:
			{
				if (skillLevel < 1)
				{
					canCrystallize = false;
				}
				break;
			}
			case CrystalType.C:
			{
				if (skillLevel < 2)
				{
					canCrystallize = false;
				}
				break;
			}
			case CrystalType.B:
			{
				if (skillLevel < 3)
				{
					canCrystallize = false;
				}
				break;
			}
			case CrystalType.A:
			{
				if (skillLevel < 4)
				{
					canCrystallize = false;
				}
				break;
			}
			case CrystalType.S:
			{
				if (skillLevel < 5)
				{
					canCrystallize = false;
				}
				break;
			}
			case CrystalType.R:
			{
				if (skillLevel < 6)
				{
					canCrystallize = false;
				}
				break;
			}
		}

		if (!canCrystallize)
		{
			player.sendPacket(SystemMessageId.YOU_MAY_NOT_CRYSTALLIZE_THIS_ITEM_YOUR_CRYSTALLIZATION_SKILL_LEVEL_IS_TOO_LOW);
			player.sendPacket(ActionFailedPacket.STATIC_PACKET);
			return ValueTask.CompletedTask;
		}

		List<ItemChanceHolder> crystallizationRewards = ItemCrystallizationData.getInstance().getCrystallizationRewards(itemToRemove);
		if (crystallizationRewards == null || crystallizationRewards.Count == 0)
		{
			player.sendPacket(SystemMessageId.ANGEL_NEVIT_S_DESCENT_BONUS_TIME_S1);
			return ValueTask.CompletedTask;
		}

		// player.setInCrystallize(true);

		// unequip if needed
		SystemMessagePacket sm;
		InventoryUpdatePacket iu;
		if (itemToRemove.isEquipped())
		{
			List<ItemInfo> itemInfos = new();
			foreach (Item item in player.getInventory().unEquipItemInSlotAndRecord(itemToRemove.getLocationSlot()))
			{
				itemInfos.Add(new ItemInfo(item, ItemChangeType.MODIFIED));
			}

			iu = new InventoryUpdatePacket(itemInfos);
			player.sendInventoryUpdate(iu);

			if (itemToRemove.getEnchantLevel() > 0)
			{
				sm = new SystemMessagePacket(SystemMessageId.S1_S2_UNEQUIPPED);
				sm.Params.addInt(itemToRemove.getEnchantLevel());
				sm.Params.addItemName(itemToRemove);
			}
			else
			{
				sm = new SystemMessagePacket(SystemMessageId.S1_UNEQUIPPED);
				sm.Params.addItemName(itemToRemove);
			}

			player.sendPacket(sm);
		}

		// remove from inventory
		Item? removedItem = player.getInventory().destroyItem("Crystalize", _objectId, _count, player, null);
        if (removedItem == null)
        {
            player.sendPacket(SystemMessageId.THIS_ITEM_CANNOT_BE_CRYSTALLIZED); // TODO: proper message
            return ValueTask.CompletedTask;
        }

		iu = new InventoryUpdatePacket(new ItemInfo(removedItem, ItemChangeType.REMOVED));
		player.sendInventoryUpdate(iu);

		foreach (ItemChanceHolder holder in crystallizationRewards)
		{
			double rand = Rnd.nextDouble() * 100;
			if (rand < holder.getChance())
			{
				// add crystals
				Item? createdItem = player.getInventory().addItem("Crystalize", holder.getId(), holder.getCount(), player, player);
                if (createdItem == null)
                {
                    player.sendPacket(SystemMessageId.YOUR_INVENTORY_IS_FULL); // TODO: proper message, atomic inventory update
                    return ValueTask.CompletedTask;
                }

                sm = new SystemMessagePacket(SystemMessageId.YOU_HAVE_OBTAINED_S1_X_S2);
				sm.Params.addItemName(createdItem);
				sm.Params.addLong(holder.getCount());
				player.sendPacket(sm);
			}
		}

		sm = new SystemMessagePacket(SystemMessageId.S1_HAS_BEEN_CRYSTALLIZED);
		sm.Params.addItemName(removedItem);
		player.sendPacket(sm);

		player.broadcastUserInfo();

		player.setInCrystallize(false);
		return ValueTask.CompletedTask;
    }
}