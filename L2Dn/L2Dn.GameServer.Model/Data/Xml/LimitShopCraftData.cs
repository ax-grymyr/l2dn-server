using System.Xml.Linq;
using L2Dn.Extensions;
using L2Dn.GameServer.Model.Holders;
using L2Dn.GameServer.Model.Items;
using L2Dn.GameServer.Utilities;
using L2Dn.Utilities;
using NLog;

namespace L2Dn.GameServer.Data.Xml;

/**
 * @author Mobius, GustavoFonseca
 */
public class LimitShopCraftData: DataReaderBase
{
	private static readonly Logger LOGGER = LogManager.GetLogger(nameof(LimitShopData));
	
	private readonly List<LimitShopProductHolder> _products = new();
	
	protected LimitShopCraftData()
	{
		load();
	}
	
	public void load()
	{
		_products.Clear();
		
		XDocument document = LoadXmlDocument(DataFileLocation.Data, "LimitShopCraft.xml");
		document.Elements("list").Where(l => l.Attribute("enabled").GetBoolean(false)).Elements("product")
			.ForEach(parseElement);
		
		if (_products.Count != 0)
		{
			LOGGER.Info(GetType().Name + ": Loaded " + _products.Count + " items.");
		}
		else
		{
			LOGGER.Info(GetType().Name + ": System is disabled.");
		}
	}

