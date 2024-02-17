using L2Dn.GameServer.Enums;
using L2Dn.GameServer.Model.Items.Instances;

namespace L2Dn.GameServer.Model.Holders;

/**
 * @author Index
 */
public class WorldExchangeHolder
{
	private readonly int _worldExchangeId;
	private readonly Item _itemInstance;
	private readonly ItemInfo _itemInfo;
	private readonly long _price;
	private readonly int _oldOwnerId;
	private WorldExchangeItemStatusType _storeType;
	private readonly WorldExchangeItemSubType _category;
	private readonly DateTime _startTime;
	private DateTime _endTime;
	private bool _hasChanges;

	public WorldExchangeHolder(int worldExchangeId, Item itemInstance, ItemInfo itemInfo, long price, int oldOwnerId,
		WorldExchangeItemStatusType storeType, WorldExchangeItemSubType category, DateTime startTime, DateTime endTime,
		bool hasChanges)
	{
		_worldExchangeId = worldExchangeId;
		_itemInstance = itemInstance;
		_itemInfo = itemInfo;
		_price = price;
		_oldOwnerId = oldOwnerId;
		_storeType = storeType;
		_category = category;
		_startTime = startTime;
		_endTime = endTime;
		_hasChanges = hasChanges;
	}

	public int getWorldExchangeId()
	{
		return _worldExchangeId;
	}

	public Item getItemInstance()
	{
		return _itemInstance;
	}

	public ItemInfo getItemInfo()
	{
		return _itemInfo;
	}

	public long getPrice()
	{
		return _price;
	}

	public int getOldOwnerId()
	{
		return _oldOwnerId;
	}

	public WorldExchangeItemStatusType getStoreType()
	{
		return _storeType;
	}

	public void setStoreType(WorldExchangeItemStatusType storeType)
	{
		_storeType = storeType;
	}

	public WorldExchangeItemSubType getCategory()
	{
		return _category;
	}

	public DateTime getStartTime()
	{
		return _startTime;
	}

	public DateTime getEndTime()
	{
		return _endTime;
	}

	public void setEndTime(DateTime endTime)
	{
		_endTime = endTime;
	}

	public bool hasChanges()
	{
		if (_hasChanges) // TODO: Fix logic.
		{
			_hasChanges = false;
			return true;
		}

		return false;
	}

	public void setHasChanges(bool hasChanges)
	{
		_hasChanges = hasChanges;
	}
}