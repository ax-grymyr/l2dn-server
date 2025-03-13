using L2Dn.GameServer.Data.Xml;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Actor.Request;
using L2Dn.GameServer.Model.Items.Instances;
using L2Dn.GameServer.Network.OutgoingPackets.RandomCraft;
using L2Dn.GameServer.StaticData;
using L2Dn.GameServer.Utilities;
using L2Dn.Network;
using L2Dn.Packets;

namespace L2Dn.GameServer.Network.IncomingPackets.RandomCraft;

public struct ExRequestRandomCraftExtractPacket: IIncomingPacket<GameSession>
{
    private Map<int, long> _items;

    public void ReadContent(PacketBitReader reader)
    {
        int size = reader.ReadInt32();
        _items = new Map<int, long>();
        for (int i = 0; i < size; i++)
        {
            int objId = reader.ReadInt32();
            long count = reader.ReadInt64();
            if (count > 0)
            {
                _items.put(objId, count);
            }
        }
    }

    public ValueTask ProcessAsync(Connection connection, GameSession session)
    {
        if (!Config.ENABLE_RANDOM_CRAFT)
            return ValueTask.CompletedTask;

        Player? player = session.Player;
        if (player == null)
            return ValueTask.CompletedTask;

        if (player.hasItemRequest() || player.hasRequest<RandomCraftRequest>())
            return ValueTask.CompletedTask;

        player.addRequest(new RandomCraftRequest(player));

        long points = 0;
        long fee = 0;
        Map<int, long> toDestroy = new();
        foreach (var e in _items)
        {
            int objId = e.Key;
            long count = e.Value;
            if (count < 1)
            {
                player.removeRequest<RandomCraftRequest>();
                return ValueTask.CompletedTask;
            }

            Item? item = player.getInventory().getItemByObjectId(objId);
            if (item != null)
            {
                if (count > item.getCount())
                {
                    continue;
                }

                toDestroy.put(objId, count);
                points += RandomCraftData.getInstance().getPoints(item.getId()) * count;
                fee += RandomCraftData.getInstance().getFee(item.getId()) * count;
            }
            else
            {
                player.sendPacket(new ExCraftExtractPacket());
            }
        }

        if (points < 1 || fee < 0)
        {
            player.removeRequest<RandomCraftRequest>();
            return ValueTask.CompletedTask;
        }

        if (player.reduceAdena("RandomCraft Extract", fee, player, true))
        {
            foreach (var e in toDestroy)
            {
                player.destroyItem("RandomCraft Extract", e.Key, e.Value, player, true);
            }

            player.getRandomCraft().addCraftPoints(checked((int)points));
        }

        player.sendPacket(new ExCraftInfoPacket(player));
        player.sendPacket(new ExCraftExtractPacket());
        player.removeRequest<RandomCraftRequest>();

        return ValueTask.CompletedTask;
    }
}