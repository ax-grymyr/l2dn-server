using System.Xml.Linq;
using L2Dn.Extensions;
using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Holders;
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
		_recipes.clear();
		
		XDocument document = LoadXmlDocument(DataFileLocation.Data, "Recipes.xml");
		document.Elements("list").Elements("item").ForEach(parseElement);
		
		LOGGER.Info(GetType().Name + ": Loaded " + _recipes.size() + " recipes.");
	}

	private void parseElement(XElement element)
	{
		// TODO: Cleanup checks enforced by XSD.
		List<RecipeHolder> recipePartList = new();
		List<RecipeStatHolder> recipeStatUseList = new();
		List<RecipeStatHolder> recipeAltStatChangeList = new();

		int id = element.Attribute("id").GetInt32();
		int recipeId = element.Attribute("recipeId").GetInt32();
		string name = element.Attribute("name").GetString();
		int craftLevel = element.Attribute("craftLevel").GetInt32();
		bool isDwarvenRecipe = element.Attribute("type").GetString() == "dwarven";
		int successRate = element.Attribute("successRate").GetInt32(100);

		element.Elements("statUse").ForEach(el =>
		{
			string statName = el.Attribute("name").GetString();
			int value = el.Attribute("value").GetInt32();
			recipeStatUseList.add(new RecipeStatHolder(statName, value));
		});

		element.Elements("altStatChange").ForEach(el =>
		{
			string statName = el.Attribute("name").GetString();
			int value = el.Attribute("value").GetInt32();
			recipeAltStatChangeList.add(new RecipeStatHolder(statName, value));
		});

		element.Elements("ingredient").ForEach(el =>
		{
			int ingId = el.Attribute("id").GetInt32();
			int ingCount = el.Attribute("count").GetInt32();
			recipePartList.add(new RecipeHolder(ingId, ingCount));
		});

		XElement prodElem = element.Elements("production").Single();
		int prodId = prodElem.Attribute("id").GetInt32();
		int prodCount = prodElem.Attribute("count").GetInt32();
		RecipeHolder production = new RecipeHolder(prodId, prodCount);

		RecipeRareHolder? rareProduction = null;
		XElement? prodRareElem = element.Elements("productionRare").SingleOrDefault();
		if (prodRareElem != null)
		{
			prodId = prodRareElem.Attribute("id").GetInt32();
			prodCount = prodRareElem.Attribute("count").GetInt32();
			int rarity = prodRareElem.Attribute("rarity").GetInt32();
			rareProduction = new RecipeRareHolder(prodId, prodCount, rarity);
		}

		RecipeList recipeList = new RecipeList(id, recipeId, recipeId, name, successRate,
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
	public RecipeList getRecipeList(int listId)
	{
		return _recipes.get(listId);
	}
	
	/**
	 * Gets the recipe by item id.
	 * @param itemId the item id
	 * @return the recipe by item id
	 */
	public RecipeList getRecipeByItemId(int itemId)
	{
		foreach (RecipeList find in _recipes.values())
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
		int[] idList = new int[_recipes.size()];
		int i = 0;
		foreach (RecipeList rec in _recipes.values())
		{
			idList[i++] = rec.getRecipeId();
		}
		return idList;
	}
	
	/**
	 * Gets the valid recipe list.
	 * @param player the player
	 * @param id the recipe list id
	 * @return the valid recipe list
	 */
	public RecipeList getValidRecipeList(Player player, int id)
	{
		RecipeList recipeList = _recipes.get(id);
		if ((recipeList == null) || (recipeList.getRecipes().Count == 0))
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