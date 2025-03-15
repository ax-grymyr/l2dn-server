using System.Xml.Linq;
using L2Dn.Extensions;
using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Holders;
using L2Dn.GameServer.StaticData;
using L2Dn.GameServer.Utilities;
using L2Dn.Utilities;
using NLog;

namespace L2Dn.GameServer.Data.Xml;

/**
 * The Class RecipeData.
 * @author Zoey76
 */
public class RecipeData: DataReaderBase
{
	private static readonly Logger LOGGER = LogManager.GetLogger(nameof(RecipeData));

	private readonly Map<int, RecipeList> _recipes = new();

	/**
	 * Instantiates a new recipe data.
	 */
	protected RecipeData()
	{
		load();
	}

	public void load()
	{
		_recipes.Clear();

		XDocument document = LoadXmlDocument(DataFileLocation.Data, "Recipes.xml");
		document.Elements("list").Elements("item").ForEach(parseElement);

		LOGGER.Info(GetType().Name + ": Loaded " + _recipes.Count + " recipes.");
	}

	private void parseElement(XElement element)
	{
		// TODO: Cleanup checks enforced by XSD.
		List<RecipeHolder> recipePartList = new();
		List<RecipeStatHolder> recipeStatUseList = new();
		List<RecipeStatHolder> recipeAltStatChangeList = new();

		int id = element.GetAttributeValueAsInt32("id");
		int recipeId = element.GetAttributeValueAsInt32("recipeId");
		string name = element.GetAttributeValueAsString("name");
		int craftLevel = element.GetAttributeValueAsInt32("craftLevel");
		bool isDwarvenRecipe = element.GetAttributeValueAsString("type") == "dwarven";
		int successRate = element.Attribute("successRate").GetInt32(100);

		element.Elements("statUse").ForEach(el =>
		{
			string statName = el.GetAttributeValueAsString("name");
			int value = el.GetAttributeValueAsInt32("value");
			recipeStatUseList.Add(new RecipeStatHolder(statName, value));
		});

		element.Elements("altStatChange").ForEach(el =>
		{
			string statName = el.GetAttributeValueAsString("name");
			int value = el.GetAttributeValueAsInt32("value");
			recipeAltStatChangeList.Add(new RecipeStatHolder(statName, value));
		});

		element.Elements("ingredient").ForEach(el =>
		{
			int ingId = el.GetAttributeValueAsInt32("id");
			int ingCount = el.GetAttributeValueAsInt32("count");
			recipePartList.Add(new RecipeHolder(ingId, ingCount));
		});

		XElement prodElem = element.Elements("production").Single();
		int prodId = prodElem.GetAttributeValueAsInt32("id");
		int prodCount = prodElem.GetAttributeValueAsInt32("count");
		RecipeHolder production = new RecipeHolder(prodId, prodCount);

		RecipeRareHolder? rareProduction = null;
		XElement? prodRareElem = element.Elements("productionRare").SingleOrDefault();
		if (prodRareElem != null)
		{
			prodId = prodRareElem.GetAttributeValueAsInt32("id");
			prodCount = prodRareElem.GetAttributeValueAsInt32("count");
			int rarity = prodRareElem.GetAttributeValueAsInt32("rarity");
			rareProduction = new RecipeRareHolder(prodId, prodCount, rarity);
		}

		RecipeList recipeList = new RecipeList(id, craftLevel, recipeId, name, successRate,
			isDwarvenRecipe, production, rareProduction);

		foreach (RecipeHolder recipePart in recipePartList)
			recipeList.addRecipe(recipePart);

		foreach (RecipeStatHolder recipeStatUse in recipeStatUseList)
			recipeList.addStatUse(recipeStatUse);

		foreach (RecipeStatHolder recipeAltStatChange in recipeAltStatChangeList)
			recipeList.addAltStatChange(recipeAltStatChange);

		_recipes.put(id, recipeList);
	}

	/**
	 * Gets the recipe list.
	 * @param listId the list id
	 * @return the recipe list
	 */
	public RecipeList? getRecipeList(int listId)
	{
		return _recipes.get(listId);
	}

	/**
	 * Gets the recipe by item id.
	 * @param itemId the item id
	 * @return the recipe by item id
	 */
	public RecipeList? getRecipeByItemId(int itemId)
	{
		foreach (RecipeList find in _recipes.Values)
		{
			if (find.getRecipeId() == itemId)
			{
				return find;
			}
		}
		return null;
	}

	/**
	 * Gets the all item ids.
	 * @return the all item ids
	 */
	public int[] getAllItemIds()
	{
		return _recipes.Values.Select(x => x.getRecipeId()).ToArray();
	}

	/**
	 * Gets the valid recipe list.
	 * @param player the player
	 * @param id the recipe list id
	 * @return the valid recipe list
	 */
	public RecipeList? getValidRecipeList(Player player, int id)
	{
		RecipeList? recipeList = _recipes.get(id);
		if (recipeList == null || recipeList.getRecipes().Count == 0)
		{
			player.sendMessage("No recipe for: " + id);
			player.setCrafting(false);
			return null;
		}
		return recipeList;
	}

	/**
	 * Gets the single instance of RecipeData.
	 * @return single instance of RecipeData
	 */
	public static RecipeData getInstance()
	{
		return SingletonHolder.INSTANCE;
	}

	/**
	 * The Class SingletonHolder.
	 */
	private static class SingletonHolder
	{
		public static readonly RecipeData INSTANCE = new();
	}
}