using System.Xml.Linq;
using L2Dn.Extensions;
using L2Dn.GameServer.Db;
using L2Dn.GameServer.Enums;
using L2Dn.GameServer.Utilities;
using L2Dn.Utilities;
using NLog;

namespace L2Dn.GameServer.Data.Xml;

/**
 * Loads the category data with Class or NPC IDs.
 * @author NosBit, xban1x
 */
public class CategoryData: DataReaderBase
{
	private static readonly Logger LOGGER = LogManager.GetLogger(nameof(CategoryData));
	
	private readonly Map<CategoryType, Set<int>> _categories = new();
	
	protected CategoryData()
	{
		load();
	}
	
	public void load()
	{
		_categories.clear();
		
		XDocument document = LoadXmlDocument(DataFileLocation.Data, "CategoryData.xml");
		document.Elements("list").Elements("category").ForEach(loadElement);
		
		LOGGER.Info(GetType().Name + ": Loaded " + _categories.size() + " categories.");
	}

	private void loadElement(XElement element)
	{
		CategoryType categoryType = element.Attribute("name").GetEnum<CategoryType>();
		if (categoryType == null)
		{
			LOGGER.Warn(GetType().Name + ": Can't find category by name: " + element.Attribute("name")?.Value);
			return;
		}

		Set<int> ids = new();
		element.Elements("id").Select(x => (int)x).ForEach(x => ids.add(x));
		_categories.put(categoryType, ids);
	}

	/**
	 * Checks if ID is in category.
	 * @param type The category type
	 * @param id The id to be checked
	 * @return {@code true} if id is in category, {@code false} if id is not in category or category was not found
	 */
	public bool isInCategory(CategoryType type, int id)
	{
		Set<int> category = getCategoryByType(type);
		if (category == null)
		{
			LOGGER.Warn(GetType().Name + ": Can't find category type: " + type);
			return false;
		}
		return category.Contains(id);
	}

	/**
	 * Checks if ID is in category.
	 * @param type The category type
	 * @param id The id to be checked
	 * @return {@code true} if id is in category, {@code false} if id is not in category or category was not found
	 */
	public bool isInCategory(CategoryType type, CharacterClass id) => isInCategory(type, (int)id);
	
	/**
	 * Gets the category by category type.
	 * @param type The category type
	 * @return A {@code Set} containing all the IDs in category if category is found, {@code null} if category was not found
	 */
	public Set<int> getCategoryByType(CategoryType type)
	{
		return _categories.get(type);
	}
	
	public static CategoryData getInstance()
	{
		return SingletonHolder.INSTANCE;
	}
	
	private static class SingletonHolder
	{
		public static readonly CategoryData INSTANCE = new();
	}
}