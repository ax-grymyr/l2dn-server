using L2Dn.GameServer.Enums;
using L2Dn.GameServer.Handlers;
using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.ItemContainers;
using L2Dn.GameServer.Model.Items.Instances;
using L2Dn.GameServer.Network.OutgoingPackets;

namespace L2Dn.GameServer.Scripts.Handlers.AdminCommandHandlers;

/**
 * @author Mobius
 */
public class AdminDestroyItems: IAdminCommandHandler
{
	private static readonly string[] ADMIN_COMMANDS =
    [
        "admin_destroy_items",
		"admin_destroy_all_items",
		"admin_destroyitems",
		"admin_destroyallitems",
    ];
	
	public bool useAdminCommand(string command, Player activeChar)
	{
		PlayerInventory inventory = activeChar.getInventory();
		List<ItemInfo> itemsToUpdate = []; 
		foreach (Item item in inventory.getItems())
		{
			if (item.isEquipped() && !command.Contains("all"))
			{
				continue;
			}

			itemsToUpdate.Add(new ItemInfo(item, ItemChangeType.REMOVED));
			inventory.destroyItem("Admin Destroy", item, activeChar, null);
		}

		InventoryUpdatePacket iu = new InventoryUpdatePacket(itemsToUpdate);
		activeChar.sendPacket(iu);
		return true;
	}
	
	public string[] getAdminCommandList()
	{
		return ADMIN_COMMANDS;
	}
}