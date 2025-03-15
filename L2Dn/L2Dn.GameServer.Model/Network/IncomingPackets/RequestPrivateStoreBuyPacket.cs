using L2Dn.GameServer.Data.Sql;
using L2Dn.GameServer.Enums;
using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Network.OutgoingPackets;
using L2Dn.GameServer.Utilities;
using L2Dn.Geometry;
using L2Dn.Network;
using L2Dn.Packets;
using Config = L2Dn.GameServer.Configuration.Config;

namespace L2Dn.GameServer.Network.IncomingPackets;

public struct RequestPrivateStoreBuyPacket: IIncomingPacket<GameSession>
{
    private const int BATCH_LENGTH = 20; // length of the one item

    private int _storePlayerId;
    private Set<ItemRequest>? _items;

    public void ReadContent(PacketBitReader reader)
    {
        _storePlayerId = reader.ReadInt32();
        int count = reader.ReadInt32();
        if (count <= 0 || count > Config.Character.MAX_ITEM_IN_PACKET || count * BATCH_LENGTH != reader.Length)
            return;

        _items = new();
        for (int i = 0; i < count; i++)
        {
            int objectId = reader.ReadInt32();
            long cnt = reader.ReadInt64();
            long price = reader.ReadInt64();
            if (objectId < 1 || cnt < 1 || price < 0)
            {
                _items = null;
                return;
            }

            _items.Add(new ItemRequest(objectId, cnt, price));
        }
    }

    public ValueTask ProcessAsync(Connection connection, GameSession session)
    {
	    Player? player = session.Player;
	    if (player == null)
		    return ValueTask.CompletedTask;

		if (_items == null)
		{
			player.sendPacket(ActionFailedPacket.STATIC_PACKET);
			return ValueTask.CompletedTask;
		}

		if (player.isRegisteredOnEvent())
		{
			player.sendMessage("You cannot open a private store while participating in an event.");
			return ValueTask.CompletedTask;
		}

		// TODO: flood protection
		// if (!client.getFloodProtectors().canPerformTransaction())
		// {
		// 	player.sendMessage("You are buying items too fast.");
		// 	return ValueTask.CompletedTask;
		// }

		WorldObject? obj = World.getInstance().getPlayer(_storePlayerId);
		if (obj == null || player.isCursedWeaponEquipped())
			return ValueTask.CompletedTask;

		Player storePlayer = (Player)obj;
		if (!player.IsInsideRadius3D(storePlayer, Npc.INTERACTION_DISTANCE))
			return ValueTask.CompletedTask;

		if (player.getInstanceWorld() != storePlayer.getInstanceWorld())
			return ValueTask.CompletedTask;

		if (!(storePlayer.getPrivateStoreType() == PrivateStoreType.SELL ||
		      storePlayer.getPrivateStoreType() == PrivateStoreType.PACKAGE_SELL))
			return ValueTask.CompletedTask;

		TradeList storeList = storePlayer.getSellList();
		if (storeList == null)
			return ValueTask.CompletedTask;

		if (!player.getAccessLevel().AllowTransaction)
		{
			player.sendMessage("Transactions are disabled for your Access Level.");
			player.sendPacket(ActionFailedPacket.STATIC_PACKET);
			return ValueTask.CompletedTask;
		}

		if (storePlayer.getPrivateStoreType() == PrivateStoreType.PACKAGE_SELL && storeList.getItemCount() > _items.size())
		{
			string msgErr = "[RequestPrivateStoreBuy] " + player + " tried to buy less items than sold by package-sell, ban this player for bot usage!";
			Util.handleIllegalPlayerAction(player, msgErr, Config.General.DEFAULT_PUNISH);
			return ValueTask.CompletedTask;
		}

		int result = storeList.privateStoreBuy(player, _items);
		if (result > 0)
		{
			player.sendPacket(ActionFailedPacket.STATIC_PACKET);
			if (result > 1)
			{
				PacketLogger.Instance.Warn("PrivateStore buy has failed due to invalid list or request. Player: " +
				                           player.getName() + ", Private store of: " + storePlayer.getName());
			}

			return ValueTask.CompletedTask;
		}

		// Update offline trade record, if realtime saving is enabled
        GameSession? storePlayerClient = storePlayer.getClient();
		if (Config.OfflineTrade.OFFLINE_TRADE_ENABLE && Config.OfflineTrade.STORE_OFFLINE_TRADE_IN_REALTIME &&
		    (storePlayerClient == null || storePlayerClient.IsDetached))
		{
			OfflineTraderTable.getInstance().onTransaction(storePlayer, storeList.getItemCount() == 0, false);
		}

		if (storeList.getItemCount() == 0)
		{
			storePlayer.setPrivateStoreType(PrivateStoreType.NONE);
			storePlayer.broadcastUserInfo();
		}

		return ValueTask.CompletedTask;
    }
}