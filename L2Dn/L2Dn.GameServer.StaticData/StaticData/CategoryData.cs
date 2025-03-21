using System.Collections.Frozen;
using L2Dn.GameServer.Enums;
using L2Dn.GameServer.StaticData.Xml.Categories;
using L2Dn.GameServer.Utilities;
using NLog;

namespace L2Dn.GameServer.StaticData;

/// <summary>
/// Loads the category data with Class or NPC IDs.
/// </summary>
public sealed class CategoryData
{
    private static readonly Logger _logger = LogManager.GetLogger(nameof(CategoryData));

    private FrozenDictionary<CategoryType, FrozenSet<int>> _categories =
        FrozenDictionary<CategoryType, FrozenSet<int>>.Empty;

    private CategoryData()
    {
    }

    public static CategoryData Instance { get; } = new();

    public void Load()
    {
        XmlCategoryList document = XmlLoader.LoadXmlDocument<XmlCategoryList>("CategoryData.xml");

        _categories = document.Categories.
            Select(category => KeyValuePair.Create(category.Type, category.Ids.ToFrozenSet())).
            ToFrozenDictionary();

        _logger.Info($"{nameof(CategoryData)}: Loaded {_categories.Count} categories.");
    }

    /// <summary>
    /// Checks if ID is in category.
    /// </summary>
    public bool IsInCategory(CategoryType type, int id) => GetCategory(type).Contains(id);

    /// <summary>
    /// Checks if CharacterClass is in category.
    /// </summary>
    public bool IsInCategory(CategoryType type, CharacterClass id) => IsInCategory(type, (int)id);

    /// <summary>
    /// Gets the category by category type.
    /// </summary>
    public FrozenSet<int> GetCategory(CategoryType type)
    {
        if (_categories.TryGetValue(type, out FrozenSet<int>? category))
            return category;

        _logger.Warn($"{nameof(CategoryData)}: Can't find category type: {type}");
        return FrozenSet<int>.Empty;
    }
}