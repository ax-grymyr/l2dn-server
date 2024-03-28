namespace L2Dn.GameServer.Model.Holders;

/**
 * This class describes a RecipeList component (1 line of the recipe : Item-Quantity needed).
 */
public class RecipeRareHolder: RecipeHolder
{
	private readonly int _rarity;

	/**
	 * Constructor of RecipeHolder (create a new line in a RecipeList).
	 * @param itemId
	 * @param quantity
	 */
	public RecipeRareHolder(int itemId, int quantity, int rarity): base(itemId, quantity)
	{
		_rarity = rarity;
	}

	public int getRarity()
	{
		return _rarity;
	}
}