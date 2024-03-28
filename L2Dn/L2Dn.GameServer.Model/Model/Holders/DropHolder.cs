using L2Dn.GameServer.Enums;

namespace L2Dn.GameServer.Model.Holders;

/**
 * @author Mobius
 */
public class DropHolder
{
	private readonly DropType _dropType;
	private readonly int _itemId;
	private readonly long _min;
	private readonly long _max;
	private readonly double _chance;

	public DropHolder(DropType dropType, int itemId, long min, long max, double chance)
	{
		_dropType = dropType;
		_itemId = itemId;
		_min = min;
		_max = max;
		_chance = chance;
	}

	public DropType getDropType()
	{
		return _dropType;
	}

	public int getItemId()
	{
		return _itemId;
	}

	public long getMin()
	{
		return _min;
	}

	public long getMax()
	{
		return _max;
	}

	public double getChance()
	{
		return _chance;
	}
}