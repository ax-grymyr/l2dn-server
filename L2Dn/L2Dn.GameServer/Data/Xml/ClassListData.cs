using System.Xml.Linq;
using L2Dn.Extensions;
using L2Dn.GameServer.Model.Holders;
using L2Dn.GameServer.Utilities;
using L2Dn.Model;
using L2Dn.Utilities;
using NLog;

namespace L2Dn.GameServer.Data.Xml;

/**
 * Loads the the list of classes and it's info.
 * @author Zoey76
 */
public class ClassListData: DataReaderBase
{
	private static readonly Logger LOGGER = LogManager.GetLogger(nameof(ClassListData));
	
	private readonly Map<CharacterClass, ClassInfoHolder> _classData = new();
	
	/**
	 * Instantiates a new class list data.
	 */
	protected ClassListData()
	{
		load();
	}
	
	public void load()
	{
		_classData.clear();

		XDocument document = LoadXmlDocument(DataFileLocation.Data, "stats/chars/classList.xml");
		document.Elements("list").Elements("class").ForEach(loadElement);
		
		LOGGER.Info(GetType().Name + ": Loaded " + _classData.size() + " class data.");
	}
	
	private void loadElement(XElement element)
	{
		CharacterClass classId = (CharacterClass)element.GetAttributeValueAsInt32("classId");
		string className = element.GetAttributeValueAsString("classId");

		int parentId = element.Attribute("parentClassId").GetInt32(-1);
		CharacterClass? parentClassId = parentId < 0 ? null : (CharacterClass)parentId;
		_classData.put(classId, new ClassInfoHolder(classId, className, parentClassId));
	}
	
	/**
	 * Gets the class list.
	 * @return the complete class list.
	 */
	public Map<CharacterClass, ClassInfoHolder> getClassList()
	{
		return _classData;
	}
	
	/**
	 * Gets the class info.
	 * @param classId the class Id as integer.
	 * @return the class info related to the given {@code classId}.
	 */
	public ClassInfoHolder getClass(CharacterClass classId)
	{
		return _classData[classId];
	}
	
	/**
	 * Gets the single instance of ClassListData.
	 * @return single instance of ClassListData
	 */
	public static ClassListData getInstance()
	{
		return SingletonHolder.INSTANCE;
	}
	
	private static class SingletonHolder
	{
		public static readonly ClassListData INSTANCE = new();
	}
}