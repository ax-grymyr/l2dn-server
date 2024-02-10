using L2Dn.GameServer.Enums;

namespace L2Dn.GameServer.Model.Holders;

/**
 * @author Index
 */
public class WorldExchangeHolder
{
	private readonly long _worldExchangeId;
	private readonly Item _itemInstance;
	private readonly ItemInfo _itemInfo;
	private readonly long _price;
	private readonly int _oldOwnerId;
	private WorldExchangeItemStatusType _storeType;
	private readonly WorldExchangeItemSubType _category;
	private readonly long _startTime;
	private long _endTime;
	private bool _hasChanges;

	public WorldExchangeHolder(long worldExchangeId, Item itemInstance, ItemInfo itemInfo, long price, int oldOwnerId,
		WorldExchangeItemStatusType storeType, WorldExchangeItemSubType category, long startTime, long endTime,
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

	public long getWorldExchangeId()
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

	public long getStartTime()
	{
		return _startTime;
	}

	public long getEndTime()
	{
		return _endTime;
	}

	public void setEndTime(long endTime)
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