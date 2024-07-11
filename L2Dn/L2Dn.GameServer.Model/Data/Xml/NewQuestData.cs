using System.Reflection.Metadata;
using System.Xml.Linq;
using L2Dn.Extensions;
using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Holders;
using L2Dn.GameServer.Model.Quests.NewQuestData;
using L2Dn.GameServer.Utilities;
using NLog;

namespace L2Dn.GameServer.Data.Xml;

/**
 * @author Magik
 */
public class NewQuestData: DataReaderBase
{
	private static readonly Logger LOGGER = LogManager.GetLogger(nameof(NewQuestData));
	
	private readonly Map<int, NewQuest> _newQuestData = new();
	
	protected NewQuestData()
	{
		load();
	}
	
	public void load()
	{
		_newQuestData.Clear();
		
		XDocument document = LoadXmlDocument(DataFileLocation.Data, "NewQuestData.xml");
		document.Elements("list").Elements("quest").ForEach(parseElement);
		
		LOGGER.Info(GetType().Name + ": Loaded " + _newQuestData.size() + " new quest data.");
	}
	
	private void parseElement(XElement element)
	{
		NewQuest holder = new NewQuest(element);
		_newQuestData.put(holder.getId(), holder);
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