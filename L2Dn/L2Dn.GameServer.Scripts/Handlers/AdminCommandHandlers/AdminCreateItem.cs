using L2Dn.GameServer.Data.Xml;
using L2Dn.GameServer.Handlers;
using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Items;
using L2Dn.GameServer.Model.Items.Instances;
using L2Dn.GameServer.Network;
using L2Dn.GameServer.Network.OutgoingPackets;
using L2Dn.GameServer.Utilities;
using NLog;

namespace L2Dn.GameServer.Scripts.Handlers.AdminCommandHandlers;

/**
 * This class handles following admin commands: - itemcreate = show menu - create_item <id> [num] = creates num items with respective id, if num is not specified, assumes 1.
 * @version $Revision: 1.2.2.2.2.3 $ $Date: 2005/04/11 10:06:06 $
 */
public class AdminCreateItem: IAdminCommandHandler
{
    private static readonly Logger _logger = LogManager.GetLogger(nameof(AdminCreateItem));
	private static readonly string[] ADMIN_COMMANDS =
    [
        "admin_itemcreate",
		"admin_create_item",
		"admin_create_coin",
		"admin_give_item_target",
		"admin_give_item_to_all",
		"admin_delete_item",
		"admin_use_item",
    ];

	public bool useAdminCommand(string command, Player activeChar)
	{
		if (command.equals("admin_itemcreate"))
		{
			AdminHtml.showAdminHtml(activeChar, "itemcreation.htm");
		}
		else if (command.startsWith("admin_create_item"))
		{
			try
			{
				string val = command.Substring(17);
				StringTokenizer st = new StringTokenizer(val);
				if (st.countTokens() == 2)
				{
					string id = st.nextToken();
					int idval = int.Parse(id);
					string num = st.nextToken();
					long numval = long.Parse(num);
					createItem(activeChar, activeChar, idval, numval);
				}
				else if (st.countTokens() == 1)
				{
					string id = st.nextToken();
					int idval = int.Parse(id);
					createItem(activeChar, activeChar, idval, 1);
				}
			}
			catch (IndexOutOfRangeException e)
			{
                _logger.Error(e);
				BuilderUtil.sendSysMessage(activeChar, "Usage: //create_item <itemId> [amount]");
			}
			catch (FormatException nfe)
			{
                _logger.Error(nfe);
				BuilderUtil.sendSysMessage(activeChar, "Specify a valid number.");
			}
			AdminHtml.showAdminHtml(activeChar, "itemcreation.htm");
		}
		else if (command.startsWith("admin_create_coin"))
		{
			try
			{
				string val = command.Substring(17);
				StringTokenizer st = new StringTokenizer(val);
				if (st.countTokens() == 2)
				{
					string name = st.nextToken();
					int idval = getCoinId(name);
					if (idval > 0)
					{
						string num = st.nextToken();
						long numval = long.Parse(num);
						createItem(activeChar, activeChar, idval, numval);
					}
				}
				else if (st.countTokens() == 1)
				{
					string name = st.nextToken();
					int idval = getCoinId(name);
					createItem(activeChar, activeChar, idval, 1);
				}
			}
			catch (IndexOutOfRangeException e)
			{
                _logger.Error(e);
				BuilderUtil.sendSysMessage(activeChar, "Usage: //create_coin <name> [amount]");
			}
			catch (FormatException nfe)
			{
                _logger.Error(nfe);
				BuilderUtil.sendSysMessage(activeChar, "Specify a valid number.");
			}
			AdminHtml.showAdminHtml(activeChar, "itemcreation.htm");
		}
		else if (command.startsWith("admin_give_item_target"))
		{
			try
			{
				WorldObject? target = activeChar.getTarget();
				if (target == null || !target.isPlayer())
				{
					BuilderUtil.sendSysMessage(activeChar, "Invalid target.");
					return false;
				}

				string val = command.Substring(22);
				StringTokenizer st = new StringTokenizer(val);
				if (st.countTokens() == 2)
				{
					string id = st.nextToken();
					int idval = int.Parse(id);
					string num = st.nextToken();
					long numval = long.Parse(num);
					createItem(activeChar, (Player) target, idval, numval);
				}
				else if (st.countTokens() == 1)
				{
					string id = st.nextToken();
					int idval = int.Parse(id);
					createItem(activeChar, (Player) target, idval, 1);
				}
			}
			catch (IndexOutOfRangeException e)
			{
                _logger.Error(e);
				BuilderUtil.sendSysMessage(activeChar, "Usage: //give_item_target <itemId> [amount]");
			}
			catch (FormatException nfe)
			{
                _logger.Error(nfe);
				BuilderUtil.sendSysMessage(activeChar, "Specify a valid number.");
			}
			AdminHtml.showAdminHtml(activeChar, "itemcreation.htm");
		}
		else if (command.startsWith("admin_give_item_to_all"))
		{
			string val = command.Substring(22);
			StringTokenizer st = new StringTokenizer(val);
			int idval = 0;
			long numval = 0;
			if (st.countTokens() == 2)
			{
				string id = st.nextToken();
				idval = int.Parse(id);
				string num = st.nextToken();
				numval = long.Parse(num);
			}
			else if (st.countTokens() == 1)
			{
				string id = st.nextToken();
				idval = int.Parse(id);
				numval = 1;
			}
			int counter = 0;
			ItemTemplate? template = ItemData.getInstance().getTemplate(idval);
			if (template == null)
			{
				BuilderUtil.sendSysMessage(activeChar, "This item doesn't exist.");
				return false;
			}
			if (numval > 10 && !template.isStackable())
			{
				BuilderUtil.sendSysMessage(activeChar, "This item does not stack - Creation aborted.");
				return false;
			}
			foreach (Player onlinePlayer in World.getInstance().getPlayers())
			{
                GameSession? onlinePlayerClient = onlinePlayer.getClient();
				if (activeChar != onlinePlayer && onlinePlayer.isOnline() && onlinePlayerClient != null && !onlinePlayerClient.IsDetached)
				{
					onlinePlayer.getInventory().addItem("Admin", idval, numval, onlinePlayer, activeChar);
					onlinePlayer.sendMessage("Admin spawned " + numval + " " + template.getName() + " in your inventory.");
					counter++;
				}
			}
			activeChar.sendMessage(counter + " players rewarded with " + template.getName());
		}
		else if (command.startsWith("admin_delete_item"))
		{
			string val = command.Substring(18);
			StringTokenizer st = new StringTokenizer(val);
			int idval = 0;
			long numval = 0;
			if (st.countTokens() == 2)
			{
				string id = st.nextToken();
				idval = int.Parse(id);
				string num = st.nextToken();
				numval = long.Parse(num);
			}
			else if (st.countTokens() == 1)
			{
				string id = st.nextToken();
				idval = int.Parse(id);
				numval = 1;
			}

            Item? item = (Item?)World.getInstance().findObject(idval);
			int? ownerId = item?.getOwnerId();
			if (ownerId > 0 && item != null)
			{
				Player? player = World.getInstance().getPlayer(ownerId.Value);
				if (player == null)
				{
					BuilderUtil.sendSysMessage(activeChar, "Player is not online.");
					return false;
				}

				if (numval == 0)
				{
					numval = item.getCount();
				}

				player.getInventory().destroyItem("AdminDelete", idval, numval, activeChar, null);
				activeChar.sendPacket(new GMViewItemListPacket(1, player));
				BuilderUtil.sendSysMessage(activeChar, "Item deleted.");
			}
			else
			{
				BuilderUtil.sendSysMessage(activeChar, "Item doesn't have owner.");
				return false;
			}
		}
		else if (command.startsWith("admin_use_item"))
		{
			string val = command.Substring(15);
			int idval = int.Parse(val);
			Item? item = (Item?)World.getInstance().findObject(idval);
			int? ownerId = item?.getOwnerId();
			if (ownerId > 0 && item != null)
			{
				Player? player = World.getInstance().getPlayer(ownerId.Value);
				if (player == null)
				{
					BuilderUtil.sendSysMessage(activeChar, "Player is not online.");
					return false;
				}

				// equip
				if (item.isEquipable())
				{
					player.useEquippableItem(item, false);
				}
				else
				{
					IItemHandler? ih = ItemHandler.getInstance().getHandler(item.getEtcItem());
					if (ih != null)
					{
						ih.useItem(player, item, false);
					}
				}
				activeChar.sendPacket(new GMViewItemListPacket(1, player));
			}
			else
			{
				BuilderUtil.sendSysMessage(activeChar, "Item doesn't have owner.");
				return false;
			}
		}
		return true;
	}

