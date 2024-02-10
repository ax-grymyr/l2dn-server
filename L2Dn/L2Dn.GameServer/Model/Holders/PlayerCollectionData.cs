namespace L2Dn.GameServer.Model.Holders;

/**
 * @author Berezkin Nikolay
 */
public class PlayerCollectionData
{
	private readonly int _collectionId;
	private readonly int _itemId;
	private readonly int _index;

	public PlayerCollectionData(int collectionId, int itemId, int index)
	{
		_collectionId = collectionId;
		_itemId = itemId;
		_index = index;
	}

	public int getCollectionId()
	{
		return _collectionId;
	}

	public int getItemId()
	{
		return _itemId;
	}

	public int getIndex()
	{
		return _index;
	}
}