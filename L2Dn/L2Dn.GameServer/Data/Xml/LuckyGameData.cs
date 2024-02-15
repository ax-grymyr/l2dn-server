using System.Xml.Linq;
using L2Dn.Extensions;
using L2Dn.GameServer.Model.Holders;
using L2Dn.GameServer.Utilities;
using L2Dn.Utilities;
using NLog;

namespace L2Dn.GameServer.Data.Xml;

/**
 * @author Sdw
 */
public class LuckyGameData: DataReaderBase
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
		
		XDocument document = LoadXmlDocument(DataFileLocation.Data, "LuckyGameData.xml");
		document.Elements("list").Elements("luckygame").ForEach(parseElement);
		
		LOGGER.Info(GetType().Name + ": Loaded " + _luckyGame.size() + " lucky game data.");
	}
	
	private void parseElement(XElement element)
	{
		int index = element.Attribute("index").GetInt32();
		int turningPoints = element.Attribute("turning_point").GetInt32();
			LuckyGameDataHolder holder = new LuckyGameDataHolder(index, turningPoints);
			
			element.Elements("common_reward").Elements("item").ForEach(el =>
			{
				int id = el.Attribute("id").GetInt32();
				double chance = el.Attribute("chance").GetDouble();
				long count = el.Attribute("count").GetInt64();
				holder.addCommonReward(new ItemChanceHolder(id, chance, count));
			});
			
			element.Elements("common_reward").Elements("item").ForEach(el =>
			{
				int id = el.Attribute("id").GetInt32();
				long count = el.Attribute("count").GetInt64();
				int points = el.Attribute("points").GetInt32();
				holder.addUniqueReward(new ItemPointHolder(id, count, points));
			});

			element.Elements("modify_reward").ForEach(el =>
			{
				int minGame = el.Attribute("min_game").GetInt32();
				int maxGame = el.Attribute("max_game").GetInt32();
				holder.setMinModifyRewardGame(minGame);
				holder.setMaxModifyRewardGame(maxGame);
				el.Elements("item").ForEach(e =>
				{
					int id = el.Attribute("id").GetInt32();
					double chance = el.Attribute("chance").GetDouble();
					long count = el.Attribute("count").GetInt64();
					holder.addModifyReward(new ItemChanceHolder(id, chance, count));
				});
			});
			
			_luckyGame.put(index, holder);
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