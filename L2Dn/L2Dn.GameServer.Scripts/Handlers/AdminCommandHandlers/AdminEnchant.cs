using L2Dn.GameServer.Data.Xml;
using L2Dn.GameServer.Enums;
using L2Dn.GameServer.Handlers;
using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.ItemContainers;
using L2Dn.GameServer.Model.Items;
using L2Dn.GameServer.Model.Items.Instances;
using L2Dn.GameServer.Network.Enums;
using L2Dn.GameServer.Network.OutgoingPackets;
using L2Dn.GameServer.StaticData;
using L2Dn.GameServer.Utilities;
using NLog;

namespace L2Dn.GameServer.Scripts.Handlers.AdminCommandHandlers;

/**
 * This class handles following admin commands: - enchant_armor
 * @version $Revision: 1.3.2.1.2.10 $ $Date: 2005/08/24 21:06:06 $
 */
public class AdminEnchant: IAdminCommandHandler
{
	private static readonly Logger LOGGER = LogManager.GetLogger(nameof(AdminEnchant));

	private static readonly string[] ADMIN_COMMANDS =
    [
        "admin_seteh", // 6
		"admin_setec", // 10
		"admin_seteg", // 9
		"admin_setel", // 11
		"admin_seteb", // 12
		"admin_setew", // 7
		"admin_setes", // 8
		"admin_setle", // 1
		"admin_setre", // 2
		"admin_setlf", // 4
		"admin_setrf", // 5
		"admin_seten", // 3
		"admin_setun", // 0
		"admin_setba", // 13
		"admin_setbe",
		"admin_enchant",
    ];

	public bool useAdminCommand(string command, Player activeChar)
	{
		if (command.equals("admin_enchant"))
		{
			showMainPage(activeChar);
		}
		else
		{
			int slot = -1;
			if (command.startsWith("admin_seteh"))
			{
				slot = Inventory.PAPERDOLL_HEAD;
			}
			else if (command.startsWith("admin_setec"))
			{
				slot = Inventory.PAPERDOLL_CHEST;
			}
			else if (command.startsWith("admin_seteg"))
			{
				slot = Inventory.PAPERDOLL_GLOVES;
			}
			else if (command.startsWith("admin_seteb"))
			{
				slot = Inventory.PAPERDOLL_FEET;
			}
			else if (command.startsWith("admin_setel"))
			{
				slot = Inventory.PAPERDOLL_LEGS;
			}
			else if (command.startsWith("admin_setew"))
			{
				slot = Inventory.PAPERDOLL_RHAND;
			}
			else if (command.startsWith("admin_setes"))
			{
				slot = Inventory.PAPERDOLL_LHAND;
			}
			else if (command.startsWith("admin_setle"))
			{
				slot = Inventory.PAPERDOLL_LEAR;
			}
			else if (command.startsWith("admin_setre"))
			{
				slot = Inventory.PAPERDOLL_REAR;
			}
			else if (command.startsWith("admin_setlf"))
			{
				slot = Inventory.PAPERDOLL_LFINGER;
			}
			else if (command.startsWith("admin_setrf"))
			{
				slot = Inventory.PAPERDOLL_RFINGER;
			}
			else if (command.startsWith("admin_seten"))
			{
				slot = Inventory.PAPERDOLL_NECK;
			}
			else if (command.startsWith("admin_setun"))
			{
				slot = Inventory.PAPERDOLL_UNDER;
			}
			else if (command.startsWith("admin_setba"))
			{
				slot = Inventory.PAPERDOLL_CLOAK;
			}
			else if (command.startsWith("admin_setbe"))
			{
				slot = Inventory.PAPERDOLL_BELT;
			}

			if (slot != -1)
			{
				try
				{
					int ench = int.Parse(command.Substring(12));

					// check value
					if (ench < 0 || ench > 127)
					{
						BuilderUtil.sendSysMessage(activeChar, "You must set the enchant level to be between 0-127.");
					}
					else
					{
						setEnchant(activeChar, ench, slot);
					}
				}
				catch (IndexOutOfRangeException e)
				{
					if (Config.DEVELOPER)
					{
						LOGGER.Warn("Set enchant error: " + e);
					}
					BuilderUtil.sendSysMessage(activeChar, "Please specify a new enchant value.");
				}
				catch (FormatException e)
				{
					if (Config.DEVELOPER)
					{
						LOGGER.Warn("Set enchant error: " + e);
					}
					BuilderUtil.sendSysMessage(activeChar, "Please specify a valid new enchant value.");
				}
			}

			// show the enchant menu after an action
			showMainPage(activeChar);
		}
		return true;
	}

