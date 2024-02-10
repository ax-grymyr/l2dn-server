namespace L2Dn.GameServer.Model.Holders;

/**
 * @author UnAfraid
 */
public class CrystallizationDataHolder
{
	private readonly int _id;
	private readonly List<ItemChanceHolder> _items;

	public CrystallizationDataHolder(int id, List<ItemChanceHolder> items)
	{
		_id = id;
		_items = Collections.unmodifiableList(items);
	}

	public int getId()
	{
		return _id;
	}

	public List<ItemChanceHolder> getItems()
	{
		return _items;
	}
}