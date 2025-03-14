using L2Dn.Extensions;
using L2Dn.GameServer.Data.Sql;
using L2Dn.GameServer.Enums;
using L2Dn.GameServer.InstanceManagers;
using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Olympiads;
using L2Dn.GameServer.Model.Zones;
using L2Dn.GameServer.Network;
using L2Dn.GameServer.Network.OutgoingPackets;
using L2Dn.Network;
using NLog;
using Config = L2Dn.GameServer.Configuration.Config;

namespace L2Dn.GameServer.Utilities;

public class OfflineTradeUtil
{
	protected static readonly Logger LOGGER_ACCOUNTING = LogManager.GetLogger("accounting");

	private OfflineTradeUtil()
	{
		// utility class
	}

	/**
	 * Check whether player is able to enter offline mode.
	 * @param player the player to be check.
	 * @return {@code true} if the player is allowed to remain as off-line shop.
	 */
	private static bool offlineMode(Player player)
	{
		if ((player == null) || player.isInOlympiadMode() || player.isRegisteredOnEvent() || player.isJailed() || (player.getVehicle() != null))
		{
			return false;
		}

		bool canSetShop = false;
		switch (player.getPrivateStoreType())
		{
			case PrivateStoreType.SELL:
			case PrivateStoreType.PACKAGE_SELL:
			case PrivateStoreType.BUY:
			case PrivateStoreType.MANUFACTURE:
			{
				canSetShop = Config.OfflineTrade.OFFLINE_TRADE_ENABLE;
				break;
			}
			default:
			{
				canSetShop = Config.OfflineTrade.OFFLINE_CRAFT_ENABLE && player.isCrafting();
				break;
			}
		}

		if (Config.OfflineTrade.OFFLINE_MODE_IN_PEACE_ZONE && !player.isInsideZone(ZoneId.PEACE))
		{
			canSetShop = false;
		}

		// Check whether client is null or player is already in offline mode.
		GameSession? client = player.getClient();
		if ((client == null) || client.IsDetached)
		{
			return false;
		}

		return canSetShop;
	}

	/**
	 * Manages the disconnection process of offline traders.
	 * @param player
	 * @return {@code true} when player entered offline mode, otherwise {@code false}
	 */
	public static bool enteredOfflineMode(Player player)
	{
		if (!offlineMode(player))
		{
			return false;
		}

		GameSession? client = player.getClient();
		Connection? connection = client?.Connection;
		if (client == null || connection == null)
			return false;

		World.OFFLINE_TRADE_COUNT++;

		connection.Send(ServerClosePacket.STATIC_PACKET, SendPacketOptions.CloseAfterSending);
		if (!Config.DualboxCheck.DUALBOX_COUNT_OFFLINE_TRADERS)
		{
			AntiFeedManager.getInstance().onDisconnect(client);
		}

		client.IsDetached = true;

		player.leaveParty();
		OlympiadManager.getInstance().unRegisterNoble(player);

		// If the Player has Pet, unsummon it
		Summon? pet = player.getPet();
		if (pet != null)
		{
			pet.setRestoreSummon(true);
			pet.unSummon(player);
			pet = player.getPet();

			// Dead pet wasn't unsummoned, broadcast npcinfo changes (pet will be without owner name - means owner offline)
			if (pet != null)
			{
				pet.broadcastNpcInfo(0);
			}
		}

		player.getServitors().Values.ForEach(s =>
		{
			s.setRestoreSummon(true);
			s.unSummon(player);
		});

		if (Config.OfflineTrade.OFFLINE_SET_NAME_COLOR)
		{
			player.getAppearance().setNameColor(Config.OfflineTrade.OFFLINE_NAME_COLOR);
			player.broadcastUserInfo();
		}

		if (player.getOfflineStartTime() == null)
		{
			player.setOfflineStartTime(DateTime.UtcNow);
		}

		// Store trade on exit, if realtime saving is enabled.
		if (Config.OfflineTrade.STORE_OFFLINE_TRADE_IN_REALTIME)
		{
			OfflineTraderTable.getInstance().onTransaction(player, false, true);
		}

		player.storeMe();
		LOGGER_ACCOUNTING.Info("Entering offline mode, " + client);

		if (!Config.OfflineTrade.OFFLINE_ABNORMAL_EFFECTS.IsDefaultOrEmpty)
		{
			player.getEffectList().startAbnormalVisualEffect(Config.OfflineTrade.OFFLINE_ABNORMAL_EFFECTS.GetRandomElement());
		}

		return true;
	}
}