using System.Text;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Variables;

namespace L2Dn.GameServer.Model.Holders;

/**
 * @author Index
 */
public class MissionLevelPlayerDataHolder
{
	private int _currentLevel = 0;
	private int _currentEXP = 0;
	private readonly List<int> _collectedNormalRewards = new();
	private readonly List<int> _collectedKeyRewards = new();
	private bool _collectedSpecialReward = false;
	private bool _collectedBonusReward = false;

	/**
	 * @implNote used only for missions where on bonus_reward_by_level_up;
	 * @apiNote store levels of taken bonus reward. If last reward be on 20 on 20, 21, 22... you will be get bonus reward;
	 */
	private readonly List<int> _listOfCollectedBonusRewards = new();

	public MissionLevelPlayerDataHolder()
	{
	}

	public MissionLevelPlayerDataHolder(String variable)
	{
		foreach (String data in variable.Split(";"))
		{
			List<String> values = data.Split(":").ToList();
			String key = values[0];
			values.RemoveAt(0);
			if (key.Equals("CurrentLevel"))
			{
				_currentLevel = int.Parse(values[0]);
				continue;
			}

			if (key.Equals("LevelXP"))
			{
				_currentEXP = int.Parse(values[0]);
				continue;
			}

			if (key.Equals("SpecialReward"))
			{
				_collectedSpecialReward = bool.Parse(values[0]);
				continue;
			}

			if (key.Equals("BonusReward"))
			{
				_collectedBonusReward = bool.Parse(values[0]);
				if (_collectedBonusReward && MissionLevel.getInstance()
					    .getMissionBySeason(MissionLevel.getInstance().getCurrentSeason()).getBonusRewardByLevelUp())
				{
					_collectedBonusReward = false;
				}

				continue;
			}

			List<int> valuesData = new();
			String[] missions = values.Count == 0 ? Array.Empty<string>() : values[0].Split(",");
			foreach (String mission in missions)
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
				_listOfCollectedBonusRewards.AddRange(valuesData);
				if (!_collectedBonusReward && _listOfCollectedBonusRewards.Count != 0 && !MissionLevel.getInstance()
					    .getMissionBySeason(MissionLevel.getInstance().getCurrentSeason()).getBonusRewardByLevelUp())
				{
					_collectedBonusReward = true;
				}
			}
		}
	}

	public String getVariablesFromInfo()
	{
		StringBuilder sb = new StringBuilder();
		// CurrentLevel:5;LevelXP:10;ListOfBaseRewards:2,19,20;ListOfKeyRewards:;SpecialRewards:;BonusRewards:;ListOfBonusRewards:;
		sb.Append("CurrentLevel").Append(":").Append(_currentLevel).Append(";");
		sb.Append("LevelXP").Append(":").Append(_currentEXP).Append(";");
		sb.Append("ListOfNormalRewards").Append(":");
		sb.Append(getStringFromList(_collectedNormalRewards));
		sb.Append(";");
		sb.Append("ListOfKeyRewards").Append(":");
		sb.Append(getStringFromList(_collectedKeyRewards));
		sb.Append(";");
		sb.Append("SpecialReward").Append(":");
		sb.Append(_collectedSpecialReward);
		sb.Append(";");
		sb.Append("BonusReward").Append(":");
		sb.Append(_collectedBonusReward);
		sb.Append(";");
		sb.Append("ListOfBonusRewards").Append(":");
		sb.Append(getStringFromList(_listOfCollectedBonusRewards));
		sb.Append(";");
		return sb.ToString();
	}

	private String getStringFromList(List<int> list)
	{
		return string.Join(",", list);
	}

	public void storeInfoInVariable(Player player)
	{
		player.getVariables()
			.set(PlayerVariables.MISSION_LEVEL_PROGRESS + MissionLevel.getInstance().getCurrentSeason(),
				getVariablesFromInfo());
	}

	public void calculateEXP(int exp)
	{
		MissionLevelHolder holder = MissionLevel.getInstance()
			.getMissionBySeason(MissionLevel.getInstance().getCurrentSeason());
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
		return _currentEXP;
	}

	public void setCurrentEXP(int currentEXP)
	{
		_currentEXP = currentEXP;
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