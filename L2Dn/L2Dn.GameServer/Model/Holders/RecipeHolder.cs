namespace L2Dn.GameServer.Model.Holders;

/**
 * This class describes a RecipeList component (1 line of the recipe : Item-Quantity needed).
 */
public class RecipeHolder
{
	/** The Identifier of the item needed in the RecipeHolder */
	private readonly int _itemId;

	/** The item quantity needed in the RecipeHolder */
	private readonly int _quantity;

	/**
	 * Constructor of RecipeHolder (create a new line in a RecipeList).
	 * @param itemId
	 * @param quantity
	 */
	public RecipeHolder(int itemId, int quantity)
	{
		_itemId = itemId;
		_quantity = quantity;
	}

	/**
	 * @return the Identifier of the RecipeHolder Item needed.
	 */
	public int getItemId()
	{
		return _itemId;
	}

	/**
	 * @return the Item quantity needed of the RecipeHolder.
	 */
	public int getQuantity()
	{
		return _quantity;
	}
}