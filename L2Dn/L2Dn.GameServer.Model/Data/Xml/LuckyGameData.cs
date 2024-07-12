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
		_luckyGame.Clear();
		
		XDocument document = LoadXmlDocument(DataFileLocation.Data, "LuckyGameData.xml");
		document.Elements("list").Elements("luckygame").ForEach(parseElement);
		
		LOGGER.Info(GetType().Name + ": Loaded " + _luckyGame.Count + " lucky game data.");
	}
	
	private void parseElement(XElement element)
	{
		int index = element.GetAttributeValueAsInt32("index");
		int turningPoints = element.GetAttributeValueAsInt32("turning_point");
			LuckyGameDataHolder holder = new LuckyGameDataHolder(index, turningPoints);
			
			element.Elements("common_reward").Elements("item").ForEach(el =>
			{
				int id = el.GetAttributeValueAsInt32("id");
				double chance = el.GetAttributeValueAsDouble("chance");
				long count = el.GetAttributeValueAsInt64("count");
				holder.addCommonReward(new ItemChanceHolder(id, chance, count));
			});
			
			element.Elements("unique_reward").Elements("item").ForEach(el =>
			{
				int id = el.GetAttributeValueAsInt32("id");
				long count = el.GetAttributeValueAsInt64("count");
				int points = el.GetAttributeValueAsInt32("points");
				holder.addUniqueReward(new ItemPointHolder(id, count, points));
			});

			element.Elements("modify_reward").ForEach(el =>
			{
				int minGame = el.GetAttributeValueAsInt32("min_game");
				int maxGame = el.GetAttributeValueAsInt32("max_game");
				holder.setMinModifyRewardGame(minGame);
				holder.setMaxModifyRewardGame(maxGame);
				el.Elements("item").ForEach(e =>
				{
					int id = e.GetAttributeValueAsInt32("id");
					double chance = e.GetAttributeValueAsDouble("chance");
					long count = e.GetAttributeValueAsInt64("count");
					holder.addModifyReward(new ItemChanceHolder(id, chance, count));
				});
			});
			
			_luckyGame.put(index, holder);
	}
	
	public int getLuckyGameCount()
	{
		return _luckyGame.Count;
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