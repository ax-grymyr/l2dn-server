using L2Dn.GameServer.Enums;
using L2Dn.GameServer.Utilities;

namespace L2Dn.GameServer.Model.Holders;

public class EquipmentUpgradeNormalHolder
{
	private readonly int _id;
	private readonly int _type;
	private readonly long _commission;
	private readonly double _chance;
	private readonly ItemEnchantHolder _initialItem;
	private readonly double _chanceToReceiveBonusItems;
	private readonly Map<UpgradeDataType, List<ItemEnchantHolder>> _items = new();

	/**
	 * @implNote Holder for "UpgradeNormal" equipment system; <list>
	 *           <li>Final Holder will be have getter getItems which get UpgradeDataType;</li>
	 *           <li>Don't forget to check in isHasCategory category type in getItems, for don`t get null or empty collections;</li> </list>
	 * @param id Upgrade ID in DAT file; (yep, duplication);
	 * @param type Upgrade type in DAT file (1 / 2 (used in classic);
	 * @param commission Default Adena count, needed for make "Transformation";
	 * @param chance Success chance of made "Transformation";
	 * @param initialItem Item for upgrade; (cannot be empty)
	 * @param materialItems Materials for upgrade; (can be empty)
	 * @param onSuccessItems Items, which player gets if RND will be < chance (if win);
	 * @param onFailureItems Items, which player gets if RND will be > chance (if lose);
	 * @param chanceToReceiveBonusItems Chance to obtain additional reward on Success (if win);
	 * @param bonusItems Bonus Items;
	 */
	public EquipmentUpgradeNormalHolder(int id, int type, long commission, double chance, ItemEnchantHolder initialItem,
		List<ItemEnchantHolder> materialItems, List<ItemEnchantHolder> onSuccessItems,
		List<ItemEnchantHolder> onFailureItems, double chanceToReceiveBonusItems, List<ItemEnchantHolder> bonusItems)
	{
		_id = id;
		_type = type;
		_commission = commission;
		_chance = chance;
		_initialItem = initialItem;
		_chanceToReceiveBonusItems = chanceToReceiveBonusItems;
		if (materialItems != null)
		{
			_items.put(UpgradeDataType.MATERIAL, materialItems);
		}

		_items.put(UpgradeDataType.ON_SUCCESS, onSuccessItems);
		if (onFailureItems != null)
		{
			_items.put(UpgradeDataType.ON_FAILURE, onFailureItems);
		}

		if (bonusItems != null)
		{
			_items.put(UpgradeDataType.BONUS_TYPE, bonusItems);
		}
	}

	public int getId()
	{
		return _id;
	}

	public int getType()
	{
		return _type;
	}

	public long getCommission()
	{
		return _commission;
	}

	public double getChance()
	{
		return _chance;
	}

	public ItemEnchantHolder getInitialItem()
	{
		return _initialItem;
	}

	public double getChanceToReceiveBonusItems()
	{
		return _chanceToReceiveBonusItems;
	}

	public List<ItemEnchantHolder> getItems(UpgradeDataType upgradeDataType)
	{
		return _items.get(upgradeDataType);
	}

	public bool isHasCategory(UpgradeDataType upgradeDataType)
	{
		return _items.ContainsKey(upgradeDataType);
	}
}