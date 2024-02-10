using L2Dn.GameServer.Model.Items;
using L2Dn.GameServer.Model.Items.Instances;
using L2Dn.GameServer.Model.Items.Types;
using L2Dn.GameServer.Utilities;
using NLog;

namespace L2Dn.GameServer.Data.Xml;

/**
 * This class holds the Enchant HP Bonus Data.
 * @author MrPoke, Zoey76
 */
public class EnchantItemHPBonusData
{
	private static readonly Logger LOGGER = LogManager.GetLogger(nameof(EnchantItemHPBonusData));
	
	private readonly Map<CrystalType, List<int>> _armorHPBonuses = new();
	
	private static readonly float FULL_ARMOR_MODIFIER = 1.5f; // TODO: Move it to config!
	
	/**
	 * Instantiates a new enchant hp bonus data.
	 */
	protected EnchantItemHPBonusData()
	{
		load();
	}
	
	public void parseDocument(Document doc, File f)
	{
		for (Node n = doc.getFirstChild(); n != null; n = n.getNextSibling())
		{
			if ("list".equalsIgnoreCase(n.getNodeName()))
			{
				for (Node d = n.getFirstChild(); d != null; d = d.getNextSibling())
				{
					if ("enchantHP".equalsIgnoreCase(d.getNodeName()))
					{
						List<int> bonuses = new ArrayList<>(12);
						for (Node e = d.getFirstChild(); e != null; e = e.getNextSibling())
						{
							if ("bonus".equalsIgnoreCase(e.getNodeName()))
							{
								bonuses.add(int.Parse(e.getTextContent()));
							}
						}
						_armorHPBonuses.put(parseEnum(d.getAttributes(), CrystalType.class, "grade"), bonuses);
					}
				}
			}
		}
	}
	
	public void load()
	{
		_armorHPBonuses.clear();
		parseDatapackFile("data/stats/enchantHPBonus.xml");
		LOGGER.Info(GetType().Name + ": Loaded " + _armorHPBonuses.size() + " enchant HP bonuses.");
	}
	
	/**
	 * Gets the HP bonus.
	 * @param item the item
	 * @return the HP bonus
	 */
	public int getHPBonus(Item item)
	{
		List<int> values = _armorHPBonuses.get(item.getTemplate().getCrystalTypePlus());
		if ((values == null) || values.isEmpty() || (item.getOlyEnchantLevel() <= 0))
		{
			return 0;
		}
		
		int bonus = values.get(Math.Min(item.getOlyEnchantLevel(), values.size()) - 1);
		if (item.getTemplate().getBodyPart() == ItemTemplate.SLOT_FULL_ARMOR)
		{
			return (int) (bonus * FULL_ARMOR_MODIFIER);
		}
		return bonus;
	}
	
	/**
	 * Gets the single instance of EnchantHPBonusData.
	 * @return single instance of EnchantHPBonusData
	 */
	public static EnchantItemHPBonusData getInstance()
	{
		return SingletonHolder.INSTANCE;
	}
	
	private static class SingletonHolder
	{
		public static readonly EnchantItemHPBonusData INSTANCE = new();
	}
}