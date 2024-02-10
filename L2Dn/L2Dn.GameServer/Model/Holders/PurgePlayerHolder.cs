namespace L2Dn.GameServer.Model.Holders;

/**
 * @author Berezkin Nikolay, Serenitty
 */
public class PurgePlayerHolder
{
	private readonly int _points;
	private readonly int _keys;
	private int _remainingKeys;

	public PurgePlayerHolder(int points, int keys, int remainingKeys)
	{
		_points = points;
		_keys = keys;
		_remainingKeys = remainingKeys;
	}

	public int getPoints()
	{
		if (_remainingKeys == 0)
		{
			return 0;
		}

		return _points;
	}

	public int getKeys()
	{
		return _keys;
	}

	public int getRemainingKeys()
	{
		if ((_keys == 0) && (_remainingKeys == 0))
		{
			_remainingKeys = 40;
		}

		return _remainingKeys;
	}
}