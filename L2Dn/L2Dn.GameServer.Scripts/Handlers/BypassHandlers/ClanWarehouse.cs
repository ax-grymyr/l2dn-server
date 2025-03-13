using L2Dn.GameServer.Handlers;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Clans;
using L2Dn.GameServer.Model.ItemContainers;
using L2Dn.GameServer.Model.Items.Instances;
using L2Dn.GameServer.Network.Enums;
using L2Dn.GameServer.Network.OutgoingPackets;
using L2Dn.GameServer.Utilities;
using NLog;
using Config = L2Dn.GameServer.Configuration.Config;
using Warehouse = L2Dn.GameServer.Model.Actor.Instances.Warehouse;

namespace L2Dn.GameServer.Scripts.Handlers.BypassHandlers;

public class ClanWarehouse: IBypassHandler
{
	private static readonly Logger _logger = LogManager.GetLogger(nameof(ClanWarehouse));

	private static readonly string[] COMMANDS =
    [
        "withdrawc",
		"depositc",
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

		Npc npc = (Npc) target;
		if (!(npc is Warehouse) && npc.getClan() != null)
		{
			return false;
		}

		if (player.hasItemRequest())
		{
			return false;
		}

        Clan? clan = player.getClan();
        if (clan == null)
        {
            player.sendPacket(SystemMessageId.YOU_DO_NOT_HAVE_THE_RIGHT_TO_USE_THE_CLAN_WAREHOUSE);
            return false;
        }

        if (clan.getLevel() == 0)
        {
            player.sendPacket(SystemMessageId.ONLY_CLANS_OF_CLAN_LEVEL_1_OR_ABOVE_CAN_USE_A_CLAN_WAREHOUSE);
            return false;
        }

        try
        {
            if (command.toLowerCase().startsWith(COMMANDS[0])) // WithdrawC
            {
                player.sendPacket(ActionFailedPacket.STATIC_PACKET);

                if (!player.hasClanPrivilege(ClanPrivilege.CL_VIEW_WAREHOUSE))
                {
                    player.sendPacket(SystemMessageId.YOU_DO_NOT_HAVE_THE_RIGHT_TO_USE_THE_CLAN_WAREHOUSE);
                    return true;
                }

                ItemContainer activeWarehouse = clan.getWarehouse();
                player.setActiveWarehouse(activeWarehouse);

                if (activeWarehouse.getSize() == 0)
                {
                    player.sendPacket(SystemMessageId.YOU_HAVE_NOT_DEPOSITED_ANY_ITEMS_IN_YOUR_WAREHOUSE);
                    return true;
                }

                foreach (Item i in activeWarehouse.getItems())
                {
                    if (i.isTimeLimitedItem() && i.getRemainingTime() <= TimeSpan.Zero)
                    {
                        activeWarehouse.destroyItem("ItemInstance", i, player, null);
                    }
                }

                player.sendPacket(new WarehouseWithdrawalListPacket(1, player, WarehouseWithdrawalListPacket.CLAN));
                player.sendPacket(new WarehouseWithdrawalListPacket(2, player, WarehouseWithdrawalListPacket.CLAN));
                return true;
            }

            if (command.toLowerCase().startsWith(COMMANDS[1])) // DepositC
            {
                player.sendPacket(ActionFailedPacket.STATIC_PACKET);
                player.setActiveWarehouse(clan.getWarehouse());
                player.setInventoryBlockingStatus(true);
                player.sendPacket(new WarehouseDepositListPacket(1, player, WarehouseDepositListPacket.CLAN));
                player.sendPacket(new WarehouseDepositListPacket(2, player, WarehouseDepositListPacket.CLAN));
                return true;
            }

            return false;
        }
        catch (Exception e)
        {
            _logger.Warn("Exception in " + GetType().Name + ": " + e);
        }
        return false;
	}

	public string[] getBypassList()
	{
		return COMMANDS;
	}
}