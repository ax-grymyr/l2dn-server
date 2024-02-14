using System.Xml.Linq;
using L2Dn.Utilities;

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

	public PrimeShopGroup(XElement element, List<PrimeShopItem> items)
	{
		_brId = element.Attribute("id").GetInt32();
		_category = element.Attribute("cat").GetInt32(0);
		_paymentType = element.Attribute("paymentType").GetInt32(0);
		_price = element.Attribute("price").GetInt32();
		_panelType = element.Attribute("panelType").GetInt32(0);
		_recommended = element.Attribute("recommended").GetInt32(0);
		_start = element.Attribute("startSale").GetInt32(0);
		_end = element.Attribute("endSale").GetInt32(0);
		_daysOfWeek = element.Attribute("daysOfWeek").GetInt32(127);
		_startHour = element.Attribute("startHour").GetInt32(0);
		_startMinute = element.Attribute("startMinute").GetInt32(0);
		_stopHour = element.Attribute("stopHour").GetInt32(0);
		_stopMinute = element.Attribute("stopMinute").GetInt32(0);
		_stock = element.Attribute("stock").GetInt32(0);
		_maxStock = element.Attribute("maxStock").GetInt32(-1);
		_salePercent = element.Attribute("salePercent").GetInt32(0);
		_minLevel = element.Attribute("minLevel").GetInt32(0);
		_maxLevel = element.Attribute("maxLevel").GetInt32(0);
		_minBirthday = element.Attribute("minBirthday").GetInt32(0);
		_maxBirthday = element.Attribute("maxBirthday").GetInt32(0);
		_accountDailyLimit = element.Attribute("accountDailyLimit").GetInt32(0);
		_accountBuyLimit = element.Attribute("accountBuyLimit").GetInt32(0);
		_isVipGift = element.Attribute("isVipGift").GetBoolean(false);
		_vipTier = element.Attribute("vipTier").GetInt32(0);

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