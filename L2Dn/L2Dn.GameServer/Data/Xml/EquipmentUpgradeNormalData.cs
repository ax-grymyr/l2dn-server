using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Holders;
using L2Dn.GameServer.Utilities;
using NLog;

namespace L2Dn.GameServer.Data.Xml;

/**
 * @author Index
 */
public class EquipmentUpgradeNormalData
{
	private static readonly Logger LOGGER = LogManager.GetLogger(nameof(EquipmentUpgradeNormalData));
	private static readonly Map<int, EquipmentUpgradeNormalHolder> _upgrades = new();
	private static readonly Set<ItemHolder> _discount = new();
	private static int _commission;
	
	protected EquipmentUpgradeNormalData()
	{
		load();
	}
	
	public void reload()
	{
		foreach (Player player in World.getInstance().getPlayers())
		{
			player.sendPacket(ExUpgradeSystemNormalResult.FAIL);
		}
		load();
	}
	
	public void load()
	{
		_commission = -1;
		_discount.clear();
		_upgrades.clear();
		parseDatapackFile("data/EquipmentUpgradeNormalData.xml");
		if (!_upgrades.isEmpty())
		{
			LOGGER.Info(GetType().Name + ": Loaded " + _upgrades.size() + " upgrade-normal equipment data. Adena commission is " + _commission + ".");
		}
		if (!_discount.isEmpty())
		{
			LOGGER.Info(GetType().Name + ": Loaded " + _discount.size() + " upgrade-normal discount data.");
		}
	}
	
	public void parseDocument(Document doc, File f)
	{
		forEach(doc, "list", listNode => forEach(listNode, "params", paramNode => _commission = new StatSet(parseAttributes(paramNode)).getInt("commission")));
		if (_commission < 0)
		{
			LOGGER.Warn(GetType().Name + ": Commission in file EquipmentUpgradeNormalData.xml not set or less than 0! Setting up default value - 100!");
			_commission = 100;
		}
		forEach(doc, "list", listNode => forEach(listNode, "discount", discountNode => forEach(discountNode, "item", itemNode =>
		{
			StatSet successSet = new StatSet(parseAttributes(itemNode));
			_discount.add(new ItemHolder(successSet.getInt("id"), successSet.getLong("count")));
		})));
		forEach(doc, "list", listNode => forEach(listNode, "upgrade", upgradeNode =>
		{
			AtomicReference<ItemEnchantHolder> initialItem = new();
			AtomicReference<List<ItemEnchantHolder>> materialItems = new(new());
			AtomicReference<List<ItemEnchantHolder>> onSuccessItems = new(new());
			AtomicReference<List<ItemEnchantHolder>> onFailureItems = new(new());
			AtomicReference<List<ItemEnchantHolder>> bonusItems = new(new());
			AtomicReference<Double> bonusChance = new AtomicReference<>();
			StatSet headerSet = new StatSet(parseAttributes(upgradeNode));
			int id = headerSet.getInt("id");
			int type = headerSet.getInt("type");
			double chance = headerSet.getDouble("chance");
			long commission = _commission == 0 ? 0 : ((headerSet.getLong("commission") / 100) * _commission);
			forEach(upgradeNode, "upgradeItem", upgradeItemNode =>
			{
				StatSet initialSet = new StatSet(parseAttributes(upgradeItemNode));
				initialItem.set(new ItemEnchantHolder(initialSet.getInt("id"), initialSet.getLong("count"), initialSet.getByte("enchantLevel")));
				if (initialItem.get() == null)
				{
					LOGGER.Warn(GetType().Name + ": upgradeItem in file EquipmentUpgradeNormalData.xml for upgrade id " + id + " seems like broken!");
				}
				if (initialItem.get().getCount() < 0)
				{
					LOGGER.Warn(GetType().Name + ": upgradeItem => item => count in file EquipmentUpgradeNormalData.xml for upgrade id " + id + " cant be less than 0!");
				}
			});
			forEach(upgradeNode, "material", materialItemNode => forEach(materialItemNode, "item", itemNode =>
			{
				StatSet successSet = new StatSet(parseAttributes(itemNode));
				materialItems.get().add(new ItemEnchantHolder(successSet.getInt("id"), successSet.getLong("count"), successSet.getInt("enchantLevel")));
				// {
				// LOGGER.Warn(GetType().Name + ": material => item => count in file EquipmentUpgradeNormalData.xml for upgrade id " + id +" cant be less than 0!");
				// }
			}));
			forEach(upgradeNode, "successItems", successItemNode => forEach(successItemNode, "item", itemNode =>
			{
				StatSet successSet = new StatSet(parseAttributes(itemNode));
				onSuccessItems.get().add(new ItemEnchantHolder(successSet.getInt("id"), successSet.getLong("count"), successSet.getInt("enchantLevel")));
			}));
			forEach(upgradeNode, "failureItems", failureItemNode => forEach(failureItemNode, "item", itemNode =>
			{
				StatSet successSet = new StatSet(parseAttributes(itemNode));
				onFailureItems.get().add(new ItemEnchantHolder(successSet.getInt("id"), successSet.getLong("count"), successSet.getInt("enchantLevel")));
			}));
			forEach(upgradeNode, "bonus_items", bonusItemNode =>
			{
				bonusChance.set(new StatSet(parseAttributes(bonusItemNode)).getDouble("chance"));
				if (bonusChance.get() < 0)
				{
					LOGGER.Warn(GetType().Name + ": bonus_items => chance in file EquipmentUpgradeNormalData.xml for upgrade id " + id + " cant be less than 0!");
				}
				forEach(bonusItemNode, "item", itemNode =>
				{
					StatSet successSet = new StatSet(parseAttributes(itemNode));
					bonusItems.get().add(new ItemEnchantHolder(successSet.getInt("id"), successSet.getLong("count"), successSet.getInt("enchantLevel")));
				});
			});
			_upgrades.put(id, new EquipmentUpgradeNormalHolder(id, type, commission, chance, initialItem.get(), materialItems.get(), onSuccessItems.get(), onFailureItems.get(), bonusChance.get() == null ? 0 : bonusChance.get(), bonusItems.get()));
		}));
	}
	
	public EquipmentUpgradeNormalHolder getUpgrade(int id)
	{
		return _upgrades.get(id);
	}
	
	public Set<ItemHolder> getDiscount()
	{
		return _discount;
	}
	
	public int getCommission()
	{
		return _commission;
	}
	
	public static EquipmentUpgradeNormalData getInstance()
	{
		return SingletonHolder.INSTANCE;
	}
	
	private static class SingletonHolder
	{
		public static readonly EquipmentUpgradeNormalData INSTANCE = new();
	}
}