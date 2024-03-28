namespace L2Dn.GameServer.Model.Holders;

/**
 * @author Mobius
 */
public class RestorationItemHolder
{
	private readonly int _id;
	private readonly long _count;
	private readonly int _minEnchant;
	private readonly int _maxEnchant;

	public RestorationItemHolder(int id, long count, int minEnchant, int maxEnchant)
	{
		_id = id;
		_count = count;
		_minEnchant = minEnchant;
		_maxEnchant = maxEnchant;
	}

	public int getId()
	{
		return _id;
	}

	public long getCount()
	{
		return _count;
	}

	public int getMinEnchant()
	{
		return _minEnchant;
	}

	public int getMaxEnchant()
	{
		return _maxEnchant;
	}
}