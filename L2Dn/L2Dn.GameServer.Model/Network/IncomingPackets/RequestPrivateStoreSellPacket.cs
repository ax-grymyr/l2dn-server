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

public struct RequestPrivateStoreSellPacket: IIncomingPacket<GameSession>
{
    private int _storePlayerId;
    private ItemRequest[]? _items;

    public void ReadContent(PacketBitReader reader)
    {
        _storePlayerId = reader.ReadInt32();
        int itemsCount = reader.ReadInt32();
        if (itemsCount <= 0 || itemsCount > Config.Character.MAX_ITEM_IN_PACKET)
        {
            return;
        }

        _items = new ItemRequest[itemsCount];
        for (int i = 0; i < itemsCount; i++)
        {
            int slot = reader.ReadInt32();
            int itemId = reader.ReadInt32();
            reader.ReadInt16(); // TODO analyse this
            reader.ReadInt16(); // TODO analyse this
            long count = reader.ReadInt64();
            long price = reader.ReadInt64();
            reader.ReadInt32(); // visual id
            reader.ReadInt32(); // option 1
            reader.ReadInt32(); // option 2

            int soulCrystals = reader.ReadByte();
            for (int s = 0; s < soulCrystals; s++)
            {
                reader.ReadInt32(); // soul crystal option
            }

            int soulCrystals2 = reader.ReadByte();
            for (int s = 0; s < soulCrystals2; s++)
            {
                reader.ReadInt32(); // sa effect
            }

            if (/* (slot < 1) || */ itemId < 1 || count < 1 || price < 0)
            {
                _items = null;
                return;
            }

            _items[i] = new ItemRequest(slot, itemId, count, price);
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
        //     player.sendMessage("You are selling items too fast.");
        //     return ValueTask.CompletedTask;
        // }

        Player? storePlayer = World.getInstance().getPlayer(_storePlayerId);
        if (storePlayer == null || !player.IsInsideRadius3D(storePlayer, Npc.INTERACTION_DISTANCE))
            return ValueTask.CompletedTask;

        if (player.getInstanceWorld() != storePlayer.getInstanceWorld())
            return ValueTask.CompletedTask;

        if (storePlayer.getPrivateStoreType() != PrivateStoreType.BUY || player.isCursedWeaponEquipped())
            return ValueTask.CompletedTask;

        TradeList storeList = storePlayer.getBuyList();
        if (storeList == null)
            return ValueTask.CompletedTask;

        if (!player.getAccessLevel().AllowTransaction)
        {
            player.sendMessage("Transactions are disabled for your Access Level.");
            player.sendPacket(ActionFailedPacket.STATIC_PACKET);
            return ValueTask.CompletedTask;
        }

        if (!storeList.privateStoreSell(player, _items))
        {
            player.sendPacket(ActionFailedPacket.STATIC_PACKET);
            PacketLogger.Instance.Warn("PrivateStore sell has failed due to invalid list or request. Player: " +
                                       player.getName() + ", Private store of: " + storePlayer.getName());

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