using L2Dn.GameServer.Model.Items.Instances;
using L2Dn.GameServer.Utilities;

namespace L2Dn.GameServer.Model.Commission;

public class CommissionItem
{
	private readonly long _commissionId;
	private readonly Item _itemInstance;
	private readonly ItemInfo _itemInfo;
	private readonly long _pricePerUnit;
	private readonly DateTime _startTime;
	private readonly int _durationInDays;
	private readonly int _discountInPercentage;
	private ScheduledFuture _saleEndTask;
	
	public CommissionItem(long commissionId, Item itemInstance, long pricePerUnit, DateTime startTime, int durationInDays, int discountInPercentage)
	{
		_commissionId = commissionId;
		_itemInstance = itemInstance;
		_itemInfo = new ItemInfo(_itemInstance);
		_pricePerUnit = pricePerUnit;
		_startTime = startTime;
		_durationInDays = durationInDays;
		_discountInPercentage = discountInPercentage;
	}
	
	/**
	 * Gets the commission id.
	 * @return the commission id
	 */
	public long getCommissionId()
	{
		return _commissionId;
	}
	
	/**
	 * Gets the item instance.
	 * @return the item instance
	 */
	public Item getItemInstance()
	{
		return _itemInstance;
	}
	
	/**
	 * Gets the item info.
	 * @return the item info
	 */
	public ItemInfo getItemInfo()
	{
		return _itemInfo;
	}
	
	/**
	 * Gets the price per unit.
	 * @return the price per unit
	 */
	public long getPricePerUnit()
	{
		return _pricePerUnit;
	}
	
	/**
	 * Gets the start time.
	 * @return the start time
	 */
	public DateTime getStartTime()
	{
		return _startTime;
	}
	
	/**
	 * Gets the duration in days.
	 * @return the duration in days
	 */
	public int getDurationInDays()
	{
		return _durationInDays;
	}
	
	/**
	 * Gets the discount in percentage
	 * @return the _discountInPercentage
	 */
	public int getDiscountInPercentage()
	{
		return _discountInPercentage;
	}
	
	/**
	 * Gets the end time.
	 * @return the end time
	 */
	public DateTime getEndTime()
	{
		return _startTime.AddDays(_durationInDays);
	}
	
	/**
	 * Gets the sale end task.
	 * @return the sale end task
	 */
	public ScheduledFuture getSaleEndTask()
	{
		return _saleEndTask;
	}
	
	/**
	 * Sets the sale end task.
	 * @param saleEndTask the sale end task
	 */
	public void setSaleEndTask(ScheduledFuture saleEndTask)
	{
		_saleEndTask = saleEndTask;
	}
}