using L2Dn.GameServer.Dto;
using L2Dn.GameServer.Enums;
using L2Dn.GameServer.Handlers;
using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Zones;
using L2Dn.GameServer.Network.Enums;
using L2Dn.GameServer.Network.OutgoingPackets;
using NLog;

namespace L2Dn.GameServer.Scripts.Handlers.PlayerActions;

/**
 * Open/Close private store player action handler.
 * @author Nik
 */
public class PrivateStore: IPlayerActionHandler
{
	private static readonly Logger LOGGER = LogManager.GetLogger(nameof(PrivateStore));
	
	public void useAction(Player player, ActionDataHolder data, bool ctrlPressed, bool shiftPressed)
	{
		PrivateStoreType type = (PrivateStoreType)data.OptionId;
		if (!Enum.IsDefined(type))
		{
			LOGGER.Warn("Incorrect private store type: " + data.OptionId);
			return;
		}
		
		// Player shouldn't be able to set stores if he/she is alike dead (dead or fake death)
		if (!player.canOpenPrivateStore())
		{
			if (player.isInsideZone(ZoneId.NO_STORE))
			{
				player.sendPacket(SystemMessageId.YOU_CANNOT_OPEN_A_PRIVATE_STORE_HERE);
			}
			
			player.sendPacket(ActionFailedPacket.STATIC_PACKET);
			return;
		}
		
		switch (type)
		{
			case PrivateStoreType.SELL:
			case PrivateStoreType.SELL_MANAGE:
			case PrivateStoreType.PACKAGE_SELL:
			{
				if ((player.getPrivateStoreType() == PrivateStoreType.SELL) || (player.getPrivateStoreType() == PrivateStoreType.SELL_MANAGE) || (player.getPrivateStoreType() == PrivateStoreType.PACKAGE_SELL))
				{
					player.setPrivateStoreType(PrivateStoreType.NONE);
				}
				break;
			}
			case PrivateStoreType.BUY:
			case PrivateStoreType.BUY_MANAGE:
			{
				if ((player.getPrivateStoreType() == PrivateStoreType.BUY) || (player.getPrivateStoreType() == PrivateStoreType.BUY_MANAGE))
				{
					player.setPrivateStoreType(PrivateStoreType.NONE);
				}
				break;
			}
			case PrivateStoreType.MANUFACTURE:
			{
				player.setPrivateStoreType(PrivateStoreType.NONE);
				player.broadcastUserInfo();
				break;
			}
		}
		
		if (player.getPrivateStoreType() == PrivateStoreType.NONE)
		{
			if (player.isSitting())
			{
				player.standUp();
			}
			
			switch (type)
			{
				case PrivateStoreType.SELL:
				case PrivateStoreType.SELL_MANAGE:
				case PrivateStoreType.PACKAGE_SELL:
				{
					player.setPrivateStoreType(PrivateStoreType.SELL_MANAGE);
					player.sendPacket(new PrivateStoreManageListSellPacket(1, player, type == PrivateStoreType.PACKAGE_SELL));
					player.sendPacket(new PrivateStoreManageListSellPacket(2, player, type == PrivateStoreType.PACKAGE_SELL));
					break;
				}
				case PrivateStoreType.BUY:
				case PrivateStoreType.BUY_MANAGE:
				{
					player.setPrivateStoreType(PrivateStoreType.BUY_MANAGE);
					player.sendPacket(new PrivateStoreManageListBuyPacket(1, player));
					player.sendPacket(new PrivateStoreManageListBuyPacket(2, player));
					break;
				}
				case PrivateStoreType.MANUFACTURE:
				{
					player.sendPacket(new RecipeShopManageListPacket(player, true));
					break;
				}
			}
		}
	}

	public bool isPetAction()
	{
		return false;
	}
}