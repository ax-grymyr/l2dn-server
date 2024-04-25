using L2Dn.GameServer.Utilities;
using L2Dn.Utilities;

namespace L2Dn.GameServer.Model.Options;

/**
 * @author Pere, Mobius
 */
public class OptionDataCategory
{
	private readonly Map<Options, Double> _options;
	private readonly Set<int> _itemIds;
	private readonly double _chance;

	public OptionDataCategory(Map<Options, Double> options, Set<int> itemIds, double chance)
	{
		_options = options;
		_itemIds = itemIds;
		_chance = chance;
	}

	public Options getRandomOptions()
	{
		Options result = null;
		do
		{
			double random = Rnd.nextDouble() * 100.0;
			foreach (var entry in _options)
			{
				if (entry.Value >= random)
				{
					result = entry.Key;
					break;
				}

				random -= entry.Value;
			}
		} while (result == null);

		return result;
	}

	public Set<int> getItemIds()
	{
		return _itemIds;
	}

	public double getChance()
	{
		return _chance;
	}
}