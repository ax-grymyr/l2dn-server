using L2Dn.GameServer.Data.Xml;
using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Actor.Request;
using L2Dn.GameServer.Model.Holders;
using L2Dn.GameServer.Model.Items;
using L2Dn.GameServer.Network.Enums;
using L2Dn.GameServer.Network.OutgoingPackets;
using L2Dn.GameServer.Network.OutgoingPackets.HuntPasses;
using L2Dn.Network;
using L2Dn.Packets;
using ThreadPool = L2Dn.GameServer.Utilities.ThreadPool;

namespace L2Dn.GameServer.Network.IncomingPackets.HuntPasses;

public struct RequestHuntPassRewardAllPacket: IIncomingPacket<GameSession>
{
    private int _huntPassType;

    public void ReadContent(PacketBitReader reader)
    {
        _huntPassType = reader.ReadByte();
    }

    public ValueTask ProcessAsync(Connection connection, GameSession session)
    {
        Player? player = session.Player;
        if (player == null)
            return ValueTask.CompletedTask;

		if (player.hasRequest<RewardRequest>())
			return ValueTask.CompletedTask;

		player.addRequest(new RewardRequest(player));

		int rewardIndex;
		int premiumRewardIndex;
		bool inventoryLimitReached = false;
		HuntPass huntPass = player.getHuntPass();

		while (true)
		{
			rewardIndex = huntPass.getRewardStep();
			premiumRewardIndex = huntPass.getPremiumRewardStep();
			if ((rewardIndex >= HuntPassData.getInstance().getRewardsCount()) && (premiumRewardIndex >= HuntPassData.getInstance().getPremiumRewardsCount()))
			{
				break;
			}

			ItemHolder? reward = null;
			if (!huntPass.isPremium())
			{
				if (rewardIndex >= huntPass.getCurrentStep())
				{
					break;
				}

				reward = HuntPassData.getInstance().getRewards()[rewardIndex];
			}
			else
			{
				if (premiumRewardIndex >= huntPass.getCurrentStep())
				{
					break;
				}

				if (rewardIndex < HuntPassData.getInstance().getRewardsCount())
				{
					reward = HuntPassData.getInstance().getRewards()[rewardIndex];
				}
				else if (premiumRewardIndex < HuntPassData.getInstance().getPremiumRewardsCount())
				{
					reward = HuntPassData.getInstance().getPremiumRewards()[premiumRewardIndex];
				}
			}

			if (reward == null)
			{
				break;
			}

			ItemTemplate? itemTemplate = ItemData.getInstance().getTemplate(reward.getId());
			long weight = itemTemplate.getWeight() * reward.getCount();
			long slots = itemTemplate.isStackable() ? 1 : reward.getCount();
			if (!player.getInventory().validateWeight(weight) || !player.getInventory().validateCapacity(slots))
			{
				player.sendPacket(SystemMessageId.YOUR_INVENTORY_S_WEIGHT_SLOT_LIMIT_HAS_BEEN_EXCEEDED_SO_YOU_CAN_T_RECEIVE_THE_REWARD_PLEASE_FREE_UP_SOME_SPACE_AND_TRY_AGAIN);
				inventoryLimitReached = true;
				break;
			}

			normalReward(player);
			premiumReward(player);
			huntPass.setRewardStep(rewardIndex + 1);
		}

		if (!inventoryLimitReached)
		{
			huntPass.setRewardAlert(false);
		}

		player.sendPacket(new HuntPassInfoPacket(player, _huntPassType));
		player.sendPacket(new HuntPassSayhasSupportInfoPacket(player));
		player.sendPacket(new HuntPassSimpleInfoPacket(player));

		ThreadPool.schedule(() => player.removeRequest<RewardRequest>(), 300);

		return ValueTask.CompletedTask;
	}

	private static void rewardItem(Player player, ItemHolder reward)
	{
		if (reward.getId() == 72286) // Sayha's Grace Sustention Points
		{
			int count = (int) reward.getCount();
			player.getHuntPass().addSayhaTime(count);

			SystemMessagePacket msg = new SystemMessagePacket(SystemMessageId.YOU_VE_GOT_S1_SAYHA_S_GRACE_SUSTENTION_POINT_S);
			msg.Params.addInt(count);
			player.sendPacket(msg);
		}
		else
		{
			player.addItem("HuntPassReward", reward, player, true);
		}
	}

	private static void premiumReward(Player player)
	{
		HuntPass huntPass = player.getHuntPass();
		int premiumRewardIndex = huntPass.getPremiumRewardStep();
		if (premiumRewardIndex >= HuntPassData.getInstance().getPremiumRewardsCount())
		{
			return;
		}

		if (!huntPass.isPremium())
		{
			return;
		}

		rewardItem(player, HuntPassData.getInstance().getPremiumRewards()[premiumRewardIndex]);
		huntPass.setPremiumRewardStep(premiumRewardIndex + 1);
	}

	private static void normalReward(Player player)
	{
		HuntPass huntPass = player.getHuntPass();
		int rewardIndex = huntPass.getRewardStep();
		if (rewardIndex >= HuntPassData.getInstance().getRewardsCount())
		{
			return;
		}

		if (huntPass.isPremium() && ((huntPass.getPremiumRewardStep() < rewardIndex) ||
		                             (huntPass.getPremiumRewardStep() >=
		                              HuntPassData.getInstance().getPremiumRewardsCount())))
		{
			return;
		}

		rewardItem(player, HuntPassData.getInstance().getRewards()[rewardIndex]);
	}
}