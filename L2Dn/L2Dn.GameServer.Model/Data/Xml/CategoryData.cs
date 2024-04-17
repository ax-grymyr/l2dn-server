using System.Collections.Frozen;
using L2Dn.GameServer.Enums;
using L2Dn.Model;
using L2Dn.Model.DataPack;
using NLog;

namespace L2Dn.GameServer.Data.Xml;

/**
 * Loads the category data with Class or NPC IDs.
 * @author NosBit, xban1x
 */
public class CategoryData: DataReaderBase
{
	private static readonly Logger _logger = LogManager.GetLogger(nameof(CategoryData));

	private FrozenDictionary<CategoryType, FrozenSet<int>> _categories =
		FrozenDictionary<CategoryType, FrozenSet<int>>.Empty;

	private CategoryData()
	{
		load();
	}

	public void load()
	{
		XmlCategoryData document = LoadXmlDocument<XmlCategoryData>(DataFileLocation.Data, "CategoryData.xml");
		_categories =
			(from category in document.Categories
				let categoryType = ParseCategoryType(category.Name)
				where categoryType != null
				select (CategoryType: categoryType.Value, Set: category.Ids.ToFrozenSet()))
			.ToFrozenDictionary(x => x.CategoryType, x => x.Set);

		_logger.Info(GetType().Name + ": Loaded " + _categories.Count + " categories.");
	}

	private static CategoryType? ParseCategoryType(string categoryTypeName)
	{
		if (Enum.TryParse(categoryTypeName, true, out CategoryType categoryType))
			return categoryType;

		_logger.Warn(typeof(CategoryData) + ": Can't find category by name: " + categoryTypeName);
		return null;
	}

	/**
	 * Checks if ID is in category.
	 * @param type The category type
	 * @param id The id to be checked
	 * @return {@code true} if id is in category, {@code false} if id is not in category or category was not found
	 */
	public bool isInCategory(CategoryType type, int id)
	{
		FrozenSet<int>? category = getCategoryByType(type);
		if (category == null)
		{
			_logger.Warn(GetType().Name + ": Can't find category type: " + type);
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
	public FrozenSet<int>? getCategoryByType(CategoryType type)
	{
		return _categories.GetValueOrDefault(type);
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