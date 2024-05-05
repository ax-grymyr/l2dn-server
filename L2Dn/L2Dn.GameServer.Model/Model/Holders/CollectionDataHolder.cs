using System.Collections.Immutable;

namespace L2Dn.GameServer.Model.Holders;

/**
 * @author Berezkin Nikolay
 */
public class CollectionDataHolder
{
	private readonly int _collectionId;
	private readonly int _optionId;
	private readonly int _category;
	private readonly int _completeCount;
	private readonly ImmutableArray<ItemEnchantHolder> _items;

	public CollectionDataHolder(int collectionId, int optionId, int category, int completeCount,
		ImmutableArray<ItemEnchantHolder> items)
	{
		_collectionId = collectionId;
		_optionId = optionId;
		_category = category;
		_completeCount = completeCount;
		_items = items;
	}

	public int getCollectionId()
	{
		return _collectionId;
	}

	public int getOptionId()
	{
		return _optionId;
	}

	public int getCategory()
	{
		return _category;
	}

	public int getCompleteCount()
	{
		return _completeCount;
	}

	public ImmutableArray<ItemEnchantHolder> getItems()
	{
		return _items;
	}
}