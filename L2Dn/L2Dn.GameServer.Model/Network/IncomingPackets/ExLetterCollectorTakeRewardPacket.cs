using L2Dn.GameServer.Dto;
using L2Dn.GameServer.InstanceManagers.Events;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Holders;
using L2Dn.GameServer.Model.ItemContainers;
using L2Dn.GameServer.Network.Enums;
using L2Dn.GameServer.Utilities;
using L2Dn.Network;
using L2Dn.Packets;
using L2Dn.Utilities;

namespace L2Dn.GameServer.Network.IncomingPackets;

public struct ExLetterCollectorTakeRewardPacket: IIncomingPacket<GameSession>
{
    private int _wordId;

    public void ReadContent(PacketBitReader reader)
    {
        _wordId = reader.ReadInt32();
    }

    public ValueTask ProcessAsync(Connection connection, GameSession session)
    {
        Player? player = session.Player;
        if (player == null)
            return ValueTask.CompletedTask;

        PlayerInventory inventory = player.getInventory();
        if (inventory == null)
            return ValueTask.CompletedTask;

        LetterCollectorManager.LetterCollectorRewardHolder? lcrh = LetterCollectorManager.getInstance().getRewards(_wordId);
        if (lcrh == null)
            return ValueTask.CompletedTask;

        List<ItemHolder>? word = LetterCollectorManager.getInstance().getWord(_wordId);
        if (word == null)
            return ValueTask.CompletedTask;

        foreach (ItemHolder needLetter in word)
        {
            if (inventory.getInventoryItemCount(needLetter.Id, -1) < needLetter.Count)
                return ValueTask.CompletedTask;
        }

        foreach (ItemHolder destroyLetter in word)
        {
            if (!player.destroyItemByItemId("LetterCollector", destroyLetter.Id, destroyLetter.Count, player, true))
                return ValueTask.CompletedTask;
        }

        ItemChanceHolder? rewardItem = getRandomReward(lcrh.getRewards(), lcrh.getChance());
        if (rewardItem == null)
        {
            player.sendPacket(SystemMessageId.NOTHING_HAPPENED);
            return ValueTask.CompletedTask;
        }

        player.addItem("LetterCollector", rewardItem.Id, rewardItem.Count, rewardItem.EnchantmentLevel, player, true);
        return ValueTask.CompletedTask;
    }

    private static ItemChanceHolder? getRandomReward(List<ItemChanceHolder> rewards, double holderChance)
    {
        double chance = Rnd.get(holderChance);
        double itemChance = 0;
        foreach (ItemChanceHolder rewardItem in rewards)
        {
            itemChance += rewardItem.Chance;
            if (chance <= itemChance)
            {
                if (rewardItem.Id == -1)
                {
                    return null;
                }

                return rewardItem;
            }
        }

        return null;
    }
}