using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.ItemContainers;
using L2Dn.GameServer.Model.Items.Instances;
using L2Dn.GameServer.Network.OutgoingPackets.RandomCraft;
using L2Dn.GameServer.Utilities;
using L2Dn.Network;
using L2Dn.Packets;
using Config = L2Dn.GameServer.Configuration.Config;

namespace L2Dn.GameServer.Network.IncomingPackets.RandomCraft;

public struct ExRequestRandomCraftLockSlotPacket: IIncomingPacket<GameSession>
{
    private static readonly int[] LOCK_PRICE =
    {
        100,
        500,
        1000
    };

    private int _id;

    public void ReadContent(PacketBitReader reader)
    {
        _id = reader.ReadInt32();
    }

    public ValueTask ProcessAsync(Connection connection, GameSession session)
    {
        if (!Config.RandomCraft.ENABLE_RANDOM_CRAFT)
            return ValueTask.CompletedTask;

        Player? player = session.Player;
        if (player == null)
            return ValueTask.CompletedTask;

        if (_id >= 0 && _id < 5)
        {
            PlayerRandomCraft rc = player.getRandomCraft();
            int lockedItemCount = rc.getLockedSlotCount();
            if (rc.getRewards().Count - 1 >= _id && lockedItemCount < 3)
            {
                int price = LOCK_PRICE[Math.Min(lockedItemCount, 2)];
                Item? lcoin = player.getInventory().getItemByItemId(Inventory.LCOIN_ID);
                if (lcoin != null && lcoin.getCount() >= price)
                {
                    player.destroyItem("RandomCraft Lock Slot", lcoin, price, player, true);
                    rc.getRewards()[_id].@lock();
                    player.sendPacket(new ExCraftRandomLockSlotPacket());
                    player.sendPacket(new ExCraftRandomInfoPacket(player));
                }
            }
        }

        return ValueTask.CompletedTask;
    }
}