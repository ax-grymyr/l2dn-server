using L2Dn.Extensions;
using L2Dn.GameServer.Data.Xml;
using L2Dn.GameServer.Enums;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Holders;
using L2Dn.GameServer.Model.Items.Instances;
using L2Dn.GameServer.Model.Variables;
using L2Dn.GameServer.Network.Enums;
using L2Dn.GameServer.Network.OutgoingPackets;
using L2Dn.GameServer.Network.OutgoingPackets.LuckyGame;
using L2Dn.GameServer.Utilities;
using L2Dn.Network;
using L2Dn.Packets;
using L2Dn.Utilities;

namespace L2Dn.GameServer.Network.IncomingPackets.LuckyGame;

public struct RequestLuckyGamePlayPacket: IIncomingPacket<GameSession>
{
    private const int FORTUNE_READING_TICKET = 23767;
    private const int LUXURY_FORTUNE_READING_TICKET = 23768;
    private LuckyGameType _type;
    private int _reading;

    public void ReadContent(PacketBitReader reader)
    {
	    _type = (LuckyGameType)Math.Clamp(reader.ReadInt32(), 0, (int)EnumUtil.GetMaxValue<LuckyGameType>());
        _reading = Math.Clamp(reader.ReadInt32(), 0, 50); // max play is 50
    }

    public ValueTask ProcessAsync(Connection connection, GameSession session)
    {
        Player? player = session.Player;
        if (player == null)
            return ValueTask.CompletedTask;

		int index = _type == LuckyGameType.LUXURY ? 102 : 2; // move to event config
		LuckyGameDataHolder holder = LuckyGameData.getInstance().getLuckyGameDataByIndex(index);
		if (holder == null)
			return ValueTask.CompletedTask;
		
		long tickets = _type == LuckyGameType.LUXURY ? player.getInventory().getInventoryItemCount(LUXURY_FORTUNE_READING_TICKET, -1) : player.getInventory().getInventoryItemCount(FORTUNE_READING_TICKET, -1);
		if (tickets < _reading)
		{
			player.sendPacket(SystemMessageId.YOU_DO_NOT_HAVE_ENOUGH_TICKETS_YOU_CANNOT_CONTINUE_THE_GAME);
			player.sendPacket(_type == LuckyGameType.LUXURY
				? ExBettingLuckyGameResultPacket.LUXURY_INVALID_ITEM_COUNT
				: ExBettingLuckyGameResultPacket.NORMAL_INVALID_ITEM_COUNT);
			
			return ValueTask.CompletedTask;
		}
		
		int playCount = player.getVariables().getInt(PlayerVariables.FORTUNE_TELLING_VARIABLE, 0);
		bool blackCat = player.getVariables().getBoolean(PlayerVariables.FORTUNE_TELLING_BLACK_CAT_VARIABLE, false);
		Map<LuckyGameItemType, List<ItemHolder>> rewards = new();
		for (int i = 0; i < _reading; i++)
		{
			double chance = 100 * Rnd.nextDouble();
			double totalChance = 0;
			foreach (ItemChanceHolder item in holder.getCommonReward())
			{
				totalChance += item.getChance();
				if (totalChance >= chance)
				{
					rewards.computeIfAbsent(LuckyGameItemType.COMMON, _ => new()).add(item);
					break;
				}
			}
			playCount++;
			if (playCount >= holder.getMinModifyRewardGame() && playCount <= holder.getMaxModifyRewardGame() && !blackCat)
			{
				List<ItemChanceHolder> modifyReward = holder.getModifyReward();
				double chanceModify = 100 * Rnd.nextDouble();
				totalChance = 0;
				foreach (ItemChanceHolder item in modifyReward)
				{
					totalChance += item.getChance();
					if (totalChance >= chanceModify)
					{
						rewards.computeIfAbsent(LuckyGameItemType.RARE, _ => new()).add(item);
						blackCat = true;
						break;
					}
				}
				
				if (playCount == holder.getMaxModifyRewardGame())
				{
					rewards.computeIfAbsent(LuckyGameItemType.RARE, _ => new())
						.add(modifyReward.get(Rnd.get(modifyReward.size())));
					
					blackCat = true;
				}
			}
		}

		int totalWeight = rewards.values().Select(list =>
			list.Select(item => ItemData.getInstance().getTemplate(item.getId()).getWeight()).Sum()).Sum();
		
		// Check inventory capacity
		if (!rewards.isEmpty() && (!player.getInventory().validateCapacity(rewards.size()) || !player.getInventory().validateWeight(totalWeight)))
		{
			player.sendPacket(_type == LuckyGameType.LUXURY ? ExBettingLuckyGameResultPacket.LUXURY_INVALID_CAPACITY : ExBettingLuckyGameResultPacket.NORMAL_INVALID_CAPACITY);
			player.sendPacket(SystemMessageId.YOUR_INVENTORY_IS_EITHER_FULL_OR_OVERWEIGHT);
			return ValueTask.CompletedTask;
		}
		
		if (!player.destroyItemByItemId("LuckyGame", _type == LuckyGameType.LUXURY ? LUXURY_FORTUNE_READING_TICKET : FORTUNE_READING_TICKET, _reading, player, true))
		{
			player.sendPacket(_type == LuckyGameType.LUXURY ? ExBettingLuckyGameResultPacket.LUXURY_INVALID_ITEM_COUNT : ExBettingLuckyGameResultPacket.NORMAL_INVALID_ITEM_COUNT);
			return ValueTask.CompletedTask;
		}
		
		for (int i = 0; i < _reading; i++)
		{
			int serverGameNumber = LuckyGameData.getInstance().increaseGame();
			holder.getUniqueReward().Where(reward => reward.getPoints() == serverGameNumber).ForEach(item =>
				rewards.computeIfAbsent(LuckyGameItemType.UNIQUE, _ => new()).add(item));
		}
		
		player.sendPacket(new ExBettingLuckyGameResultPacket(LuckyGameResultType.SUCCESS, _type, rewards, (int) (_type == LuckyGameType.LUXURY ? player.getInventory().getInventoryItemCount(LUXURY_FORTUNE_READING_TICKET, -1) : player.getInventory().getInventoryItemCount(FORTUNE_READING_TICKET, -1))));
		
		foreach (var reward in rewards)
		{
			foreach (ItemHolder r in reward.Value)
			{
				Item item = player.addItem("LuckyGame", r.getId(), r.getCount(), player, true);
				if (reward.Key == LuckyGameItemType.UNIQUE)
				{
					SystemMessagePacket sm = new SystemMessagePacket(SystemMessageId.CONGRATULATIONS_C1_HAS_OBTAINED_S2_X_S3_IN_THE_STANDARD_LUCKY_GAME);
					sm.Params.addPcName(player);
					sm.Params.addLong(r.getCount());
					sm.Params.addItemName(item);
					player.broadcastPacket(sm, 1000);
					break;
				}
			}
		}
		
		player.sendItemList();
		
		player.getVariables().set(PlayerVariables.FORTUNE_TELLING_VARIABLE, playCount >= 50 ? playCount - 50 : playCount);
		if (blackCat && playCount < 50)
		{
			player.getVariables().set(PlayerVariables.FORTUNE_TELLING_BLACK_CAT_VARIABLE, true);
		}
        
        return ValueTask.CompletedTask;
    }
}