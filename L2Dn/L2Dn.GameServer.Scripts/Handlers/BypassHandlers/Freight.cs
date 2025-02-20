using L2Dn.GameServer.Handlers;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.ItemContainers;
using L2Dn.GameServer.Model.Items.Instances;
using L2Dn.GameServer.Network.Enums;
using L2Dn.GameServer.Network.OutgoingPackets;
using L2Dn.GameServer.Utilities;

namespace L2Dn.GameServer.Scripts.Handlers.BypassHandlers;

/**
 * @author UnAfraid
 */
public class Freight: IBypassHandler
{
	private static readonly string[] COMMANDS =
    [
        "package_withdraw",
		"package_deposit",
    ];

	public bool useBypass(string command, Player player, Creature? target)
	{
		if (target is null || !target.isNpc())
		{
			return false;
		}

		if (command.equalsIgnoreCase(COMMANDS[0]))
		{
			PlayerFreight freight = player.getFreight();
			if (freight != null)
			{
				if (freight.getSize() > 0)
				{
					player.setActiveWarehouse(freight);
					foreach (Item i in freight.getItems())
					{
						if (i.isTimeLimitedItem() && i.getRemainingTime() <= TimeSpan.Zero)
						{
                            freight.destroyItem("ItemInstance", i, player, null);
						}
					}
					player.sendPacket(new WarehouseWithdrawalListPacket(1, player, WarehouseWithdrawalListPacket.FREIGHT));
					player.sendPacket(new WarehouseWithdrawalListPacket(2, player, WarehouseWithdrawalListPacket.FREIGHT));
				}
				else
				{
					player.sendPacket(SystemMessageId.YOU_HAVE_NOT_DEPOSITED_ANY_ITEMS_IN_YOUR_WAREHOUSE);
				}
			}
		}
		else if (command.equalsIgnoreCase(COMMANDS[1]))
		{
			if (player.getAccountChars().Count < 1)
			{
				player.sendPacket(SystemMessageId.THAT_CHARACTER_DOES_NOT_EXIST);
			}
			else
			{
				player.sendPacket(new PackageToListPacket(player.getAccountChars()));
			}
		}
		return false;
	}

	public string[] getBypassList()
	{
		return COMMANDS;
	}
}