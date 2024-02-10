using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Holders;
using L2Dn.GameServer.Model.Quests.NewQuestData;
using L2Dn.GameServer.Utilities;
using NLog;

namespace L2Dn.GameServer.Data.Xml;

/**
 * @author Magik
 */
public class NewQuestData
{
	private static readonly Logger LOGGER = LogManager.GetLogger(nameof(NewQuestData));
	
	private readonly Map<int, NewQuest> _newQuestData = new();
	
	protected NewQuestData()
	{
		load();
	}
	
	public void load()
	{
		_newQuestData.clear();
		parseDatapackFile("data/NewQuestData.xml");
		
		LOGGER.Info(GetType().Name + ": Loaded " + _newQuestData.size() + " new quest data.");
	}
	
	public void parseDocument(Document doc, File f)
	{
		forEach(doc, "list", listNode => forEach(listNode, "quest", questNode =>
		{
			StatSet set = new StatSet(parseAttributes(questNode));
			forEach(questNode, "locations", locationsNode =>
			{
				forEach(locationsNode, "param", paramNode => set.set(parseString(paramNode.getAttributes(), "name"), paramNode.getTextContent()));
			});
			
			forEach(questNode, "conditions", conditionsNode =>
			{
				forEach(conditionsNode, "param", paramNode => set.set(parseString(paramNode.getAttributes(), "name"), paramNode.getTextContent()));
			});
			
			forEach(questNode, "rewards", rewardsNode =>
			{
				
				List<ItemHolder> rewardItems = new();
				forEach(rewardsNode, "items", itemsNode => forEach(itemsNode, "item", itemNode =>
				{
					int itemId = parseInteger(itemNode.getAttributes(), "id");
					int itemCount = parseInteger(itemNode.getAttributes(), "count");
					ItemHolder rewardItem = new ItemHolder(itemId, itemCount);
					rewardItems.add(rewardItem);
				}));
				
				set.set("rewardItems", rewardItems);
				forEach(rewardsNode, "param", paramNode => set.set(parseString(paramNode.getAttributes(), "name"), paramNode.getTextContent()));
			});
			
			forEach(questNode, "goals", goalsNode =>
			{
				forEach(goalsNode, "param", paramNode => set.set(parseString(paramNode.getAttributes(), "name"), paramNode.getTextContent()));
			});
			
			NewQuest holder = new NewQuest(set);
			_newQuestData.put(holder.getId(), holder);
		}));
	}
	
	public NewQuest getQuestById(int id)
	{
		return _newQuestData.get(id);
	}
	
	public ICollection<NewQuest> getQuests()
	{
		return _newQuestData.values();
	}
	
	/**
	 * Gets the single instance of NewQuestData.
	 * @return single instance of NewQuestData
	 */
	public static NewQuestData getInstance()
	{
		return NewQuestData.SingletonHolder.INSTANCE;
	}
	
	private static class SingletonHolder
	{
		public static readonly NewQuestData INSTANCE = new();
	}
}