using L2Dn.GameServer.Data.Xml;
using L2Dn.GameServer.Enums;
using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Ensoul;
using L2Dn.GameServer.Model.Holders;
using L2Dn.GameServer.Model.Items.Enchant.Attributes;
using L2Dn.GameServer.Model.Items.Instances;
using L2Dn.GameServer.Model.Variables;
using L2Dn.GameServer.Network.OutgoingPackets;
using L2Dn.GameServer.Network.OutgoingPackets.EquipmentUpgrade;
using L2Dn.GameServer.Utilities;
using L2Dn.Model.Enums;
using L2Dn.Network;
using L2Dn.Packets;

namespace L2Dn.GameServer.Network.IncomingPackets;

public struct RequestUpgradeSystemResultPacket: IIncomingPacket<GameSession>
{
    private int _objectId;
    private int _upgradeId;

    public void ReadContent(PacketBitReader reader)
    {
        _objectId = reader.ReadInt32();
        _upgradeId = reader.ReadInt32();
    }

    public ValueTask ProcessAsync(Connection connection, GameSession session)
    {
        Player? player = session.Player;
        if (player == null)
            return ValueTask.CompletedTask;

		Item existingItem = player.getInventory().getItemByObjectId(_objectId);
		if (existingItem == null)
		{
			player.sendPacket(new ExUpgradeSystemResultPacket(0, 0));
			return ValueTask.CompletedTask;
		}
		
		EquipmentUpgradeHolder upgradeHolder = EquipmentUpgradeData.getInstance().getUpgrade(_upgradeId);
		if (upgradeHolder == null)
		{
			player.sendPacket(new ExUpgradeSystemResultPacket(0, 0));
			return ValueTask.CompletedTask;
		}
		
		foreach (ItemHolder material in upgradeHolder.getMaterials())
		{
			if (player.getInventory().getInventoryItemCount(material.getId(), -1) < material.getCount())
			{
				player.sendPacket(new ExUpgradeSystemResultPacket(0, 0));
				return ValueTask.CompletedTask;
			}
		}
		
		long adena = upgradeHolder.getAdena();
		if ((adena > 0) && (player.getAdena() < adena))
		{
			player.sendPacket(new ExUpgradeSystemResultPacket(0, 0));
			return ValueTask.CompletedTask;
		}
		
		if ((existingItem.getTemplate().getId() != upgradeHolder.getRequiredItemId()) || (existingItem.getEnchantLevel() != upgradeHolder.getRequiredItemEnchant()))
		{
			player.sendPacket(new ExUpgradeSystemResultPacket(0, 0));
			return ValueTask.CompletedTask;
		}
		
		// Store old item enchantment info.
		ItemInfo itemEnchantment = new ItemInfo(existingItem);
		
		// Get materials.
		player.destroyItem("UpgradeEquipment", _objectId, 1, player, true);
		foreach (ItemHolder material in upgradeHolder.getMaterials())
		{
			player.destroyItemByItemId("UpgradeEquipment", material.getId(), material.getCount(), player, true);
		}
		
		if (adena > 0)
		{
			player.reduceAdena("UpgradeEquipment", adena, player, true);
		}
		
		// Give item.
		Item addedItem = player.addItem("UpgradeEquipment", upgradeHolder.getResultItemId(), 1, player, true);
		if (upgradeHolder.isAnnounce())
		{
			Broadcast.toAllOnlinePlayers(new ExItemAnnouncePacket(player, addedItem, ExItemAnnouncePacket.UPGRADE));
		}
		
		// Transfer item enchantments.
		if (addedItem.isEquipable())
		{
			addedItem.setAugmentation(itemEnchantment.getAugmentation(), false);
			if (addedItem.isWeapon() && (addedItem.getTemplate().getAttributes() == null))
			{
				if (itemEnchantment.getAttackElementPower() > 0)
				{
					addedItem.setAttribute(new AttributeHolder(itemEnchantment.getAttackElementType(), itemEnchantment.getAttackElementPower()), false);
				}
			}
			else if (addedItem.getTemplate().getAttributes() == null)
			{
				if (itemEnchantment.getAttributeDefence(AttributeType.FIRE) > 0)
				{
					addedItem.setAttribute(new AttributeHolder(AttributeType.FIRE, itemEnchantment.getAttributeDefence(AttributeType.FIRE)), false);
				}
				if (itemEnchantment.getAttributeDefence(AttributeType.WATER) > 0)
				{
					addedItem.setAttribute(new AttributeHolder(AttributeType.WATER, itemEnchantment.getAttributeDefence(AttributeType.WATER)), false);
				}
				if (itemEnchantment.getAttributeDefence(AttributeType.WIND) > 0)
				{
					addedItem.setAttribute(new AttributeHolder(AttributeType.WIND, itemEnchantment.getAttributeDefence(AttributeType.WIND)), false);
				}
				if (itemEnchantment.getAttributeDefence(AttributeType.EARTH) > 0)
				{
					addedItem.setAttribute(new AttributeHolder(AttributeType.EARTH, itemEnchantment.getAttributeDefence(AttributeType.EARTH)), false);
				}
				if (itemEnchantment.getAttributeDefence(AttributeType.HOLY) > 0)
				{
					addedItem.setAttribute(new AttributeHolder(AttributeType.HOLY, itemEnchantment.getAttributeDefence(AttributeType.HOLY)), false);
				}
				if (itemEnchantment.getAttributeDefence(AttributeType.DARK) > 0)
				{
					addedItem.setAttribute(new AttributeHolder(AttributeType.DARK, itemEnchantment.getAttributeDefence(AttributeType.DARK)), false);
				}
			}
			
			if (itemEnchantment.getSoulCrystalOptions() != null)
			{
				int pos = -1;
				foreach (EnsoulOption ensoul in itemEnchantment.getSoulCrystalOptions())
				{
					pos++;
					addedItem.addSpecialAbility(ensoul, pos, 1, false);
				}
			}
			
			if (itemEnchantment.getSoulCrystalSpecialOptions() != null)
			{
				foreach (EnsoulOption ensoul in itemEnchantment.getSoulCrystalSpecialOptions())
				{
					addedItem.addSpecialAbility(ensoul, 0, 2, false);
				}
			}
			
			if (itemEnchantment.getVisualId() > 0)
			{
				ItemVariables oldVars = existingItem.getVariables();
				ItemVariables newVars = addedItem.getVariables();
				newVars.set(ItemVariables.VISUAL_ID, oldVars.getInt(ItemVariables.VISUAL_ID, 0));
				newVars.set(ItemVariables.VISUAL_APPEARANCE_STONE_ID, oldVars.getInt(ItemVariables.VISUAL_APPEARANCE_STONE_ID, 0));
				newVars.set(ItemVariables.VISUAL_APPEARANCE_LIFE_TIME, oldVars.getLong(ItemVariables.VISUAL_APPEARANCE_LIFE_TIME, 0));
				newVars.storeMe();
				addedItem.scheduleVisualLifeTime();
			}
		}
		
		// Apply update holder enchant.
		int enchantLevel = upgradeHolder.getResultItemEnchant();
		if (enchantLevel > 0)
		{
			addedItem.setEnchantLevel(enchantLevel);
		}
		
		// Save item.
		addedItem.updateDatabase(true);
		
		// Send result packet.
		player.sendPacket(new ExUpgradeSystemResultPacket(addedItem.ObjectId, 1));
		player.sendItemList();
        
        return ValueTask.CompletedTask;
    }
}