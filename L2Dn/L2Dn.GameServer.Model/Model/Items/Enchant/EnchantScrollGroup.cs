namespace L2Dn.GameServer.Model.Items.Enchant;

/**
 * @author UnAfraid
 */
public class EnchantScrollGroup
{
	private readonly int _id;
	private List<EnchantRateItem> _rateGroups = [];

	public EnchantScrollGroup(int id)
	{
		_id = id;
	}

	/**
	 * @return id of current enchant scroll group.
	 */
	public int getId()
	{
		return _id;
	}

	/**
	 * Adds new rate group.
	 * @param group
	 */
	public void addRateGroup(EnchantRateItem group)
	{
		_rateGroups.Add(group);
	}

	/**
	 * @return {@code List} of all enchant rate items, Empty list if none.
	 */
	public List<EnchantRateItem> getRateGroups()
	{
		return _rateGroups;
	}

	/**
	 * @param item
	 * @return {@link EnchantRateItem}, {@code NULL} in case non of rate items can be used with.
	 */
	public EnchantRateItem? getRateGroup(ItemTemplate item)
	{
		foreach (EnchantRateItem group in getRateGroups())
		{
			if (group.validate(item))
			{
				return group;
			}
		}

		return null;
	}
}