using L2Dn.GameServer.Model.Holders;
using L2Dn.GameServer.Utilities;
using NLog;

namespace L2Dn.GameServer.InstanceManagers.Events;

/**
 * @author Index
 */
public class LetterCollectorManager
{
	protected static readonly Logger LOGGER = LogManager.GetLogger(nameof(LetterCollectorManager));

	private readonly Map<int, LetterCollectorRewardHolder> _rewards = new();
	private readonly Map<int, List<ItemHolder>> _words = new();
	private readonly Map<string, int> _letter = new();
	private readonly Map<int, bool> _needToSumAllChance = new();

	private int _minLevel = 1;
	private int _maxLevel = Config.PLAYER_MAXIMUM_LEVEL;

	protected LetterCollectorManager()
	{
	}

	public void init()
	{
		LOGGER.Info(GetType().Name +": Loaded " + _rewards.Count + " words.");
		LOGGER.Info(GetType().Name +": Loaded " + _letter.Count + " letters.");
	}

	public int getMinLevel()
	{
		return _minLevel;
	}

	public void setMinLevel(int minLevel)
	{
		_minLevel = minLevel;
	}

	public int getMaxLevel()
	{
		return _maxLevel;
	}

	public void setMaxLevel(int maxLevel)
	{
		if (maxLevel < 1)
		{
			_maxLevel = Config.PLAYER_MAXIMUM_LEVEL;
		}
		else
		{
			_maxLevel = maxLevel;
		}
	}

	public LetterCollectorRewardHolder? getRewards(int id)
	{
		return _rewards.get(id);
	}

	public List<ItemHolder>? getWord(int id)
	{
		return _words.get(id);
	}

	public void setRewards(Map<int, LetterCollectorRewardHolder> rewards)
	{
		_rewards.putAll(rewards);
	}

	public void setWords(Map<int, List<ItemHolder>> words)
	{
		_words.putAll(words);
	}

	public void addRewards(int id, LetterCollectorRewardHolder rewards)
	{
		_rewards.put(id, rewards);
	}

	public void addWords(int id, List<ItemHolder> words)
	{
		_words.put(id, words);
	}

	public void resetField()
	{
		_minLevel = 1;
		_rewards.Clear();
		_words.Clear();
		_needToSumAllChance.Clear();
	}

	public void setLetters(Map<string, int> letters)
	{
		_letter.putAll(letters);
	}

	public Map<string, int> getLetters()
	{
		return _letter;
	}

	public void setNeedToSumAllChance(int id, bool needToSumAllChance)
	{
		_needToSumAllChance.put(id, needToSumAllChance);
	}

	public bool getNeedToSumAllChance(int id)
	{
		return _needToSumAllChance.get(id);
	}

	public class LetterCollectorRewardHolder
	{
		List<ItemChanceHolder> _rewards;
		double _chance;

		public LetterCollectorRewardHolder(List<ItemChanceHolder> rewards, double chance)
		{
			_rewards = rewards;
			_chance = chance;
		}

		public List<ItemChanceHolder> getRewards()
		{
			return _rewards;
		}

		public double getChance()
		{
			return _chance;
		}
	}

	public static LetterCollectorManager getInstance()
	{
		return SingletonHolder.INSTANCE;
	}

	private static class SingletonHolder
	{
		public static readonly LetterCollectorManager INSTANCE = new LetterCollectorManager();
	}
}