	private void setEnchant(Player activeChar, int ench, int slot)
	{
		// Get the target.
		Player? player = activeChar.getTarget() != null ? activeChar.getTarget()?.getActingPlayer() : activeChar;
		if (player == null)
		{
			activeChar.sendPacket(SystemMessageId.INVALID_TARGET);
			return;
		}

		// Now we need to find the equipped weapon of the targeted character...
		Item? itemInstance = null;

		// Only attempt to enchant if there is a weapon equipped.
		Item? paperdollInstance = player.getInventory().getPaperdollItem(slot);
		if (paperdollInstance != null && paperdollInstance.getLocationSlot() == slot)
		{
			itemInstance = paperdollInstance;
		}

		if (itemInstance != null)
		{
			int curEnchant = itemInstance.getEnchantLevel();

			// Set enchant value.
			int enchant = ench;
			if (Config.OVER_ENCHANT_PROTECTION && !player.isGM())
			{
				if (itemInstance.isWeapon())
				{
					if (enchant > EnchantItemGroupsData.getInstance().getMaxWeaponEnchant())
					{
						BuilderUtil.sendSysMessage(activeChar, "Maximum enchantment for weapon items is " + EnchantItemGroupsData.getInstance().getMaxWeaponEnchant() + ".");
						enchant = EnchantItemGroupsData.getInstance().getMaxWeaponEnchant();
					}
				}
				else if (itemInstance.getTemplate().getType2() == ItemTemplate.TYPE2_ACCESSORY)
				{
					if (enchant > EnchantItemGroupsData.getInstance().getMaxAccessoryEnchant())
					{
						BuilderUtil.sendSysMessage(activeChar, "Maximum enchantment for accessory items is " + EnchantItemGroupsData.getInstance().getMaxAccessoryEnchant() + ".");
						enchant = EnchantItemGroupsData.getInstance().getMaxAccessoryEnchant();
					}
				}
				else if (enchant > EnchantItemGroupsData.getInstance().getMaxArmorEnchant())
				{
					BuilderUtil.sendSysMessage(activeChar, "Maximum enchantment for armor items is " + EnchantItemGroupsData.getInstance().getMaxArmorEnchant() + ".");
					enchant = EnchantItemGroupsData.getInstance().getMaxArmorEnchant();
				}
			}
			player.getInventory().unEquipItemInSlot(slot);
			itemInstance.setEnchantLevel(enchant);
			player.getInventory().equipItem(itemInstance);

			// Send packets.
			InventoryUpdatePacket iu = new InventoryUpdatePacket(new ItemInfo(itemInstance, ItemChangeType.MODIFIED));
			player.sendInventoryUpdate(iu);
			player.broadcastUserInfo();

			// Information.
			BuilderUtil.sendSysMessage(activeChar, "Changed enchantment of " + player.getName() + "'s " + itemInstance.getTemplate().getName() + " from " + curEnchant + " to " + enchant + ".");
			player.sendMessage("Admin has changed the enchantment of your " + itemInstance.getTemplate().getName() + " from " + curEnchant + " to " + enchant + ".");
		}
	}

	private void showMainPage(Player activeChar)
	{
		AdminHtml.showAdminHtml(activeChar, "enchant.htm");
	}

	public string[] getAdminCommandList()
	{
		return ADMIN_COMMANDS;
	}
}