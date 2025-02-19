using L2Dn.GameServer.Enums;
using L2Dn.GameServer.Handlers;
using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.ItemContainers;
using L2Dn.GameServer.Model.Items.Enchant.Attributes;
using L2Dn.GameServer.Model.Items.Instances;
using L2Dn.GameServer.Network.Enums;
using L2Dn.GameServer.Network.OutgoingPackets;
using L2Dn.GameServer.Utilities;
using L2Dn.Model.Enums;

namespace L2Dn.GameServer.Scripts.Handlers.AdminCommandHandlers;

/**
 * This class handles following admin commands: - delete = deletes target
 * @version $Revision: 1.2.2.1.2.4 $ $Date: 2005/04/11 10:05:56 $
 */
public class AdminElement: IAdminCommandHandler
{
	private static readonly string[] ADMIN_COMMANDS =
    [
        "admin_setlh",
		"admin_setlc",
		"admin_setll",
		"admin_setlg",
		"admin_setlb",
		"admin_setlw",
		"admin_setls",
    ];
	
	public bool useAdminCommand(string command, Player activeChar)
	{
		int armorType = -1;
		if (command.startsWith("admin_setlh"))
		{
			armorType = Inventory.PAPERDOLL_HEAD;
		}
		else if (command.startsWith("admin_setlc"))
		{
			armorType = Inventory.PAPERDOLL_CHEST;
		}
		else if (command.startsWith("admin_setlg"))
		{
			armorType = Inventory.PAPERDOLL_GLOVES;
		}
		else if (command.startsWith("admin_setlb"))
		{
			armorType = Inventory.PAPERDOLL_FEET;
		}
		else if (command.startsWith("admin_setll"))
		{
			armorType = Inventory.PAPERDOLL_LEGS;
		}
		else if (command.startsWith("admin_setlw"))
		{
			armorType = Inventory.PAPERDOLL_RHAND;
		}
		else if (command.startsWith("admin_setls"))
		{
			armorType = Inventory.PAPERDOLL_LHAND;
		}
		
		if (armorType != -1)
		{
			try
			{
				string[] args = command.Split(" ");
				AttributeType type = Enum.Parse<AttributeType>(args[1]);
				int value = int.Parse(args[2]);
				if ((!Enum.IsDefined(type) || (value < 0) || (value > 450)))
				{
					BuilderUtil.sendSysMessage(activeChar, "Usage: //setlh/setlc/setlg/setlb/setll/setlw/setls <element> <value>[0-450]");
					return false;
				}
				
				setElement(activeChar, type, value, armorType);
			}
			catch (Exception e)
			{
				BuilderUtil.sendSysMessage(activeChar, "Usage: //setlh/setlc/setlg/setlb/setll/setlw/setls <element>[0-5] <value>[0-450]");
				return false;
			}
		}
		
		return true;
	}
	
	public string[] getAdminCommandList()
	{
		return ADMIN_COMMANDS;
	}
	
	private void setElement(Player activeChar, AttributeType type, int value, int armorType)
	{
		// get the target
		WorldObject target = activeChar.getTarget();
		if (target == null)
		{
			target = activeChar;
		}
		Player player = null;
		if (target.isPlayer())
		{
			player = (Player) target;
		}
		else
		{
			activeChar.sendPacket(SystemMessageId.INVALID_TARGET);
			return;
		}
		
		Item itemInstance = null;
		
		// only attempt to enchant if there is a weapon equipped
		Item parmorInstance = player.getInventory().getPaperdollItem(armorType);
		if ((parmorInstance != null) && (parmorInstance.getLocationSlot() == armorType))
		{
			itemInstance = parmorInstance;
		}
		
		if (itemInstance != null)
		{
			string old;
			string current;
			AttributeHolder element = itemInstance.getAttribute(type);
			if (element == null)
			{
				old = "None";
			}
			else
			{
				old = element.ToString();
			}
			
			// set enchant value
			player.getInventory().unEquipItemInSlot(armorType);
			if (type == AttributeType.NONE)
			{
				itemInstance.clearAllAttributes();
			}
			else if (value < 1)
			{
				itemInstance.clearAttribute(type);
			}
			else
			{
				itemInstance.setAttribute(new AttributeHolder(type, value), true);
			}
			player.getInventory().equipItem(itemInstance);
			
			if (itemInstance.getAttributes() == null)
			{
				current = "None";
			}
			else
			{
				current = itemInstance.getAttribute(type).ToString();
			}
			
			// send packets
			InventoryUpdatePacket iu = new InventoryUpdatePacket(new ItemInfo(itemInstance, ItemChangeType.MODIFIED));
			player.sendInventoryUpdate(iu);
			
			// informations
			BuilderUtil.sendSysMessage(activeChar, "Changed elemental power of " + player.getName() + "'s " + itemInstance.getTemplate().getName() + " from " + old + " to " + current + ".");
			if (player != activeChar)
			{
				player.sendMessage(activeChar.getName() + " has changed the elemental power of your " + itemInstance.getTemplate().getName() + " from " + old + " to " + current + ".");
			}
		}
	}
}
