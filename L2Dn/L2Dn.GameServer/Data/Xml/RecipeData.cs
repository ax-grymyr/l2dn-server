using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Holders;
using L2Dn.GameServer.Utilities;
using NLog;

namespace L2Dn.GameServer.Data.Xml;

/**
 * The Class RecipeData.
 * @author Zoey76
 */
public class RecipeData
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
		parseDatapackFile("data/Recipes.xml");
		LOGGER.Info(GetType().Name + ": Loaded " + _recipes.size() + " recipes.");
	}
	
	public void parseDocument(Document doc, File f)
	{
		// TODO: Cleanup checks enforced by XSD.
		List<RecipeHolder> recipePartList = new();
		List<RecipeStatHolder> recipeStatUseList = new();
		List<RecipeStatHolder> recipeAltStatChangeList = new();
		for (Node n = doc.getFirstChild(); n != null; n = n.getNextSibling())
		{
			if ("list".equalsIgnoreCase(n.getNodeName()))
			{
				RECIPES_FILE: for (Node d = n.getFirstChild(); d != null; d = d.getNextSibling())
				{
					if ("item".equalsIgnoreCase(d.getNodeName()))
					{
						recipePartList.Clear();
						recipeStatUseList.Clear();
						recipeAltStatChangeList.Clear();
						NamedNodeMap attrs = d.getAttributes();
						Node att;
						int id = -1;
						bool haveRare = false;
						StatSet set = new StatSet();
						
						att = attrs.getNamedItem("id");
						if (att == null)
						{
							LOGGER.Error(GetType().Name + ": Missing id for recipe item, skipping");
							continue;
						}
						id = int.Parse(att.getNodeValue());
						set.set("id", id);
						
						att = attrs.getNamedItem("recipeId");
						if (att == null)
						{
							LOGGER.Error(GetType().Name + ": Missing recipeId for recipe item id: " + id + ", skipping");
							continue;
						}
						set.set("recipeId", int.Parse(att.getNodeValue()));
						
						att = attrs.getNamedItem("name");
						if (att == null)
						{
							LOGGER.Error(GetType().Name + ": Missing name for recipe item id: " + id + ", skipping");
							continue;
						}
						set.set("recipeName", att.getNodeValue());
						
						att = attrs.getNamedItem("craftLevel");
						if (att == null)
						{
							LOGGER.Error(GetType().Name + ": Missing level for recipe item id: " + id + ", skipping");
							continue;
						}
						set.set("craftLevel", int.Parse(att.getNodeValue()));
						
						att = attrs.getNamedItem("type");
						if (att == null)
						{
							LOGGER.Error(GetType().Name + ": Missing type for recipe item id: " + id + ", skipping");
							continue;
						}
						set.set("isDwarvenRecipe", att.getNodeValue().equalsIgnoreCase("dwarven"));
						
						att = attrs.getNamedItem("successRate");
						if (att == null)
						{
							LOGGER.Error(GetType().Name + ": Missing successRate for recipe item id: " + id + ", skipping");
							continue;
						}
						set.set("successRate", int.Parse(att.getNodeValue()));
						
						for (Node c = d.getFirstChild(); c != null; c = c.getNextSibling())
						{
							if ("statUse".equalsIgnoreCase(c.getNodeName()))
							{
								String statName = c.getAttributes().getNamedItem("name").getNodeValue();
								int value = int.Parse(c.getAttributes().getNamedItem("value").getNodeValue());
								try
								{
									recipeStatUseList.add(new RecipeStatHolder(statName, value));
								}
								catch (Exception e)
								{
									LOGGER.Error(GetType().Name + ": Error in StatUse parameter for recipe item id: " + id + ", skipping");
									continue RECIPES_FILE;
								}
							}
							else if ("altStatChange".equalsIgnoreCase(c.getNodeName()))
							{
								String statName = c.getAttributes().getNamedItem("name").getNodeValue();
								int value = int.Parse(c.getAttributes().getNamedItem("value").getNodeValue());
								try
								{
									recipeAltStatChangeList.add(new RecipeStatHolder(statName, value));
								}
								catch (Exception e)
								{
									LOGGER.Error(GetType().Name + ": Error in AltStatChange parameter for recipe item id: " + id + ", skipping");
									continue RECIPES_FILE;
								}
							}
							else if ("ingredient".equalsIgnoreCase(c.getNodeName()))
							{
								int ingId = int.Parse(c.getAttributes().getNamedItem("id").getNodeValue());
								int ingCount = int.Parse(c.getAttributes().getNamedItem("count").getNodeValue());
								recipePartList.add(new RecipeHolder(ingId, ingCount));
							}
							else if ("production".equalsIgnoreCase(c.getNodeName()))
							{
								set.set("itemId", int.Parse(c.getAttributes().getNamedItem("id").getNodeValue()));
								set.set("count", int.Parse(c.getAttributes().getNamedItem("count").getNodeValue()));
							}
							else if ("productionRare".equalsIgnoreCase(c.getNodeName()))
							{
								set.set("rareItemId", int.Parse(c.getAttributes().getNamedItem("id").getNodeValue()));
								set.set("rareCount", int.Parse(c.getAttributes().getNamedItem("count").getNodeValue()));
								set.set("rarity", int.Parse(c.getAttributes().getNamedItem("rarity").getNodeValue()));
								haveRare = true;
							}
						}
						
						RecipeList recipeList = new RecipeList(set, haveRare);
						foreach (RecipeHolder recipePart in recipePartList)
						{
							recipeList.addRecipe(recipePart);
						}
						foreach (RecipeStatHolder recipeStatUse in recipeStatUseList)
						{
							recipeList.addStatUse(recipeStatUse);
						}
						foreach (RecipeStatHolder recipeAltStatChange in recipeAltStatChangeList)
						{
							recipeList.addAltStatChange(recipeAltStatChange);
						}
						
						_recipes.put(id, recipeList);
					}
				}
			}
		}
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
		if ((recipeList == null) || (recipeList.getRecipes().Length == 0))
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