using L2Dn.GameServer.Handlers;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Network.Enums;
using L2Dn.GameServer.Network.OutgoingPackets;
using L2Dn.GameServer.Utilities;
using NLog;
using Config = L2Dn.GameServer.Configuration.Config;

namespace L2Dn.GameServer.Scripts.Handlers.BypassHandlers;

public class PrivateWarehouse: IBypassHandler
{
	private static readonly Logger _logger = LogManager.GetLogger(nameof(PrivateWarehouse));

	private static readonly string[] COMMANDS =
    [
        "withdrawp",
		"depositp",
    ];

	public bool useBypass(string command, Player player, Creature? target)
	{
		if (!Config.ALLOW_WAREHOUSE)
		{
			return false;
		}

		if (target is null || !target.isNpc())
		{
			return false;
		}

		if (player.hasItemRequest())
		{
			return false;
		}

		try
		{
			if (command.toLowerCase().startsWith(COMMANDS[0])) // WithdrawP
			{
				showWithdrawWindow(player);
				return true;
			}

            if (command.toLowerCase().startsWith(COMMANDS[1])) // DepositP
            {
                player.sendPacket(ActionFailedPacket.STATIC_PACKET);
                player.setActiveWarehouse(player.getWarehouse());
                player.setInventoryBlockingStatus(true);
                player.sendPacket(new WarehouseDepositListPacket(1, player, WarehouseDepositListPacket.PRIVATE));
                player.sendPacket(new WarehouseDepositListPacket(2, player, WarehouseDepositListPacket.PRIVATE));
                return true;
            }

            return false;
		}
		catch (Exception e)
		{
			_logger.Warn("Exception in " + GetType().Name, e);
		}
		return false;
	}

	private void showWithdrawWindow(Player player)
	{
		player.sendPacket(ActionFailedPacket.STATIC_PACKET);
		player.setActiveWarehouse(player.getWarehouse());

		if (player.getWarehouse().getSize() == 0)
		{
			player.sendPacket(SystemMessageId.YOU_HAVE_NOT_DEPOSITED_ANY_ITEMS_IN_YOUR_WAREHOUSE);
			return;
		}

		player.sendPacket(new WarehouseWithdrawalListPacket(1, player, WarehouseWithdrawalListPacket.PRIVATE));
		player.sendPacket(new WarehouseWithdrawalListPacket(2, player, WarehouseWithdrawalListPacket.PRIVATE));
	}

	public string[] getBypassList()
	{
		return COMMANDS;
	}
}