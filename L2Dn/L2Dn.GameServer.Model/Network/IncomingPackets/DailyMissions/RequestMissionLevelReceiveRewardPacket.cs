using L2Dn.GameServer.Data.Xml;
using L2Dn.GameServer.Dto;
using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Actor.Request;
using L2Dn.GameServer.Model.Holders;
using L2Dn.GameServer.Network.OutgoingPackets.DailyMissions;
using L2Dn.Network;
using L2Dn.Packets;
using ThreadPool = L2Dn.GameServer.Utilities.ThreadPool;

namespace L2Dn.GameServer.Network.IncomingPackets.DailyMissions;

public struct RequestMissionLevelReceiveRewardPacket: IIncomingPacket<GameSession>
{
    private int _level;
    private int _rewardType;

    public void ReadContent(PacketBitReader reader)
    {
        _level = reader.ReadInt32();
        _rewardType = reader.ReadInt32();
    }

    public ValueTask ProcessAsync(Connection connection, GameSession session)
    {
        Player? player = session.Player;
        if (player == null)
            return ValueTask.CompletedTask;

        MissionLevelHolder? holder = MissionLevel.getInstance()
	        .getMissionBySeason(MissionLevel.getInstance().getCurrentSeason());

        if (holder == null)
        {
            player.removeRequest<RewardRequest>();
            return ValueTask.CompletedTask;
        }

		if (player.hasRequest<RewardRequest>())
			return ValueTask.CompletedTask;

		player.addRequest(new RewardRequest(player));

		MissionLevelPlayerDataHolder info = player.getMissionLevelProgress();
		switch (_rewardType)
		{
			case 1:
			{
                ItemHolder? reward = holder.getNormalRewards().get(_level);
				if (reward == null || info.getCollectedNormalRewards().Contains(_level) ||
                    (info.getCurrentLevel() != _level && info.getCurrentLevel() < _level))
				{
					player.removeRequest<RewardRequest>();
					return ValueTask.CompletedTask;
				}

				player.addItem("Mission Level", reward.Id, reward.Count, null, true);
				info.addToCollectedNormalRewards(_level);
				info.storeInfoInVariable(player);
				break;
			}
			case 2:
			{
                ItemHolder? reward = holder.getKeyRewards().get(_level);
				if (reward == null || info.getCollectedKeyRewards().Contains(_level) ||
                    (info.getCurrentLevel() != _level && info.getCurrentLevel() < _level))
				{
					player.removeRequest<RewardRequest>();
					return ValueTask.CompletedTask;
				}

				player.addItem("Mission Level", reward.Id, reward.Count, null, true);
				info.addToCollectedKeyReward(_level);
				info.storeInfoInVariable(player);
				return ValueTask.CompletedTask;
			}
			case 3:
			{
                ItemHolder? specialReward = holder.getSpecialReward();
				if (specialReward == null || info.getCollectedSpecialReward() || (info.getCurrentLevel() != _level && info.getCurrentLevel() < _level))
				{
					player.removeRequest<RewardRequest>();
					return ValueTask.CompletedTask;
				}

				player.addItem("Mission Level", specialReward.Id, specialReward.Count, null, true);
				info.setCollectedSpecialReward(true);
				info.storeInfoInVariable(player);
				break;
			}
			case 4:
            {
                ItemHolder? bonusReward = holder.getBonusReward();
				if (!holder.getBonusRewardIsAvailable() || bonusReward == null || !info.getCollectedSpecialReward() || info.getCollectedBonusReward() || (info.getCurrentLevel() != _level && info.getCurrentLevel() < _level))
				{
					player.removeRequest<RewardRequest>();
					return ValueTask.CompletedTask;
				}

				if (holder.getBonusRewardByLevelUp())
				{
					int maxNormalLevel = holder.getBonusLevel();
					int availableReward = -1;
					for (int level = maxNormalLevel; level <= holder.getMaxLevel(); level++)
					{
						if (level <= info.getCurrentLevel() && !info.getListOfCollectedBonusRewards().Contains(level))
						{
							availableReward = level;
							break;
						}
					}
					if (availableReward != -1)
					{
						info.addToListOfCollectedBonusRewards(availableReward);
					}
					else
					{
						player.removeRequest<RewardRequest>();
						return ValueTask.CompletedTask;
					}
				}
				else
				{
					info.setCollectedBonusReward(true);
				}

				player.addItem("Mission Level", bonusReward.Id, bonusReward.Count, null, true);
				info.storeInfoInVariable(player);
				break;
			}
		}

 		player.sendPacket(new ExMissionLevelRewardListPacket(player, holder));

		ThreadPool.schedule(() => player.removeRequest<RewardRequest>(), 300);

        return ValueTask.CompletedTask;
    }
}