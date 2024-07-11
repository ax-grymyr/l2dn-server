using L2Dn.GameServer.Data.Xml;
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

        MissionLevelHolder holder = MissionLevel.getInstance()
	        .getMissionBySeason(MissionLevel.getInstance().getCurrentSeason());
 
		if (player.hasRequest<RewardRequest>())
			return ValueTask.CompletedTask;

		player.addRequest(new RewardRequest(player));
		
		MissionLevelPlayerDataHolder info = player.getMissionLevelProgress();
		switch (_rewardType)
		{
			case 1:
			{
				if (!holder.getNormalRewards().ContainsKey(_level) || info.getCollectedNormalRewards().Contains(_level) ||
				    (info.getCurrentLevel() != _level && info.getCurrentLevel() < _level))
				{
					player.removeRequest<RewardRequest>();
					return ValueTask.CompletedTask;
				}
				
				ItemHolder reward = holder.getNormalRewards().get(_level);
				player.addItem("Mission Level", reward.getId(), reward.getCount(), null, true);
				info.addToCollectedNormalRewards(_level);
				info.storeInfoInVariable(player);
				break;
			}
			case 2:
			{
				if (!holder.getKeyRewards().ContainsKey(_level) || info.getCollectedKeyRewards().Contains(_level) ||
				    (info.getCurrentLevel() != _level && info.getCurrentLevel() < _level))
				{
					player.removeRequest<RewardRequest>();
					return ValueTask.CompletedTask;
				}
				
				ItemHolder reward = holder.getKeyRewards().get(_level);
				player.addItem("Mission Level", reward.getId(), reward.getCount(), null, true);
				info.addToCollectedKeyReward(_level);
				info.storeInfoInVariable(player);
				return ValueTask.CompletedTask;
			}
			case 3:
			{
				if (holder.getSpecialReward() == null || info.getCollectedSpecialReward() || (info.getCurrentLevel() != _level && info.getCurrentLevel() < _level))
				{
					player.removeRequest<RewardRequest>();
					return ValueTask.CompletedTask;
				}
				
				ItemHolder reward = holder.getSpecialReward();
				player.addItem("Mission Level", reward.getId(), reward.getCount(), null, true);
				info.setCollectedSpecialReward(true);
				info.storeInfoInVariable(player);
				break;
			}
			case 4:
			{
				if (!holder.getBonusRewardIsAvailable() || holder.getBonusReward() == null || !info.getCollectedSpecialReward() || info.getCollectedBonusReward() || (info.getCurrentLevel() != _level && info.getCurrentLevel() < _level))
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
				
				ItemHolder reward = holder.getBonusReward();
				player.addItem("Mission Level", reward.getId(), reward.getCount(), null, true);
				info.storeInfoInVariable(player);
				break;
			}
		}
		
		player.sendPacket(new ExMissionLevelRewardListPacket(player));
		
		ThreadPool.schedule(() => player.removeRequest<RewardRequest>(), 300);
        
        return ValueTask.CompletedTask;
    }
}