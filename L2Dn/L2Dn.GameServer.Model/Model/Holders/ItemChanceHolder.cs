using L2Dn.GameServer.Dto;
using L2Dn.GameServer.Utilities;
using L2Dn.Utilities;

namespace L2Dn.GameServer.Model.Holders;

/**
 * A DTO for items; contains item ID, count and chance.
 * @author xban1x
 */
public class ItemChanceHolder: ItemHolder
{
	private readonly double _chance;
	private readonly byte _enchantmentLevel;
	private readonly bool _maintainIngredient;

	public ItemChanceHolder(int id, double chance = 100.0, long count = 1, byte enchantmentLevel = 0,
		bool maintainIngredient = false)
		: base(id, count)
	{
		_chance = chance;
		_enchantmentLevel = enchantmentLevel;
		_maintainIngredient = maintainIngredient;
	}

	/**
	 * Gets the chance.
	 * @return the drop chance of the item contained in this object
	 */
	public double getChance()
	{
		return _chance;
	}

	/**
	 * Gets the enchant level.
	 * @return the enchant level of the item contained in this object
	 */
	public byte getEnchantmentLevel()
	{
		return _enchantmentLevel;
	}

	public bool isMaintainIngredient()
	{
		return _maintainIngredient;
	}

	/**
	 * Calculates a cumulative chance of all given holders. If all holders' chance sum up to 100% or above, there is 100% guarantee a holder will be selected.
	 * @param holders list of holders to calculate chance from.
	 * @return {@code ItemChanceHolder} of the successful random roll or {@code null} if there was no lucky holder selected.
	 */
	public static ItemChanceHolder? getRandomHolder(List<ItemChanceHolder> holders)
	{
		double itemRandom = 100 * Rnd.nextDouble();
		foreach (ItemChanceHolder holder in holders)
		{
			double chance = holder.getChance();

			// Calculate chance
			if (chance > itemRandom)
				return holder;

			itemRandom -= chance;
		}

		return null;
	}

	public override string ToString()
	{
		return "[" + GetType().Name + "] ID: " + Id + ", count: " + getCount() + ", chance: " + _chance;
	}
}