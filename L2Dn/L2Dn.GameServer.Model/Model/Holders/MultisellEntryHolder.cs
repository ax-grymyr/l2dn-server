using L2Dn.GameServer.Data.Xml;
using L2Dn.GameServer.Model.Items;

namespace L2Dn.GameServer.Model.Holders;

/**
 * @author Nik, Mobius
 */
public class MultisellEntryHolder
{
	private readonly bool _stackable;
	private readonly List<ItemChanceHolder> _ingredients;
	private readonly List<ItemChanceHolder> _products;

	public MultisellEntryHolder(List<ItemChanceHolder> ingredients, List<ItemChanceHolder> products)
	{
		_ingredients = ingredients;
		_products = products;

		foreach (ItemChanceHolder product in products)
		{
			ItemTemplate? item = ItemData.getInstance().getTemplate(product.getId());
			if (item == null || !item.isStackable())
			{
				_stackable = false;
				return;
			}
		}

		_stackable = true;
	}

	public List<ItemChanceHolder> getIngredients()
	{
		return _ingredients;
	}

	public List<ItemChanceHolder> getProducts()
	{
		return _products;
	}

	public bool isStackable()
	{
		return _stackable;
	}
}