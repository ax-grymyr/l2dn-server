namespace L2Dn.GameServer.Model.Holders;

/**
 * @author Mobius
 */
public class RandomCraftExtractDataHolder
{
	private readonly long _points;
	private readonly long _fee;

	public RandomCraftExtractDataHolder(long points, long fee)
	{
		_points = points;
		_fee = fee;
	}

	public long getPoints()
	{
		return _points;
	}

	public long getFee()
	{
		return _fee;
	}
}