	private void parseElement(XElement element)
	{
		int id = element.GetAttributeValueAsInt32("id");
		int category = element.GetAttributeValueAsInt32("category");
		int minLevel = element.Attribute("minLevel").GetInt32(1);
		int maxLevel = element.Attribute("maxLevel").GetInt32(999);
		int[] ingredientIds = new int[5];
		ingredientIds[0] = 0;
		ingredientIds[1] = 0;
		ingredientIds[2] = 0;
		ingredientIds[3] = 0;
		ingredientIds[4] = 0;
		long[] ingredientQuantities = new long[5];
		ingredientQuantities[0] = 0;
		ingredientQuantities[1] = 0;
		ingredientQuantities[2] = 0;
		ingredientQuantities[3] = 0;
		ingredientQuantities[4] = 0;
		int[] ingredientEnchants = new int[5];
		ingredientEnchants[0] = 0;
		ingredientEnchants[1] = 0;
		ingredientEnchants[2] = 0;
		ingredientEnchants[3] = 0;
		ingredientEnchants[4] = 0;
		int productionId = 0;
		int productionId2 = 0;
		int productionId3 = 0;
		int productionId4 = 0;
		int productionId5 = 0;
		long count = 1L;
		long count2 = 1L;
		long count3 = 1L;
		long count4 = 1L;
		long count5 = 1L;
		float chance = 100f;
		float chance2 = 100f;
		float chance3 = 100f;
		float chance4 = 100f;
		bool announce = false;
		bool announce2 = false;
		bool announce3 = false;
		bool announce4 = false;
		bool announce5 = false;
		int enchant = 0;
		int accountDailyLimit = 0;
		int accountMontlyLimit = 0;
		int accountBuyLimit = 0;

		element.Elements("ingredient").ForEach(el =>
		{
			int ingredientId = el.GetAttributeValueAsInt32("id");
			long ingredientQuantity = el.Attribute("count").GetInt64(1);
			int ingredientEnchant = el.Attribute("enchant").GetInt32(0);

			if (ingredientId > 0)
			{
				ItemTemplate item = ItemData.getInstance().getTemplate(ingredientId);
				if (item == null)
				{
					LOGGER.Error(GetType().Name + ": Item template null for itemId: " + productionId + " productId: " +
					             id);
					return;
				}
			}

			if (ingredientIds[0] == 0)
			{
				ingredientIds[0] = ingredientId;
			}
			else if (ingredientIds[1] == 0)
			{
				ingredientIds[1] = ingredientId;
			}
			else if (ingredientIds[2] == 0)
			{
				ingredientIds[2] = ingredientId;
			}
			else if (ingredientIds[3] == 0)
			{
				ingredientIds[3] = ingredientId;
			}
			else
			{
				ingredientIds[4] = ingredientId;
			}

			if (ingredientQuantities[0] == 0)
			{
				ingredientQuantities[0] = ingredientQuantity;
			}
			else if (ingredientQuantities[1] == 0)
			{
				ingredientQuantities[1] = ingredientQuantity;
			}
			else if (ingredientQuantities[2] == 0)
			{
				ingredientQuantities[2] = ingredientQuantity;
			}
			else if (ingredientQuantities[3] == 0)
			{
				ingredientQuantities[3] = ingredientQuantity;
			}
			else
			{
				ingredientQuantities[4] = ingredientQuantity;
			}

			if (ingredientEnchants[0] == 0)
			{
				ingredientEnchants[0] = ingredientEnchant;
			}
			else if (ingredientEnchants[1] == 0)
			{
				ingredientEnchants[1] = ingredientEnchant;
			}
			else if (ingredientEnchants[2] == 0)
			{
				ingredientEnchants[2] = ingredientEnchant;
			}
			else if (ingredientEnchants[3] == 0)
			{
				ingredientEnchants[3] = ingredientEnchant;
			}
			else
			{
				ingredientEnchants[4] = ingredientEnchant;
			}
		});

		element.Elements("production").ForEach(el =>
		{
			productionId = el.GetAttributeValueAsInt32("id");
			count = el.Attribute("count").GetInt64(1);
			chance = el.Attribute("chance").GetFloat(100f);
			announce = el.Attribute("announce").GetBoolean(false);
			enchant = el.Attribute("enchant").GetInt32(0);
			productionId2 = el.Attribute("id2").GetInt32(0);
			count2 = el.Attribute("count2").GetInt64(1);
			chance2 = el.Attribute("chance2").GetFloat(100f);
			announce2 = el.Attribute("announce2").GetBoolean(false);
			productionId3 = el.Attribute("id3").GetInt32(0);
			count3 = el.Attribute("count3").GetInt64(1);
			chance3 = el.Attribute("chance3").GetFloat(100f);
			announce3 = el.Attribute("announce3").GetBoolean(false);
			productionId4 = el.Attribute("id4").GetInt32(0);
			count4 = el.Attribute("count4").GetInt64(1);
			chance4 = el.Attribute("chance4").GetFloat(100f);
			announce4 = el.Attribute("announce4").GetBoolean(false);
			productionId5 = el.Attribute("id5").GetInt32(0);
			count5 = el.Attribute("count5").GetInt64(1);
			announce5 = el.Attribute("announce5").GetBoolean(false);
			accountDailyLimit = el.Attribute("accountDailyLimit").GetInt32(0);
			accountMontlyLimit = el.Attribute("accountMontlyLimit").GetInt32(0);
			accountBuyLimit = el.Attribute("accountBuyLimit").GetInt32(0);

			ItemTemplate item = ItemData.getInstance().getTemplate(productionId);
			if (item == null)
				LOGGER.Error(GetType().Name + ": Item template null for itemId: " + productionId + " productId: " + id);
		});

		_products.add(new LimitShopProductHolder(id, category, minLevel, maxLevel, ingredientIds, ingredientQuantities,
			ingredientEnchants, productionId, count, chance, announce, enchant, productionId2, count2, chance2,
			announce2, productionId3, count3, chance3, announce3, productionId4, count4, chance4, announce4,
			productionId5, count5, announce5, accountDailyLimit, accountMontlyLimit, accountBuyLimit));
	}

	public LimitShopProductHolder getProduct(int id)
	{
		foreach (LimitShopProductHolder product in _products)
		{
			if (product.getId() == id)
			{
				return product;
			}
		}
		return null;
	}
	
	public List<LimitShopProductHolder> getProducts()
	{
		return _products;
	}
	
	public static LimitShopCraftData getInstance()
	{
		return SingletonHolder.INSTANCE;
	}
	
	private static class SingletonHolder
	{
		public static readonly LimitShopCraftData INSTANCE = new();
	}
}