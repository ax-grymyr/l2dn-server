using L2Dn.GameServer.Model.Holders;
using NLog;

namespace L2Dn.GameServer.Model.Items.Enchant;

/**
 * @author UnAfraid
 */
public class EnchantItemGroup
{
	private static readonly Logger LOGGER = LogManager.GetLogger(nameof(EnchantItemGroup));
	private readonly List<RangeChanceHolder> _chances = new();
	private readonly string _name;
	private int _maximumEnchant = -1;

	public EnchantItemGroup(string name)
	{
		_name = name;
	}

	/**
	 * @return name of current enchant item group.
	 */
	public string getName()
	{
		return _name;
	}

	/**
	 * @param holder
	 */
	public void addChance(RangeChanceHolder holder)
	{
		_chances.Add(holder);
	}

	/**
	 * @param index
	 * @return chance for success rate for current enchant item group.
	 */
	public double getChance(int index)
	{
		if (_chances.Count != 0)
		{
			foreach (RangeChanceHolder holder in _chances)
			{
				if ((holder.getMin() <= index) && (holder.getMax() >= index))
				{
					return holder.getChance();
				}
			}

			// LOGGER.log(Level.WARNING, getClass().getSimpleName() + ": Couldn't match proper chance for item group: " + _name);
			return _chances[_chances.Count - 1].getChance();
		}

		LOGGER.Warn(GetType().Name + ": item group: " + _name + " doesn't have any chances!");
		return -1;
	}

	/**
	 * @return the maximum enchant level for current enchant item group.
	 */
	public int getMaximumEnchant()
	{
		if (_maximumEnchant == -1)
		{
			foreach (RangeChanceHolder holder in _chances)
			{
				if ((holder.getChance() > 0) && (holder.getMax() > _maximumEnchant))
				{
					_maximumEnchant = holder.getMax();
				}
			}

			_maximumEnchant++;
		}

		return _maximumEnchant;
	}
}