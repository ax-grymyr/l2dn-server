using L2Dn.GameServer.Dto;
using L2Dn.GameServer.Model.Holders;
using L2Dn.GameServer.Model.Items.Instances;
using L2Dn.GameServer.Utilities;

namespace L2Dn.GameServer.Model.Actor.Request;

public class EnchantItemRequest: AbstractRequest
{
	private volatile int _enchantingItemObjectId;
	private volatile int _enchantingScrollObjectId;
	private volatile int _supportItemObjectId;
	private volatile int _enchantingItemCurrentEnchantLevel;
	private readonly Map<int, int> _multiEnchantingItems = new();
	private readonly Map<int, ItemHolder> _multiFailRewardItems = new();
	private readonly Map<int, int[]> _successEnchant = new();
	private readonly Map<int, int> _failureEnchant = new();

	public EnchantItemRequest(Player player, int enchantingScrollObjectId): base(player)
	{
		_enchantingScrollObjectId = enchantingScrollObjectId;
	}

	public void setMultiSuccessEnchantList(Map<int, int[]> list)
	{
		_successEnchant.putAll(list);
	}

	public void setMultiFailureEnchantList(Map<int, int> list)
	{
		_failureEnchant.putAll(list);
	}

	public void clearMultiSuccessEnchantList()
	{
		_successEnchant.Clear();
	}

	public void clearMultiFailureEnchantList()
	{
		_failureEnchant.Clear();
	}

	public Map<int, int[]> getMultiSuccessEnchantList()
	{
		return _successEnchant;
	}

	public Map<int, int> getMultiFailureEnchantList()
	{
		return _failureEnchant;
	}

	public Item? getEnchantingItem()
	{
		return getActiveChar().getInventory().getItemByObjectId(_enchantingItemObjectId);
	}

	public void setEnchantingItem(int objectId)
	{
		_enchantingItemObjectId = objectId;
	}

	public Item? getEnchantingScroll()
	{
		return getActiveChar().getInventory().getItemByObjectId(_enchantingScrollObjectId);
	}

	public void setEnchantingScroll(int objectId)
	{
		_enchantingScrollObjectId = objectId;
	}

	public Item? getSupportItem()
	{
		return getActiveChar().getInventory().getItemByObjectId(_supportItemObjectId);
	}

	public void setSupportItem(int objectId)
	{
		_supportItemObjectId = objectId;
	}

	public void setEnchantLevel(int enchantLevel)
	{
		_enchantingItemCurrentEnchantLevel = enchantLevel;
	}

	public int getEnchantLevel()
	{
		return _enchantingItemCurrentEnchantLevel;
	}

	public void addMultiEnchantingItems(int slot, int objectId)
	{
		_multiEnchantingItems.put(slot, objectId);
	}

	public int getMultiEnchantingItemsBySlot(int slot)
	{
		return _multiEnchantingItems.GetValueOrDefault(slot, -1);
	}

	public void changeMultiEnchantingItemsBySlot(int slot, int objectId)
	{
		_multiEnchantingItems.replace(slot, objectId);
	}

	public bool checkMultiEnchantingItemsByObjectId(int objectId)
	{
		return _multiEnchantingItems.Values.Contains(objectId);
	}

	public int getMultiEnchantingItemsCount()
	{
		return _multiEnchantingItems.Count;
	}

	public void clearMultiEnchantingItemsBySlot()
	{
		_multiEnchantingItems.Clear();
	}

	public string getMultiEnchantingItemsLits()
	{
		return _multiEnchantingItems.toString();
	}

	public void addMultiEnchantFailItems(ItemHolder itemHolder)
	{
		_multiFailRewardItems.put(getMultiFailItemsCount() + 1, itemHolder);
	}

	public int getMultiFailItemsCount()
	{
		return _multiFailRewardItems.Count;
	}

	public void clearMultiFailReward()
	{
		_multiFailRewardItems.Clear();
	}

	public Map<int, ItemHolder> getMultiEnchantFailItems()
	{
		return _multiFailRewardItems;
	}

	public override bool isItemRequest()
	{
		return true;
	}

	public override bool canWorkWith(AbstractRequest request)
	{
		return !request.isItemRequest();
	}

	public override bool isUsing(int objectId)
	{
		return objectId > 0 && (objectId == _enchantingItemObjectId || objectId == _enchantingScrollObjectId || objectId == _supportItemObjectId);
	}
}