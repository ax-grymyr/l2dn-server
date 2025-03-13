using L2Dn.GameServer.Dto;
using L2Dn.GameServer.Model.Holders;
using L2Dn.GameServer.Utilities;

namespace L2Dn.GameServer.Model.Items.Henna;

/**
 * @author Serenitty
 */
public class DyePotentialFee
{
	private readonly int _step;
	private readonly List<ItemHolder> _items;
	private readonly int _dailyCount;
	private readonly Map<int, double> _enchantExp;

	public DyePotentialFee(int step, List<ItemHolder> items, int dailyCount, Map<int, double> enchantExp)
	{
		_step = step;
		_items = items;
		_dailyCount = dailyCount;
		_enchantExp = enchantExp;
	}

	public int getStep()
	{
		return _step;
	}

	public List<ItemHolder> getItems()
	{
		return _items;
	}

	public int getDailyCount()
	{
		return _dailyCount;
	}

	public Map<int, double> getEnchantExp()
	{
		return _enchantExp;
	}
}