namespace L2Dn.GameServer.Model.Holders;

/**
 * @author UnAfraid
 */
public class RangeChanceHolder
{
	private readonly int _min;
	private readonly int _max;
	private readonly double _chance;

	public RangeChanceHolder(int min, int max, double chance)
	{
		_min = min;
		_max = max;
		_chance = chance;
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
	 * @return the chance.
	 */
	public double getChance()
	{
		return _chance;
	}
}