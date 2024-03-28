namespace L2Dn.GameServer.Model.Holders;

/**
 * @author UnAfraid
 */
public class RangeAbilityPointsHolder
{
	private readonly int _min;
	private readonly int _max;
	private readonly long _sp;

	public RangeAbilityPointsHolder(int min, int max, long sp)
	{
		_min = min;
		_max = max;
		_sp = sp;
	}

	/**
	 * @return minimum value.
	 */
	public int getMin()
	{
		return _min;
	}

	/**
	 * @return maximum value.
	 */
	public int getMax()
	{
		return _max;
	}

	/**
	 * @return the SP.
	 */
	public long getSP()
	{
		return _sp;
	}
}