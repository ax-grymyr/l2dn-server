using L2Dn.GameServer.Model.Holders;

namespace L2Dn.GameServer.Model;

/**
 * This class describes a Recipe used by Dwarf to craft Item. All RecipeList are made of RecipeHolder (1 line of the recipe : Item-Quantity needed).
 */
public class RecipeList
{
	/** The table containing all RecipeHolder (1 line of the recipe : Item-Quantity needed) of the RecipeList */
	private RecipeHolder[] _recipes;
	
	/** The table containing all RecipeStatHolder for the statUse parameter of the RecipeList */
	private RecipeStatHolder[] _statUse;
	
	/** The table containing all RecipeStatHolder for the altStatChange parameter of the RecipeList */
	private RecipeStatHolder[] _altStatChange;
	
	/** The Identifier of the Instance */
	private readonly int _id;
	
	/** The crafting level needed to use this RecipeList */
	private readonly int _level;
	
	/** The Identifier of the RecipeList */
	private readonly int _recipeId;
	
	/** The name of the RecipeList */
	private readonly String _recipeName;
	
	/** The crafting success rate when using the RecipeList */
	private readonly int _successRate;
	
	/** The Identifier of the Item crafted with this RecipeList */
	private readonly int _itemId;
	
	/** The quantity of Item crafted when using this RecipeList */
	private readonly int _count;
	
	/** The Identifier of the Rare Item crafted with this RecipeList */
	private int _rareItemId;
	
	/** The quantity of Rare Item crafted when using this RecipeList */
	private int _rareCount;
	
	/** The chance of Rare Item crafted when using this RecipeList */
	private int _rarity;
	
	/** If this a common or a dwarven recipe */
	private readonly bool _isDwarvenRecipe;
	
	/**
	 * Constructor of RecipeList (create a new Recipe).
	 * @param set
	 * @param haveRare
	 */
	public RecipeList(StatSet set, bool haveRare)
	{
		_recipes = new RecipeHolder[0];
		_statUse = new RecipeStatHolder[0];
		_altStatChange = new RecipeStatHolder[0];
		_id = set.getInt("id");
		_level = set.getInt("craftLevel");
		_recipeId = set.getInt("recipeId");
		_recipeName = set.getString("recipeName");
		_successRate = set.getInt("successRate");
		_itemId = set.getInt("itemId");
		_count = set.getInt("count");
		if (haveRare)
		{
			_rareItemId = set.getInt("rareItemId");
			_rareCount = set.getInt("rareCount");
			_rarity = set.getInt("rarity");
		}
		_isDwarvenRecipe = set.getBoolean("isDwarvenRecipe");
	}
	
	/**
	 * Add a RecipeHolder to the RecipeList (add a line Item-Quantity needed to the Recipe).
	 * @param recipe
	 */
	public void addRecipe(RecipeHolder recipe)
	{
		int len = _recipes.Length;
		RecipeHolder[] tmp = new RecipeHolder[len + 1];
		System.arraycopy(_recipes, 0, tmp, 0, len);
		tmp[len] = recipe;
		_recipes = tmp;
	}
	
	/**
	 * Add a RecipeStatHolder of the statUse parameter to the RecipeList.
	 * @param statUse
	 */
	public void addStatUse(RecipeStatHolder statUse)
	{
		int len = _statUse.Length;
		RecipeStatHolder[] tmp = new RecipeStatHolder[len + 1];
		System.arraycopy(_statUse, 0, tmp, 0, len);
		tmp[len] = statUse;
		_statUse = tmp;
	}
	
	/**
	 * Add a RecipeStatHolder of the altStatChange parameter to the RecipeList.
	 * @param statChange
	 */
	public void addAltStatChange(RecipeStatHolder statChange)
	{
		int len = _altStatChange.Length;
		RecipeStatHolder[] tmp = new RecipeStatHolder[len + 1];
		System.arraycopy(_altStatChange, 0, tmp, 0, len);
		tmp[len] = statChange;
		_altStatChange = tmp;
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
	public String getRecipeName()
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
		return _itemId;
	}
	
	/**
	 * @return the quantity of Item crafted when using this RecipeList.
	 */
	public int getCount()
	{
		return _count;
	}
	
	/**
	 * @return the Identifier of the Rare Item crafted with this RecipeList.
	 */
	public int getRareItemId()
	{
		return _rareItemId;
	}
	
	/**
	 * @return the quantity of Rare Item crafted when using this RecipeList.
	 */
	public int getRareCount()
	{
		return _rareCount;
	}
	
	/**
	 * @return the chance of Rare Item crafted when using this RecipeList.
	 */
	public int getRarity()
	{
		return _rarity;
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
	public RecipeHolder[] getRecipes()
	{
		return _recipes;
	}
	
	/**
	 * @return the table containing all RecipeStatHolder of the statUse parameter of the RecipeList.
	 */
	public RecipeStatHolder[] getStatUse()
	{
		return _statUse;
	}
	
	/**
	 * @return the table containing all RecipeStatHolder of the AltStatChange parameter of the RecipeList.
	 */
	public RecipeStatHolder[] getAltStatChange()
	{
		return _altStatChange;
	}
}