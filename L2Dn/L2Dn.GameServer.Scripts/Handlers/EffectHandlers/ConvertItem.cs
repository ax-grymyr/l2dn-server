using L2Dn.GameServer.Enums;
using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Effects;
using L2Dn.GameServer.Model.ItemContainers;
using L2Dn.GameServer.Model.Items;
using L2Dn.GameServer.Model.Items.Enchant.Attributes;
using L2Dn.GameServer.Model.Items.Instances;
using L2Dn.GameServer.Model.Skills;
using L2Dn.GameServer.Network.Enums;
using L2Dn.GameServer.Network.OutgoingPackets;
using L2Dn.GameServer.Utilities;

namespace L2Dn.GameServer.Scripts.Handlers.EffectHandlers;

/**
 * Convert Item effect implementation.
 * @author Zoey76
 */
public class ConvertItem: AbstractEffect
{
	public ConvertItem(StatSet @params)
	{
	}

	public override bool isInstant()
	{
		return true;
	}

	public override void instant(Creature effector, Creature effected, Skill skill, Item item)
	{
        Player? player = effected.getActingPlayer();
		if (effected.isAlikeDead() || !effected.isPlayer() || player == null)
		{
			return;
		}

		if (player.hasItemRequest())
		{
			return;
		}

		Weapon weaponItem = player.getActiveWeaponItem();
		if (weaponItem == null)
		{
			return;
		}

		Item? wpn = player.getInventory().getPaperdollItem(Inventory.PAPERDOLL_RHAND);
		if (wpn == null)
		{
			wpn = player.getInventory().getPaperdollItem(Inventory.PAPERDOLL_LHAND);
		}

		if (wpn == null || wpn.isAugmented() || weaponItem.getChangeWeaponId() == 0)
		{
			return;
		}

		int newItemId = weaponItem.getChangeWeaponId();
		if (newItemId == -1)
		{
			return;
		}

		int enchantLevel = wpn.getEnchantLevel();
		AttributeHolder? elementals = wpn.getAttributes() == null ? null : wpn.getAttackAttribute();
		List<Item> unequipped = player.getInventory().unEquipItemInBodySlotAndRecord(wpn.getTemplate().getBodyPart());
		InventoryUpdatePacket iu = new InventoryUpdatePacket(unequipped.Select(x => new ItemInfo(x, ItemChangeType.MODIFIED)).ToList());
		player.sendInventoryUpdate(iu);

		if (unequipped.Count == 0)
		{
			return;
		}

		byte count = 0;
		foreach (Item unequippedItem in unequipped)
		{
			if (!(unequippedItem.getTemplate() is Weapon))
			{
				count++;
				continue;
			}

			SystemMessagePacket sm;
			if (unequippedItem.getEnchantLevel() > 0)
			{
				sm = new SystemMessagePacket(SystemMessageId.S1_S2_UNEQUIPPED);
				sm.Params.addInt(unequippedItem.getEnchantLevel());
				sm.Params.addItemName(unequippedItem);
			}
			else
			{
				sm = new SystemMessagePacket(SystemMessageId.S1_UNEQUIPPED);
				sm.Params.addItemName(unequippedItem);
			}

			player.sendPacket(sm);
		}

		if (count == unequipped.Count)
		{
			return;
		}

		Item? destroyItem = player.getInventory().destroyItem("ChangeWeapon", wpn, player, null);
		if (destroyItem == null)
		{
			return;
		}

		Item newItem = player.getInventory().addItem("ChangeWeapon", newItemId, 1, player, destroyItem);
		if (newItem == null)
		{
			return;
		}

		if (elementals != null)
		{
			newItem.setAttribute(elementals, true);
		}
		newItem.setEnchantLevel(enchantLevel);
		player.getInventory().equipItem(newItem);

		SystemMessagePacket msg;
		if (newItem.getEnchantLevel() > 0)
		{
			msg = new SystemMessagePacket(SystemMessageId.S1_S2_EQUIPPED);
			msg.Params.addInt(newItem.getEnchantLevel());
			msg.Params.addItemName(newItem);
		}
		else
		{
			msg = new SystemMessagePacket(SystemMessageId.S1_EQUIPPED);
			msg.Params.addItemName(newItem);
		}
		player.sendPacket(msg);

		List<ItemInfo> items =
		[
			new ItemInfo(destroyItem, ItemChangeType.REMOVED),
			new ItemInfo(newItem, ItemChangeType.ADDED)
		];

		InventoryUpdatePacket u = new InventoryUpdatePacket(items);
		player.sendInventoryUpdate(u);

		player.broadcastUserInfo();
	}
}