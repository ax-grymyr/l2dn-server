using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Holders;
using L2Dn.GameServer.Utilities;
using NLog;

namespace L2Dn.GameServer.Data.Xml;

/**
 * @author Index
 */
public class MissionLevel
{
	private static readonly Logger LOGGER = LogManager.GetLogger(nameof(MissionLevel));
	
	private readonly Map<int, MissionLevelHolder> _template = new();
	private int _currentSeason;
	
	protected MissionLevel()
	{
		load();
	}
	
	public void load()
	{
		_template.clear();
		parseDatapackFile("data/MissionLevel.xml");
		if (_currentSeason > 0)
		{
			LOGGER.Info(GetType().Name + ": Loaded " + _template.size() + " seasons.");
		}
		else
		{
			_template.clear();
		}
	}
	
	public void parseDocument(Document doc, File f)
	{
		forEach(doc, "list", listNode =>
		{
			forEach(listNode, "current", current => _currentSeason = parseInteger(current.getAttributes(), "season"));
			forEach(listNode, "missionLevel", missionNode =>
			{
				StatSet missionSet = new StatSet(parseAttributes(missionNode));
				AtomicInteger season = new AtomicInteger(missionSet.getInt("season"));
				AtomicInteger maxLevel = new AtomicInteger(missionSet.getInt("maxLevel"));
				AtomicBoolean bonusRewardIsAvailable = new AtomicBoolean(missionSet.getBoolean("bonusRewardIsAvailable"));
				AtomicBoolean bonusRewardByLevelUp = new AtomicBoolean(missionSet.getBoolean("bonusRewardByLevelUP"));
				AtomicReference<Map<int, ItemHolder>> keyReward = new AtomicReference<>(new());
				AtomicReference<Map<int, ItemHolder>> normalReward = new AtomicReference<>(new());
				AtomicReference<Map<int, int>> xpForLevel = new AtomicReference<>(new());
				AtomicReference<ItemHolder> specialReward = new AtomicReference<>();
				AtomicReference<ItemHolder> bonusReward = new AtomicReference<>();
				forEach(missionNode, "expTable", expListNode => forEach(expListNode, "exp", expNode =>
				{
					StatSet expSet = new StatSet(parseAttributes(expNode));
					xpForLevel.get().put(expSet.getInt("level"), expSet.getInt("amount"));
				}));
				forEach(missionNode, "baseRewards", baseRewardsNode => forEach(baseRewardsNode, "baseReward", rewards =>
				{
					StatSet rewardsSet = new StatSet(parseAttributes(rewards));
					normalReward.get().put(rewardsSet.getInt("level"), new ItemHolder(rewardsSet.getInt("itemId"), rewardsSet.getLong("itemCount")));
				}));
				forEach(missionNode, "keyRewards", keyRewardsNode => forEach(keyRewardsNode, "keyReward", rewards =>
				{
					StatSet rewardsSet = new StatSet(parseAttributes(rewards));
					keyReward.get().put(rewardsSet.getInt("level"), new ItemHolder(rewardsSet.getInt("itemId"), rewardsSet.getLong("itemCount")));
				}));
				forEach(missionNode, "specialReward", specialRewardNode =>
				{
					StatSet specialRewardSet = new StatSet(parseAttributes(specialRewardNode));
					specialReward.set(new ItemHolder(specialRewardSet.getInt("itemId"), specialRewardSet.getLong("itemCount")));
				});
				forEach(missionNode, "bonusReward", bonusRewardNode =>
				{
					StatSet bonusRewardSet = new StatSet(parseAttributes(bonusRewardNode));
					bonusReward.set(new ItemHolder(bonusRewardSet.getInt("itemId"), bonusRewardSet.getLong("itemCount")));
				});
				int bonusLevel = normalReward.get().keySet().stream().max(int::compare).orElse(maxLevel.get());
				if (bonusLevel == maxLevel.get())
				{
					bonusLevel = bonusLevel - 1;
				}
				_template.put(season.get(), new MissionLevelHolder(maxLevel.get(), bonusLevel + 1, xpForLevel.get(), normalReward.get(), keyReward.get(), specialReward.get(), bonusReward.get(), bonusRewardByLevelUp.get(), bonusRewardIsAvailable.get()));
			});
		});
	}
	
	public int getCurrentSeason()
	{
		return _currentSeason;
	}
	
	public MissionLevelHolder getMissionBySeason(int season)
	{
		return _template.getOrDefault(season, null);
	}
	
	public static MissionLevel getInstance()
	{
		return SingletonHolder.INSTANCE;
	}
	
	private static class SingletonHolder
	{
		public static readonly MissionLevel INSTANCE = new();
	}
}