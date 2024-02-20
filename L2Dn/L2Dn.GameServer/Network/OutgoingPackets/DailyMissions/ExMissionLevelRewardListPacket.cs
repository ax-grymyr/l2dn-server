using L2Dn.GameServer.Data.Xml;
using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Holders;
using L2Dn.Packets;

namespace L2Dn.GameServer.Network.OutgoingPackets.DailyMissions;

public readonly struct ExMissionLevelRewardListPacket: IOutgoingPacket
{
    private readonly Player _player;
    private readonly MissionLevelPlayerDataHolder _info;
    private readonly int _maxNormalLevel;
    private readonly string _currentSeason = MissionLevel.getInstance().getCurrentSeason().ToString();

    private readonly MissionLevelHolder _holder =
        MissionLevel.getInstance().getMissionBySeason(MissionLevel.getInstance().getCurrentSeason());

    private readonly List<int> _collectedNormalRewards;
    private readonly List<int> _collectedKeyRewards;
    private readonly List<int> _collectedBonusRewards;

    public ExMissionLevelRewardListPacket(Player player)
    {
        _player = player;

        _holder = MissionLevel.getInstance().getMissionBySeason(MissionLevel.getInstance().getCurrentSeason());
        _currentSeason = MissionLevel.getInstance().getCurrentSeason().ToString();

        // After normal rewards there will be bonus.
        _maxNormalLevel = _holder.getBonusLevel();

        _info = _player.getMissionLevelProgress();
        _collectedNormalRewards = _info.getCollectedNormalRewards();
        _collectedKeyRewards = _info.getCollectedKeyRewards();
        _collectedBonusRewards = _info.getListOfCollectedBonusRewards();
    }

    public void WriteContent(PacketBitWriter writer)
    {
        writer.WritePacketCode(OutgoingPacketCodes.EX_MISSION_LEVEL_REWARD_LIST);

        if (_info.getCurrentLevel() == 0)
        {
            writer.WriteInt32(1); // 0 -> does not work, -1 -> game crushed
            writer.WriteInt32(3); // Type
            writer.WriteInt32(-1); // Level
            writer.WriteInt32(0); // State
        }
        else
        {
            sendAvailableRewardsList(writer);
        }

        writer.WriteInt32(_info.getCurrentLevel()); // Level
        writer.WriteInt32(getPercent()); // PointPercent
        string year = _currentSeason.Substring(0, 4);
        writer.WriteInt32(int.Parse(year)); // SeasonYear
        string month = _currentSeason.Substring(4, 6);
        writer.WriteInt32(int.Parse(month)); // SeasonMonth
        writer.WriteInt32(getAvailableRewards()); // TotalRewardsAvailable
        if (_holder.getBonusRewardIsAvailable() && _holder.getBonusRewardByLevelUp())
        {
            bool check = false;
            for (int level = _maxNormalLevel; level <= _holder.getMaxLevel(); level++)
            {
                if ((level <= _info.getCurrentLevel()) && !_collectedBonusRewards.Contains(level))
                {
                    check = true;
                    break;
                }
            }

            writer.WriteInt32(check); // ExtraRewardsAvailable
        }
        else
        {
            if (_holder.getBonusRewardIsAvailable() && _info.getCollectedSpecialReward() &&
                !_info.getCollectedBonusReward())
            {
                writer.WriteInt32(1); // ExtraRewardsAvailable
            }
            else
            {
                writer.WriteInt32(0); // ExtraRewardsAvailable
            }
        }

        writer.WriteInt32(0); // RemainSeasonTime / does not work? / not used?
    }

    private int getAvailableRewards()
    {
        int availableRewards = 0;
        foreach (int level in _holder.getNormalRewards().Keys)
        {
            if ((level <= _info.getCurrentLevel()) && !_collectedNormalRewards.Contains(level))
                availableRewards++;
        }

        foreach (int level in _holder.getKeyRewards().Keys)
        {
            if ((level <= _info.getCurrentLevel()) && !_collectedKeyRewards.Contains(level))
                availableRewards++;
        }

        if (_holder.getBonusRewardIsAvailable() && _holder.getBonusRewardByLevelUp() &&
            _info.getCollectedSpecialReward())
        {
            List<int> collectedBonusRewards = _info.getListOfCollectedBonusRewards();
            for (int level = _maxNormalLevel; level <= _holder.getMaxLevel(); level++)
            {
                if ((level <= _info.getCurrentLevel()) && !collectedBonusRewards.Contains(level))
                {
                    availableRewards++;
                    break;
                }
            }
        }
        else if (_holder.getBonusRewardIsAvailable() && _holder.getBonusRewardByLevelUp() &&
                 (_info.getCurrentLevel() >= _maxNormalLevel))
        {
            availableRewards++;
        }
        else if (_holder.getBonusRewardIsAvailable() && (_info.getCurrentLevel() >= _holder.getMaxLevel()) &&
                 !_info.getCollectedBonusReward() && _info.getCollectedSpecialReward())
        {
            availableRewards++;
        }
        else if ((_info.getCurrentLevel() >= _holder.getMaxLevel()) && !_info.getCollectedBonusReward())
        {
            availableRewards++;
        }

        return availableRewards;
    }

    private int getTotalRewards()
    {
        int totalRewards = 0;
        foreach (int level in _holder.getNormalRewards().Keys)
        {
            if (level <= _info.getCurrentLevel())
                totalRewards++;
        }

        foreach (int level in _holder.getKeyRewards().Keys)
        {
            if (level <= _info.getCurrentLevel())
                totalRewards++;
        }

        if (_holder.getBonusRewardByLevelUp() && _info.getCollectedSpecialReward() &&
            _holder.getBonusRewardIsAvailable() && (_maxNormalLevel <= _info.getCurrentLevel()))
        {
            for (int level = _maxNormalLevel; level <= _holder.getMaxLevel(); level++)
            {
                if (level <= _info.getCurrentLevel())
                {
                    totalRewards++;
                    break;
                }
            }
        }
        else if (_info.getCollectedSpecialReward() && _holder.getBonusRewardIsAvailable() &&
                 (_maxNormalLevel <= _info.getCurrentLevel()))
        {
            totalRewards++;
        }
        else if (_maxNormalLevel <= _info.getCurrentLevel())
        {
            totalRewards++;
        }

        return totalRewards;
    }

    private int getPercent()
    {
        if (_info.getCurrentLevel() >= _holder.getMaxLevel())
        {
            return 100;
        }

        return (int)Math.Floor((double)_info.getCurrentEXP() / _holder.getXPForSpecifiedLevel(_info.getCurrentLevel()) *
                               100.0);
    }

    private void sendAvailableRewardsList(PacketBitWriter writer)
    {
        writer.WriteInt32(getTotalRewards()); // PkMissionLevelReward
        foreach (int level in _holder.getNormalRewards().Keys)
        {
            if (level <= _info.getCurrentLevel())
            {
                writer.WriteInt32(1); // Type
                writer.WriteInt32(level); // Level
                writer.WriteInt32(_collectedNormalRewards.Contains(level) ? 2 : 1); // State
            }
        }

        foreach (int level in _holder.getKeyRewards().Keys)
        {
            if (level <= _info.getCurrentLevel())
            {
                writer.WriteInt32(2); // Type
                writer.WriteInt32(level); // Level
                writer.WriteInt32(_collectedKeyRewards.Contains(level) ? 2 : 1); // State
            }
        }

        if (_holder.getBonusRewardByLevelUp() && _info.getCollectedSpecialReward() &&
            _holder.getBonusRewardIsAvailable() && (_maxNormalLevel <= _info.getCurrentLevel()))
        {
            writer.WriteInt32(3); // Type
            int sendLevel = 0;
            for (int level = _maxNormalLevel; level <= _holder.getMaxLevel(); level++)
            {
                if ((level <= _info.getCurrentLevel()) && !_collectedBonusRewards.Contains(level))
                {
                    sendLevel = level;
                    break;
                }
            }

            writer.WriteInt32(sendLevel == 0 ? _holder.getMaxLevel() : sendLevel); // Level
            writer.WriteInt32(2); // State
        }
        else if (_info.getCollectedSpecialReward() && _holder.getBonusRewardIsAvailable() &&
                 (_maxNormalLevel <= _info.getCurrentLevel()))
        {
            writer.WriteInt32(3); // Type
            writer.WriteInt32(_holder.getMaxLevel()); // Level
            writer.WriteInt32(2); // State
        }
        else if (_maxNormalLevel <= _info.getCurrentLevel())
        {
            writer.WriteInt32(3); // Type
            writer.WriteInt32(_holder.getMaxLevel()); // Level
            writer.WriteInt32(!_info.getCollectedSpecialReward()); // State
        }
    }
}