	public string[] getAdminCommandList()
	{
		return ADMIN_COMMANDS;
	}

	private void createItem(Player activeChar, Player target, int id, long num)
	{
		ItemTemplate? template = ItemData.getInstance().getTemplate(id);
		if (template == null)
		{
			BuilderUtil.sendSysMessage(activeChar, "This item doesn't exist.");
			return;
		}
		if (num > 10 && !template.isStackable())
		{
			BuilderUtil.sendSysMessage(activeChar, "This item does not stack - Creation aborted.");
			return;
		}

		target.getInventory().addItem("Admin", id, num, target, activeChar);
		if (activeChar != target)
		{
			target.sendMessage("Admin spawned " + num + " " + template.getName() + " in your inventory.");
		}
		target.sendItemList();

		BuilderUtil.sendSysMessage(activeChar, "You have spawned " + num + " " + template.getName() + "(" + id + ") in " + target.getName() + " inventory.");
		target.sendPacket(new ExAdenaInvenCountPacket(target));
	}

	private int getCoinId(string name)
	{
		int id;
		if (name.equalsIgnoreCase("adena"))
		{
			id = 57;
		}
		else if (name.equalsIgnoreCase("ancientadena"))
		{
			id = 97145;
		}
		else if (name.equalsIgnoreCase("blueeva"))
		{
			id = 4355;
		}
		else if (name.equalsIgnoreCase("goldeinhasad"))
		{
			id = 4356;
		}
		else if (name.equalsIgnoreCase("silvershilen"))
		{
			id = 4357;
		}
		else if (name.equalsIgnoreCase("bloodypaagrio"))
		{
			id = 4358;
		}
		else if (name.equalsIgnoreCase("lcoin"))
		{
			id = 91663;
		}
		else
		{
			id = 0;
		}
		return id;
	}
}