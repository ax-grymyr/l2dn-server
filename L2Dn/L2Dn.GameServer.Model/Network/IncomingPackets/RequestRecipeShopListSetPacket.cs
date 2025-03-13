using L2Dn.GameServer.Data.Xml;
using L2Dn.GameServer.Enums;
using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.ItemContainers;
using L2Dn.GameServer.Model.Zones;
using L2Dn.GameServer.Network.Enums;
using L2Dn.GameServer.Network.OutgoingPackets;
using L2Dn.GameServer.TaskManagers;
using L2Dn.GameServer.Utilities;
using L2Dn.Network;
using L2Dn.Packets;
using Config = L2Dn.GameServer.Configuration.Config;

namespace L2Dn.GameServer.Network.IncomingPackets;

public struct RequestRecipeShopListSetPacket: IIncomingPacket<GameSession>
{
    private const int BATCH_LENGTH = 12;

    private ManufactureItem[]? _items;

    public void ReadContent(PacketBitReader reader)
    {
        int count = reader.ReadInt32();
        if (count <= 0 || count > Config.Character.MAX_ITEM_IN_PACKET || count * BATCH_LENGTH != reader.Length)
        {
            return;
        }

        _items = new ManufactureItem[count];
        for (int i = 0; i < count; i++)
        {
            int id = reader.ReadInt32();
            long cost = reader.ReadInt64();
            if (cost < 0)
            {
                _items = null;
                return;
            }

            _items[i] = new ManufactureItem(id, cost);
        }
    }

    public ValueTask ProcessAsync(Connection connection, GameSession session)
    {
        Player? player = session.Player;
        if (player == null)
            return ValueTask.CompletedTask;

        if (_items == null)
        {
            player.setPrivateStoreType(PrivateStoreType.NONE);
            player.broadcastUserInfo();
            return ValueTask.CompletedTask;
        }

        if (AttackStanceTaskManager.getInstance().hasAttackStanceTask(player) || player.isInDuel())
        {
            player.sendPacket(SystemMessageId.WHILE_YOU_ARE_ENGAGED_IN_COMBAT_YOU_CANNOT_OPERATE_A_PRIVATE_STORE_OR_PRIVATE_WORKSHOP);
            player.sendPacket(ActionFailedPacket.STATIC_PACKET);
            return ValueTask.CompletedTask;
        }

        if (player.isInsideZone(ZoneId.NO_STORE))
        {
            player.sendPacket(SystemMessageId.YOU_CANNOT_OPEN_A_PRIVATE_WORKSHOP_HERE);
            player.sendPacket(ActionFailedPacket.STATIC_PACKET);
            return ValueTask.CompletedTask;
        }

        player.getManufactureItems().Clear();
        foreach (ManufactureItem i in _items)
        {
            RecipeList? list = RecipeData.getInstance().getRecipeList(i.getRecipeId());
            if (list == null)
                return ValueTask.CompletedTask;

            if (!player.getDwarvenRecipeBook().Contains(list) && !player.getCommonRecipeBook().Contains(list))
            {
                Util.handleIllegalPlayerAction(player,
                    "Warning!! " + player + " of account " + player.getAccountName() +
                    " tried to set recipe which he dont have.", Config.DEFAULT_PUNISH);

                return ValueTask.CompletedTask;
            }

            if (i.getCost() > Inventory.MAX_ADENA)
            {
                Util.handleIllegalPlayerAction(player,
                    "Warning!! " + player + " of account " + player.getAccountName() +
                    " tried to set price more than " + Inventory.MAX_ADENA + " adena in Private Manufacture.",
                    Config.DEFAULT_PUNISH);

                return ValueTask.CompletedTask;
            }

            player.getManufactureItems().put(i.getRecipeId(), i);
        }

        player.setStoreName(!player.hasManufactureShop() ? "" : player.getStoreName());
        player.setPrivateStoreType(PrivateStoreType.MANUFACTURE);
        player.sitDown();
        player.broadcastUserInfo();
        Broadcast.toSelfAndKnownPlayers(player, new RecipeShopMsgPacket(player));

        return ValueTask.CompletedTask;
    }
}