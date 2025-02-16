using L2Dn.GameServer.Handlers;
using L2Dn.GameServer.InstanceManagers;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.ItemAuction;
using L2Dn.GameServer.Network.Enums;
using L2Dn.GameServer.Network.OutgoingPackets;
using L2Dn.GameServer.Utilities;
using NLog;

namespace L2Dn.GameServer.Scripts.Handlers.BypassHandlers;

public class ItemAuctionLink: IBypassHandler
{
	private static readonly Logger _logger = LogManager.GetLogger(nameof(ItemAuctionLink));
	
	private static readonly string[] COMMANDS =
	{
		"ItemAuction"
	};
	
	public bool useBypass(string command, Player player, Creature target)
	{
		if (!target.isNpc())
		{
			return false;
		}
		
		if (!Config.ALT_ITEM_AUCTION_ENABLED)
		{
			player.sendPacket(SystemMessageId.IT_IS_NOT_AN_AUCTION_PERIOD);
			return true;
		}
		
		ItemAuctionInstance au = ItemAuctionManager.getInstance().getManagerInstance(target.getId());
		if (au == null)
		{
			return false;
		}
		
		try
		{
			StringTokenizer st = new StringTokenizer(command);
			st.nextToken(); // bypass "ItemAuction"
			if (!st.hasMoreTokens())
			{
				return false;
			}
			
			string cmd = st.nextToken();
			if ("show".equalsIgnoreCase(cmd))
			{
				// TODO: flood protectors
				// if (!player.getClient().getFloodProtectors().canUseItemAuction())
				// {
				// 	return false;
				// }
				
				if (player.isItemAuctionPolling())
				{
					return false;
				}
				
				ItemAuction currentAuction = au.getCurrentAuction();
				ItemAuction nextAuction = au.getNextAuction();
				if (currentAuction == null)
				{
					player.sendPacket(SystemMessageId.IT_IS_NOT_AN_AUCTION_PERIOD);
					
					if (nextAuction != null)
					{
						player.sendMessage("The next auction will begin on the " +
						                   nextAuction.getStartingTime().ToString("HH:mm:ss dd.MM.yyyy") + ".");
					}
					return true;
				}
				
				player.sendPacket(new ExItemAuctionInfoPacket(false, currentAuction, nextAuction));
			}
			else if ("cancel".equalsIgnoreCase(cmd))
			{
				bool returned = false;
				foreach (ItemAuction auction in au.getAuctionsByBidder(player.ObjectId))
				{
					if (auction.cancelBid(player))
					{
						returned = true;
					}
				}
				if (!returned)
				{
					player.sendPacket(SystemMessageId.THERE_ARE_NO_OFFERINGS_I_OWN_OR_I_MADE_A_BID_FOR);
				}
			}
			else
			{
				return false;
			}
		}
		catch (Exception e)
		{
			_logger.Warn("Exception in " + GetType().Name, e);
		}
		
		return true;
	}
	
	public string[] getBypassList()
	{
		return COMMANDS;
	}
}