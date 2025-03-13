using L2Dn.GameServer.Data.Xml;
using L2Dn.GameServer.Handlers;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.BuyList;
using L2Dn.GameServer.Network.OutgoingPackets;
using L2Dn.GameServer.Utilities;
using NLog;
using Config = L2Dn.GameServer.Configuration.Config;

namespace L2Dn.GameServer.Scripts.Handlers.BypassHandlers;

public class Wear: IBypassHandler
{
	private static readonly Logger _logger = LogManager.GetLogger(nameof(Wear));

	private static readonly string[] COMMANDS =
    [
        "Wear",
    ];

	public bool useBypass(string command, Player player, Creature? target)
	{
		if (target == null || !target.isNpc())
		{
			return false;
		}

		if (!Config.ALLOW_WEAR)
		{
			return false;
		}

		try
		{
			StringTokenizer st = new StringTokenizer(command, " ");
			st.nextToken();

			if (st.countTokens() < 1)
			{
				return false;
			}

			showWearWindow(player, int.Parse(st.nextToken()));
			return true;
		}
		catch (Exception e)
		{
			_logger.Warn("Exception in " + GetType().Name, e);
		}

		return false;
	}

	private void showWearWindow(Player player, int value)
	{
		ProductList? buyList = BuyListData.getInstance().getBuyList(value);
		if (buyList == null)
		{
			_logger.Warn("BuyList not found! BuyListId:" + value);
			player.sendPacket(ActionFailedPacket.STATIC_PACKET);
			return;
		}

		player.setInventoryBlockingStatus(true);

		player.sendPacket(new ShopPreviewListPacket(buyList, player.getAdena()));
	}

	public string[] getBypassList()
	{
		return COMMANDS;
	}
}