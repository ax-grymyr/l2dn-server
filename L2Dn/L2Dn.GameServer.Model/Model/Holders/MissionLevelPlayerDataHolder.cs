using System.Text;
using L2Dn.GameServer.Data.Xml;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Variables;
using NLog;

namespace L2Dn.GameServer.Model.Holders;

/**
 * @author Index
 */
public class MissionLevelPlayerDataHolder
{
    private static readonly Logger _logger = LogManager.GetLogger(nameof(MissionLevelPlayerDataHolder));
	private int _currentLevel;
	private int _currentExp;
	private readonly List<int> _collectedNormalRewards = [];
	private readonly List<int> _collectedKeyRewards = [];
	private bool _collectedSpecialReward;
	private bool _collectedBonusReward;

	/**
	 * @implNote used only for missions where on bonus_reward_by_level_up;
	 * @apiNote store levels of taken bonus reward. If last reward be on 20 on 20, 21, 22... you will be get bonus reward;
	 */
	private readonly List<int> _listOfCollectedBonusRewards = [];

	public MissionLevelPlayerDataHolder()
	{
	}

	public MissionLevelPlayerDataHolder(string variable)
	{
		foreach (string data in variable.Split(";"))
		{
			List<string> values = data.Split(":").ToList();
			string key = values[0];
			values.RemoveAt(0);
			if (key.Equals("CurrentLevel"))
			{
				_currentLevel = int.Parse(values[0]);
				continue;
			}

			if (key.Equals("LevelXP"))
			{
				_currentExp = int.Parse(values[0]);
				continue;
			}

			if (key.Equals("SpecialReward"))
			{
				_collectedSpecialReward = bool.Parse(values[0]);
				continue;
			}

			if (key.Equals("BonusReward"))
            {
                MissionLevelHolder? missionLevelHolder = MissionLevel.getInstance().
                    getMissionBySeason(MissionLevel.getInstance().getCurrentSeason());

                if (missionLevelHolder is null)
                {
                    _logger.Error("No mission level data for current season");
                    _collectedBonusReward = false;
                    continue;
                }

				_collectedBonusReward = bool.Parse(values[0]);
				if (_collectedBonusReward && missionLevelHolder.getBonusRewardByLevelUp())
				{
					_collectedBonusReward = false;
				}

				continue;
			}

			List<int> valuesData = new();
			string[] missions = values.Count == 0 || string.IsNullOrEmpty(values[0]) ? Array.Empty<string>() : values[0].Split(",");
			foreach (string mission in missions)
			{
				valuesData.Add(int.Parse(mission));
			}

			if (key.Equals("ListOfNormalRewards"))
			{
				_collectedNormalRewards.AddRange(valuesData);
				continue;
			}

			if (key.Equals("ListOfKeyRewards"))
			{
				_collectedKeyRewards.AddRange(valuesData);
				continue;
			}

			if (key.Equals("ListOfBonusRewards"))
			{
                MissionLevelHolder? missionLevelHolder = MissionLevel.getInstance().
                    getMissionBySeason(MissionLevel.getInstance().getCurrentSeason());

                if (missionLevelHolder is null)
                {
                    _logger.Error("No mission level data for current season");
                    _collectedBonusReward = false;
                    continue;
                }

                _listOfCollectedBonusRewards.AddRange(valuesData);
				if (!_collectedBonusReward && _listOfCollectedBonusRewards.Count != 0 && !missionLevelHolder.getBonusRewardByLevelUp())
				{
					_collectedBonusReward = true;
				}
			}
		}
	}

	public string getVariablesFromInfo()
	{
		StringBuilder sb = new StringBuilder();
		// CurrentLevel:5;LevelXP:10;ListOfBaseRewards:2,19,20;ListOfKeyRewards:;SpecialRewards:;BonusRewards:;ListOfBonusRewards:;
		sb.Append("CurrentLevel").Append(':').Append(_currentLevel).Append(';');
		sb.Append("LevelXP").Append(':').Append(_currentExp).Append(';');
		sb.Append("ListOfNormalRewards").Append(':');
		sb.AppendJoin(',', _collectedNormalRewards);
		sb.Append(';');
		sb.Append("ListOfKeyRewards").Append(':');
		sb.AppendJoin(',', _collectedKeyRewards);
		sb.Append(';');
		sb.Append("SpecialReward").Append(':');
		sb.Append(_collectedSpecialReward);
		sb.Append(';');
		sb.Append("BonusReward").Append(':');
		sb.Append(_collectedBonusReward);
		sb.Append(';');
		sb.Append("ListOfBonusRewards").Append(':');
		sb.AppendJoin(',', _listOfCollectedBonusRewards);
		sb.Append(';');
		return sb.ToString();
	}

	public void storeInfoInVariable(Player player)
	{
		player.getVariables()
			.set(PlayerVariables.MISSION_LEVEL_PROGRESS + MissionLevel.getInstance().getCurrentSeason(),
				getVariablesFromInfo());
	}

	public void calculateEXP(int exp)
	{
		MissionLevelHolder? holder = MissionLevel.getInstance()
			.getMissionBySeason(MissionLevel.getInstance().getCurrentSeason());

        if (holder is null)
        {
            _logger.Error("No mission level data for current season");
            return;
        }

		if (getCurrentLevel() >= holder.getMaxLevel())
		{
			return;
		}

		int giveEXP = exp;
		while (true)
		{
			try
			{
				// char have 20 exp, for next level 25, you will give 10 exp = 25 - (20 + 10) = -5; 5 going to current EXP and char get level up ;)
				int takeEXP = holder.getXPForSpecifiedLevel(getCurrentLevel() + 1) - (getCurrentEXP() + giveEXP);
				if (takeEXP <= 0)
				{
					giveEXP = Math.Abs(takeEXP);
					setCurrentLevel(getCurrentLevel() + 1);
					setCurrentEXP(0);
				}
				else
				{
					setCurrentEXP(getCurrentEXP() + giveEXP);
					break;
				}
			}
			catch (NullReferenceException e)
			{
                _logger.Error(e);
				break;
			}
		}
	}

	public int getCurrentLevel()
	{
		return _currentLevel;
	}

	public void setCurrentLevel(int currentLevel)
	{
		_currentLevel = currentLevel;
	}

	public int getCurrentEXP()
	{
		return _currentExp;
	}

	public void setCurrentEXP(int currentEXP)
	{
		_currentExp = currentEXP;
	}

	public List<int> getCollectedNormalRewards()
	{
		return _collectedNormalRewards;
	}

	public void addToCollectedNormalRewards(int pos)
	{
		_collectedNormalRewards.Add(pos);
	}

	public List<int> getCollectedKeyRewards()
	{
		return _collectedKeyRewards;
	}

	public void addToCollectedKeyReward(int pos)
	{
		_collectedKeyRewards.Add(pos);
	}

	public bool getCollectedSpecialReward()
	{
		return _collectedSpecialReward;
	}

	public void setCollectedSpecialReward(bool collectedSpecialReward)
	{
		_collectedSpecialReward = collectedSpecialReward;
	}

	public bool getCollectedBonusReward()
	{
		return _collectedBonusReward;
	}

	public void setCollectedBonusReward(bool collectedBonusReward)
	{
		_collectedBonusReward = collectedBonusReward;
	}

	public List<int> getListOfCollectedBonusRewards()
	{
		return _listOfCollectedBonusRewards;
	}

	public void addToListOfCollectedBonusRewards(int pos)
	{
		_listOfCollectedBonusRewards.Add(pos);
	}
}