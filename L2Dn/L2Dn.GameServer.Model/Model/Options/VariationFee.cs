namespace L2Dn.GameServer.Model.Options;

/**
 * @author Pere
 */
public class VariationFee
{
	private readonly int _itemId;
	private readonly long _itemCount;
	private readonly long _adenaFee;
	private readonly long _cancelFee;

	public VariationFee(int itemId, long itemCount, long adenaFee, long cancelFee)
	{
		_itemId = itemId;
		_itemCount = itemCount;
		_adenaFee = adenaFee;
		_cancelFee = cancelFee;
	}

	public int getItemId()
	{
		return _itemId;
	}

	public long getItemCount()
	{
		return _itemCount;
	}

	public long getAdenaFee()
	{
		return _adenaFee;
	}

	public long getCancelFee()
	{
		return _cancelFee;
	}
}