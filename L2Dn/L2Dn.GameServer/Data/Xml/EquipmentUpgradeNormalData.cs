using System.Xml.Linq;
using L2Dn.Extensions;
using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Holders;
using L2Dn.GameServer.Utilities;
using L2Dn.Utilities;
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
		
		string filePath = Path.Combine(Config.DATAPACK_ROOT_PATH, "data/EquipmentUpgradeNormalData.xml");
		using FileStream stream = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read);
		XDocument document = XDocument.Load(stream);
		document.Elements("list").Elements("params").ForEach(el => _commission = el.Attribute("commission").GetInt32());
		if (_commission < 0)
		{
			LOGGER.Warn(GetType().Name +
			            ": Commission in file EquipmentUpgradeNormalData.xml not set or " +
			            "less than 0! Setting up default value - 100!");

			_commission = 100;
		}

		document.Elements("list").Elements("discount").Elements("item").ForEach(parseDiscountElement);
		document.Elements("list").Elements("upgrade").ForEach(parseUpgradeElement);
		
		if (!_upgrades.isEmpty())
		{
			LOGGER.Info(GetType().Name + ": Loaded " + _upgrades.size() + " upgrade-normal equipment data. Adena commission is " + _commission + ".");
		}
		
		if (!_discount.isEmpty())
		{
			LOGGER.Info(GetType().Name + ": Loaded " + _discount.size() + " upgrade-normal discount data.");
		}
	}

	private void parseDiscountElement(XElement element)
	{
		int id = element.Attribute("id").GetInt32();
		long count = element.Attribute("count").GetInt64();
		_discount.add(new ItemHolder(id, count));
	}

	private void parseUpgradeElement(XElement element)
	{
			ItemEnchantHolder? initialItem = null;
			List<ItemEnchantHolder> materialItems = new();
			List<ItemEnchantHolder> onSuccessItems = new();
			List<ItemEnchantHolder> onFailureItems = new();
			List<ItemEnchantHolder> bonusItems = new();
			double bonusChance = 0;

			int id = element.Attribute("id").GetInt32();
			int type = element.Attribute("type").GetInt32();
			double chance = element.Attribute("chance").GetDouble();
			long commission = _commission == 0 ? 0 : ((element.Attribute("commission").GetInt64() / 100) * _commission);
			
			element.Elements("upgradeItem").ForEach(upgradeEl =>
			{
				int itemId = upgradeEl.Attribute("id").GetInt32();
				long count =  upgradeEl.Attribute("count").GetInt64();
				byte enchantLevel = upgradeEl.Attribute("enchantLevel").GetByte();
				initialItem = new ItemEnchantHolder(itemId, count, enchantLevel);
				if (initialItem.getCount() < 0)
				{
					LOGGER.Warn(GetType().Name + ": upgradeItem => item => count in file EquipmentUpgradeNormalData.xml for upgrade id " + id + " cant be less than 0!");
				}
			});

			element.Elements("material").Elements("item").ForEach(materialEl =>
			{
				int itemId = materialEl.Attribute("id").GetInt32();
				long count = materialEl.Attribute("count").GetInt64();
				int enchantLevel = materialEl.Attribute("enchantLevel").GetInt32();
				// {
				// LOGGER.Warn(GetType().Name + ": material => item => count in file EquipmentUpgradeNormalData.xml for upgrade id " + id +" cant be less than 0!");
				// }
				materialItems.add(new ItemEnchantHolder(itemId, count, enchantLevel));
			});

			element.Elements("successItems").Elements("item").ForEach(materialEl =>
			{
				int itemId = materialEl.Attribute("id").GetInt32();
				long count = materialEl.Attribute("count").GetInt64();
				int enchantLevel = materialEl.Attribute("enchantLevel").GetInt32();
				onSuccessItems.add(new ItemEnchantHolder(itemId, count, enchantLevel));
			});

			element.Elements("failureItems").Elements("item").ForEach(materialEl =>
			{
				int itemId = materialEl.Attribute("id").GetInt32();
				long count = materialEl.Attribute("count").GetInt64();
				int enchantLevel = materialEl.Attribute("enchantLevel").GetInt32();
				onFailureItems.add(new ItemEnchantHolder(itemId, count, enchantLevel));
			});

			element.Elements("bonus_items").ForEach(bonusEl =>
			{
				bonusChance = bonusEl.Attribute("chance").GetDouble();
				if (bonusChance < 0)
				{
					LOGGER.Warn(GetType().Name + ": bonus_items => chance in file EquipmentUpgradeNormalData.xml for upgrade id " + id + " cant be less than 0!");
				}
				
				bonusEl.Elements("item").ForEach(materialEl =>
				{
					int itemId = materialEl.Attribute("id").GetInt32();
					long count = materialEl.Attribute("count").GetInt64();
					int enchantLevel = materialEl.Attribute("enchantLevel").GetInt32();
					bonusItems.add(new ItemEnchantHolder(itemId, count, enchantLevel));
				});
			});

			if (initialItem is null)
			{
				LOGGER.Error(GetType().Name + ": no upgradeItem for upgrade id " + id);
			}
			
			_upgrades.put(id,
				new EquipmentUpgradeNormalHolder(id, type, commission, chance, initialItem, materialItems,
					onSuccessItems, onFailureItems, bonusChance, bonusItems));
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