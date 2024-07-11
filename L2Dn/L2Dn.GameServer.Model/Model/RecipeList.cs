using L2Dn.GameServer.Model.Holders;
using L2Dn.GameServer.Utilities;

namespace L2Dn.GameServer.Model;

/**
 * This class describes a Recipe used by Dwarf to craft Item. All RecipeList are made of RecipeHolder (1 line of the recipe : Item-Quantity needed).
 */
public class RecipeList
{
	/** The table containing all RecipeHolder (1 line of the recipe : Item-Quantity needed) of the RecipeList */
	private List<RecipeHolder> _recipes = new();
	
	/** The table containing all RecipeStatHolder for the statUse parameter of the RecipeList */
	private List<RecipeStatHolder> _statUse = new();
	
	/** The table containing all RecipeStatHolder for the altStatChange parameter of the RecipeList */
	private List<RecipeStatHolder> _altStatChange = new();
	
	/** The Identifier of the Instance */
	private readonly int _id;
	
	/** The crafting level needed to use this RecipeList */
	private readonly int _level;
	
	/** The Identifier of the RecipeList */
	private readonly int _recipeId;
	
	/** The name of the RecipeList */
	private readonly string _recipeName;
	
	/** The crafting success rate when using the RecipeList */
	private readonly int _successRate;
	
	/** The Identifier and the quantity of the Item crafted with this RecipeList */
	private readonly RecipeHolder _production;
	
	/** The Identifier, the quantity and the chance of the Rare Item crafted with this RecipeList */
	private readonly RecipeRareHolder? _rareProduction;
		
	/** If this a common or a dwarven recipe */
	private readonly bool _isDwarvenRecipe;

	/**
	 * Constructor of RecipeList (create a new Recipe).
	 * @param set
	 * @param haveRare
	 */
	public RecipeList(int id, int craftLevel, int recipeId, string recipeName, int successRate, bool isDwarvenRecipe,
		RecipeHolder production, RecipeRareHolder? rareProduction)
	{
		_id = id;
		_level = craftLevel;
		_recipeId = recipeId;
		_recipeName = recipeName;
		_successRate = successRate;
		_production = production;
		_rareProduction = rareProduction;
		_isDwarvenRecipe = isDwarvenRecipe;
	}

	/**
	 * Add a RecipeHolder to the RecipeList (add a line Item-Quantity needed to the Recipe).
	 * @param recipe
	 */
	public void addRecipe(RecipeHolder recipe)
	{
		_recipes.Add(recipe);
	}
	
	/**
	 * Add a RecipeStatHolder of the statUse parameter to the RecipeList.
	 * @param statUse
	 */
	public void addStatUse(RecipeStatHolder statUse)
	{
		_statUse.Add(statUse);
	}
	
	/**
	 * Add a RecipeStatHolder of the altStatChange parameter to the RecipeList.
	 * @param statChange
	 */
	public void addAltStatChange(RecipeStatHolder statChange)
	{
		_altStatChange.Add(statChange);
	}
	
	/**
	 * @return the Identifier of the Instance.
	 */
	public int getId()
	{
		return _id;
	}
	
	/**
	 * @return the crafting level needed to use this RecipeList.
	 */
	public int getLevel()
	{
		return _level;
	}
	
	/**
	 * @return the Identifier of the RecipeList.
	 */
	public int getRecipeId()
	{
		return _recipeId;
	}
	
	/**
	 * @return the name of the RecipeList.
	 */
	public string getRecipeName()
	{
		return _recipeName;
	}
	
	/**
	 * @return the crafting success rate when using the RecipeList.
	 */
	public int getSuccessRate()
	{
		return _successRate;
	}
	
	/**
	 * @return the Identifier of the Item crafted with this RecipeList.
	 */
	public int getItemId()
	{
		return _production.getItemId();
	}
	
	/**
	 * @return the quantity of Item crafted when using this RecipeList.
	 */
	public int getCount()
	{
		return _production.getQuantity();
	}
	
	/**
	 * @return the Identifier of the Rare Item crafted with this RecipeList.
	 */
	public int getRareItemId()
	{
		return _rareProduction?.getItemId() ?? 0;
	}
	
	/**
	 * @return the quantity of Rare Item crafted when using this RecipeList.
	 */
	public int getRareCount()
	{
		return _rareProduction?.getQuantity() ?? 0;
	}
	
	/**
	 * @return the chance of Rare Item crafted when using this RecipeList.
	 */
	public int getRarity()
	{
		return _rareProduction?.getRarity() ?? 0;
	}
	
	/**
	 * @return {@code true} if this a Dwarven recipe or {@code false} if its a Common recipe
	 */
	public bool isDwarvenRecipe()
	{
		return _isDwarvenRecipe;
	}
	
	/**
	 * @return the table containing all RecipeHolder (1 line of the recipe : Item-Quantity needed) of the RecipeList.
	 */
	public List<RecipeHolder> getRecipes()
	{
		return _recipes;
	}
	
	/**
	 * @return the table containing all RecipeStatHolder of the statUse parameter of the RecipeList.
	 */
	public List<RecipeStatHolder> getStatUse()
	{
		return _statUse;
	}
	
	/**
	 * @return the table containing all RecipeStatHolder of the AltStatChange parameter of the RecipeList.
	 */
	public List<RecipeStatHolder> getAltStatChange()
	{
		return _altStatChange;
	}
}