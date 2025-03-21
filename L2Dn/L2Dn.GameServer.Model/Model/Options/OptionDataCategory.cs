using L2Dn.GameServer.Dto;
using L2Dn.GameServer.Utilities;
using L2Dn.Utilities;

namespace L2Dn.GameServer.Model.Options;

/**
 * @author Pere, Mobius
 */
public sealed class OptionDataCategory(Map<Option, double> options, Set<int> itemIds, double chance)
{
    public Option getRandomOptions()
	{
		Option? result = null;
		do
		{
			double random = Rnd.nextDouble() * 100.0;
			foreach (KeyValuePair<Option, double> entry in options)
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