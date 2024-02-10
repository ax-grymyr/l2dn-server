using L2Dn.GameServer.Enums;
using L2Dn.GameServer.Utilities;
using NLog;

namespace L2Dn.GameServer.Data.Xml;

/**
 * Loads the category data with Class or NPC IDs.
 * @author NosBit, xban1x
 */
public class CategoryData
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
		parseDatapackFile("data/CategoryData.xml");
		LOGGER.Info(GetType().Name + ": Loaded " + _categories.size() + " categories.");
	}
	
	public void parseDocument(Document doc, File f)
	{
		for (Node node = doc.getFirstChild(); node != null; node = node.getNextSibling())
		{
			if ("list".equalsIgnoreCase(node.getNodeName()))
			{
				for (Node list_node = node.getFirstChild(); list_node != null; list_node = list_node.getNextSibling())
				{
					if ("category".equalsIgnoreCase(list_node.getNodeName()))
					{
						NamedNodeMap attrs = list_node.getAttributes();
						CategoryType categoryType = CategoryType.findByName(attrs.getNamedItem("name").getNodeValue());
						if (categoryType == null)
						{
							LOGGER.Warn(GetType().Name + ": Can't find category by name: " + attrs.getNamedItem("name").getNodeValue());
							continue;
						}
						
						Set<int> ids = new();
						for (Node category_node = list_node.getFirstChild(); category_node != null; category_node = category_node.getNextSibling())
						{
							if ("id".equalsIgnoreCase(category_node.getNodeName()))
							{
								ids.add(int.Parse(category_node.getTextContent()));
							}
						}
						_categories.put(categoryType, ids);
					}
				}
			}
		}
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