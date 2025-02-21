using L2Dn.GameServer.Utilities;
using L2Dn.Utilities;

namespace L2Dn.GameServer.Model.Options;

/**
 * @author Pere, Mobius
 */
public sealed class OptionDataGroup(List<OptionDataCategory> categories)
{
    public Options? getRandomEffect(int itemId)
	{
		List<OptionDataCategory> exclutions = [];
		Options? result = null;
		do
		{
			double random = Rnd.nextDouble() * 100.0;
			foreach (OptionDataCategory category in categories)
			{
				if (!category.getItemIds().isEmpty() && !category.getItemIds().Contains(itemId))
				{
					if (!exclutions.Contains(category))
					{
						exclutions.Add(category);
					}

					continue;
				}

				if (category.getChance() >= random)
				{
					result = category.getRandomOptions();
					break;
				}

				random -= category.getChance();
			}
		} while (result == null && exclutions.Count < categories.Count);

		return result;
	}
}