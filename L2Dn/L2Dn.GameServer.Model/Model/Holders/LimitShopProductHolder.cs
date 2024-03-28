namespace L2Dn.GameServer.Model.Holders;

/**
 * @author Mobius
 */
public class LimitShopProductHolder
{
	private readonly int _id;
	private readonly int _category;
	private readonly int _minLevel;
	private readonly int _maxLevel;
	private readonly int[] _ingredientIds;
	private readonly long[] _ingredientQuantities;
	private readonly int[] _ingredientEnchants;
	private readonly int _productionId;
	private readonly long _count;
	private readonly float _chance;
	private readonly bool _announce;
	private readonly int _enchant;
	private readonly int _productionId2;
	private readonly long _count2;
	private readonly float _chance2;
	private readonly bool _announce2;
	private readonly int _productionId3;
	private readonly long _count3;
	private readonly float _chance3;
	private readonly bool _announce3;
	private readonly int _productionId4;
	private readonly long _count4;
	private readonly float _chance4;
	private readonly bool _announce4;
	private readonly int _productionId5;
	private readonly long _count5;
	private readonly bool _announce5;
	private readonly int _accountDailyLimit;
	private readonly int _accountMontlyLimit;
	private readonly int _accountBuyLimit;

	public LimitShopProductHolder(int id, int category, int minLevel, int maxLevel, int[] ingredientIds,
		long[] ingredientQuantities, int[] ingredientEnchants, int productionId, long count, float chance,
		bool announce, int enchant, int productionId2, long count2, float chance2, bool announce2, int productionId3,
		long count3, float chance3, bool announce3, int productionId4, long count4, float chance4, bool announce4,
		int productionId5, long count5, bool announce5, int accountDailyLimit, int accountMontlyLimit,
		int accountBuyLimit)
	{
		_id = id;
		_category = category;
		_minLevel = minLevel;
		_maxLevel = maxLevel;
		_ingredientIds = ingredientIds;
		_ingredientQuantities = ingredientQuantities;
		_ingredientEnchants = ingredientEnchants;
		_productionId = productionId;
		_count = count;
		_chance = chance;
		_announce = announce;
		_enchant = enchant;
		_productionId2 = productionId2;
		_count2 = count2;
		_chance2 = chance2;
		_announce2 = announce2;
		_productionId3 = productionId3;
		_count3 = count3;
		_chance3 = chance3;
		_announce3 = announce3;
		_productionId4 = productionId4;
		_count4 = count4;
		_chance4 = chance4;
		_announce4 = announce4;
		_productionId5 = productionId5;
		_count5 = count5;
		_announce5 = announce5;
		_accountDailyLimit = accountDailyLimit;
		_accountMontlyLimit = accountMontlyLimit;
		_accountBuyLimit = accountBuyLimit;
	}

	public int getId()
	{
		return _id;
	}

	public int getCategory()
	{
		return _category;
	}

	public int getMinLevel()
	{
		return _minLevel;
	}

	public int getMaxLevel()
	{
		return _maxLevel;
	}

	public int[] getIngredientIds()
	{
		return _ingredientIds;
	}

	public long[] getIngredientQuantities()
	{
		return _ingredientQuantities;
	}

	public int[] getIngredientEnchants()
	{
		return _ingredientEnchants;
	}

	public int getProductionId()
	{
		return _productionId;
	}

	public long getCount()
	{
		return _count;
	}

	public float getChance()
	{
		return _chance;
	}

	public bool isAnnounce()
	{
		return _announce;
	}

	public int getEnchant()
	{
		return _enchant;
	}

	public int getProductionId2()
	{
		return _productionId2;
	}

	public long getCount2()
	{
		return _count2;
	}

	public float getChance2()
	{
		return _chance2;
	}

	public bool isAnnounce2()
	{
		return _announce2;
	}

	public int getProductionId3()
	{
		return _productionId3;
	}

	public long getCount3()
	{
		return _count3;
	}

	public float getChance3()
	{
		return _chance3;
	}

	public bool isAnnounce3()
	{
		return _announce3;
	}

	public int getProductionId4()
	{
		return _productionId4;
	}

	public long getCount4()
	{
		return _count4;
	}

	public float getChance4()
	{
		return _chance4;
	}

	public bool isAnnounce4()
	{
		return _announce4;
	}

	public int getProductionId5()
	{
		return _productionId5;
	}

	public long getCount5()
	{
		return _count5;
	}

	public bool isAnnounce5()
	{
		return _announce5;
	}

	public int getAccountDailyLimit()
	{
		return _accountDailyLimit;
	}

	public int getAccountMontlyLimit()
	{
		return _accountMontlyLimit;
	}

	public int getAccountBuyLimit()
	{
		return _accountBuyLimit;
	}
}