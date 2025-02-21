using L2Dn.GameServer.Utilities;
using L2Dn.Utilities;

namespace L2Dn.GameServer.Model.Options;

/**
 * @author Pere, Mobius
 */
public sealed class OptionDataCategory(Map<Options, double> options, Set<int> itemIds, double chance)
{
    public Options getRandomOptions()
	{
		Options? result = null;
		do
		{
			double random = Rnd.nextDouble() * 100.0;
			foreach (KeyValuePair<Options, double> entry in options)
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
		return itemIds;
	}

	public double getChance()
	{
		return chance;
	}
}