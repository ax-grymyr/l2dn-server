using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Holders;
using L2Dn.GameServer.Utilities;
using NLog;

namespace L2Dn.GameServer.Data.Xml;

/**
 * @author Sdw
 */
public class LuckyGameData
{
	private static readonly Logger LOGGER = LogManager.GetLogger(nameof(LuckyGameData));
	private readonly Map<int, LuckyGameDataHolder> _luckyGame = new();
	private readonly AtomicInteger _serverPlay = new AtomicInteger();
	
	protected LuckyGameData()
	{
		load();
	}
	
	public void load()
	{
		_luckyGame.clear();
		parseDatapackFile("data/LuckyGameData.xml");
		LOGGER.Info(GetType().Name + ": Loaded " + _luckyGame.size() + " lucky game data.");
	}
	
	public void parseDocument(Document doc, File f)
	{
		forEach(doc, "list", listNode => forEach(listNode, "luckygame", rewardNode =>
		{
			LuckyGameDataHolder holder = new LuckyGameDataHolder(new StatSet(parseAttributes(rewardNode)));
			
			forEach(rewardNode, "common_reward", commonRewardNode => forEach(commonRewardNode, "item", itemNode =>
			{
				StatSet stats = new StatSet(parseAttributes(itemNode));
				holder.addCommonReward(new ItemChanceHolder(stats.getInt("id"), stats.getDouble("chance"), stats.getLong("count")));
			}));
			
			forEach(rewardNode, "unique_reward", uniqueRewardNode => forEach(uniqueRewardNode, "item", itemNode =>
			{
				holder.addUniqueReward(new ItemPointHolder(new StatSet(parseAttributes(itemNode))));
			}));
			
			forEach(rewardNode, "modify_reward", uniqueRewardNode =>
			{
				holder.setMinModifyRewardGame(parseInteger(uniqueRewardNode.getAttributes(), "min_game"));
				holder.setMaxModifyRewardGame(parseInteger(uniqueRewardNode.getAttributes(), "max_game"));
				forEach(uniqueRewardNode, "item", itemNode =>
				{
					StatSet stats = new StatSet(parseAttributes(itemNode));
					holder.addModifyReward(new ItemChanceHolder(stats.getInt("id"), stats.getDouble("chance"), stats.getLong("count")));
				});
			});
			
			_luckyGame.put(parseInteger(rewardNode.getAttributes(), "index"), holder);
		}));
	}
	
	public int getLuckyGameCount()
	{
		return _luckyGame.size();
	}
	
	public LuckyGameDataHolder getLuckyGameDataByIndex(int index)
	{
		return _luckyGame.get(index);
	}
	
	public int increaseGame()
	{
		return _serverPlay.incrementAndGet();
	}
	
	public int getServerPlay()
	{
		return _serverPlay.get();
	}
	
	public static LuckyGameData getInstance()
	{
		return SingletonHolder.INSTANCE;
	}
	
	private static class SingletonHolder
	{
		public static readonly LuckyGameData INSTANCE = new();
	}
}