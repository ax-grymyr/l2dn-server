namespace L2Dn.GameServer.Model.Holders;

/**
 * @author L2CCCP
 */
public class GreaterMagicLampHolder
{
	private readonly int _itemId;
	private readonly long _count;

	public GreaterMagicLampHolder(StatSet @params)
	{
		_itemId = @params.getInt("item");
		_count = @params.getLong("count");
	}

	public int getItemId()
	{
		return _itemId;
	}

	public long getCount()
	{
		return _count;
	}
}