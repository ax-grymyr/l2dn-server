namespace L2Dn.GameServer.Model.PrimeShop;

public class PrimeShopGroup
{
	private readonly int _brId;
	private readonly int _category;
	private readonly int _paymentType;
	private readonly int _price;
	private readonly int _panelType;
	private readonly int _recommended;
	private readonly int _start;
	private readonly int _end;
	private readonly int _daysOfWeek;
	private readonly int _startHour;
	private readonly int _startMinute;
	private readonly int _stopHour;
	private readonly int _stopMinute;
	private readonly int _stock;
	private readonly int _maxStock;
	private readonly int _salePercent;
	private readonly int _minLevel;
	private readonly int _maxLevel;
	private readonly int _minBirthday;
	private readonly int _maxBirthday;
	private readonly int _accountDailyLimit;
	private readonly int _accountBuyLimit;
	private readonly bool _isVipGift;
	private readonly int _vipTier;
	private readonly List<PrimeShopItem> _items;

	public PrimeShopGroup(StatSet set, List<PrimeShopItem> items)
	{
		_brId = set.getInt("id");
		_category = set.getInt("cat", 0);
		_paymentType = set.getInt("paymentType", 0);
		_price = set.getInt("price");
		_panelType = set.getInt("panelType", 0);
		_recommended = set.getInt("recommended", 0);
		_start = set.getInt("startSale", 0);
		_end = set.getInt("endSale", 0);
		_daysOfWeek = set.getInt("daysOfWeek", 127);
		_startHour = set.getInt("startHour", 0);
		_startMinute = set.getInt("startMinute", 0);
		_stopHour = set.getInt("stopHour", 0);
		_stopMinute = set.getInt("stopMinute", 0);
		_stock = set.getInt("stock", 0);
		_maxStock = set.getInt("maxStock", -1);
		_salePercent = set.getInt("salePercent", 0);
		_minLevel = set.getInt("minLevel", 0);
		_maxLevel = set.getInt("maxLevel", 0);
		_minBirthday = set.getInt("minBirthday", 0);
		_maxBirthday = set.getInt("maxBirthday", 0);
		_accountDailyLimit = set.getInt("accountDailyLimit", 0);
		_accountBuyLimit = set.getInt("accountBuyLimit", 0);
		_isVipGift = set.getBoolean("isVipGift", false);
		_vipTier = set.getInt("vipTier", 0);

		_items = items;
	}

	public int getBrId()
	{
		return _brId;
	}

	public int getCat()
	{
		return _category;
	}

	public int getPaymentType()
	{
		return _paymentType;
	}

	public int getPrice()
	{
		return _price;
	}

	public long getCount()
	{
		return _items.Select(x => x.getCount()).Sum();
	}

	public int getWeight()
	{
		return _items.Select(x => x.getWeight()).Sum();
	}

	public int getPanelType()
	{
		return _panelType;
	}

	public int getRecommended()
	{
		return _recommended;
	}

	public int getStartSale()
	{
		return _start;
	}

	public int getEndSale()
	{
		return _end;
	}

	public int getDaysOfWeek()
	{
		return _daysOfWeek;
	}

	public int getStartHour()
	{
		return _startHour;
	}

	public int getStartMinute()
	{
		return _startMinute;
	}

	public int getStopHour()
	{
		return _stopHour;
	}

	public int getStopMinute()
	{
		return _stopMinute;
	}

	public int getStock()
	{
		return _stock;
	}

	public int getTotal()
	{
		return _maxStock;
	}

	public int getSalePercent()
	{
		return _salePercent;
	}

	public int getMinLevel()
	{
		return _minLevel;
	}

	public int getMaxLevel()
	{
		return _maxLevel;
	}

	public int getMinBirthday()
	{
		return _minBirthday;
	}

	public int getMaxBirthday()
	{
		return _maxBirthday;
	}

	public int getAccountDailyLimit()
	{
		return _accountDailyLimit;
	}

	public int getAccountBuyLimit()
	{
		return _accountBuyLimit;
	}

	public bool isVipGift()
	{
		return _isVipGift;
	}

	public int getVipTier()
	{
		return _vipTier;
	}

	public List<PrimeShopItem> getItems()
	{
		return _items;
	}
}