using System.Collections.Frozen;
using System.Collections.Immutable;
using L2Dn.GameServer.Model.Items;
using L2Dn.GameServer.Model.Items.Instances;
using L2Dn.Model.Xml;
using NLog;

namespace L2Dn.GameServer.Data.Xml;

public sealed class EnchantItemOptionsData: DataReaderBase
{
	private static readonly Logger _logger = LogManager.GetLogger(nameof(EnchantItemOptionsData));

	private FrozenDictionary<(int ItemId, int Level), ImmutableArray<int>> _data =
		FrozenDictionary<(int ItemId, int Level), ImmutableArray<int>>.Empty;

	private EnchantItemOptionsData()
	{
		load();
	}

	public void load()
	{
		int itemCount = 0;
		Dictionary<(int ItemId, int Level), ImmutableArray<int>> data = [];

		XmlEnchantItemOptionData document =
			LoadXmlDocument<XmlEnchantItemOptionData>(DataFileLocation.Data, "EnchantItemOptions.xml");

		foreach (XmlEnchantItemOptionItem xmlEnchantItemOptionItem in document.Items)
		{
			int itemId = xmlEnchantItemOptionItem.ItemId;
			ItemTemplate? template = ItemData.getInstance().getTemplate(itemId);
			if (template == null)
			{
				_logger.Error(GetType().Name + ": Could not find item template for id " + itemId);
				continue;
			}

			foreach (XmlEnchantItemOption xmlEnchantItemOption in xmlEnchantItemOptionItem.Options)
			{
				int level = xmlEnchantItemOption.Level;
				List<int> enchantOptions =
					[xmlEnchantItemOption.Option1, xmlEnchantItemOption.Option2, xmlEnchantItemOption.Option3];

				enchantOptions.RemoveAll(op => op <= 0);

				foreach (int option in enchantOptions)
				{
					if (OptionData.getInstance().getOptions(option) == null)
						_logger.Error(GetType().Name + ": Could not find option " + option + " for item " + template);
				}

				if (!data.TryAdd((itemId, level), enchantOptions.ToImmutableArray()))
					_logger.Error(GetType().Name + $": Duplicated data for item id {itemId} and level {level}.");
			}

			itemCount++;
		}

		_data = data.ToFrozenDictionary();

		_logger.Info(GetType().Name + ": Loaded " + itemCount + " items and " + _data.Count + " options.");
	}

	/**
	 * @param itemId
	 * @param enchantLevel
	 * @return enchant effects information.
	 */
	public ImmutableArray<int> getOptions(int itemId, int enchantLevel)
	{
		return _data.GetValueOrDefault((itemId, enchantLevel), ImmutableArray<int>.Empty);
	}

	/**
	 * @param item
	 * @return enchant effects information.
	 */
	public ImmutableArray<int> getOptions(Item item)
	{
		return item != null ? getOptions(item.getId(), item.getEnchantLevel()) : ImmutableArray<int>.Empty;
	}

	/**
	 * Gets the single instance of EnchantOptionsData.
	 * @return single instance of EnchantOptionsData
	 */
	public static EnchantItemOptionsData getInstance()
	{
		return SingletonHolder.INSTANCE;
	}

	private static class SingletonHolder
	{
		public static readonly EnchantItemOptionsData INSTANCE = new();
	}
}