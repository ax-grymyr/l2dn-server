using L2Dn.GameServer.Utilities;

namespace L2Dn.GameServer.Model.Options;

/**
 * @author Pere, Mobius
 */
public class OptionDataGroup
{
	private readonly List<OptionDataCategory> _categories;

	public OptionDataGroup(List<OptionDataCategory> categories)
	{
		_categories = categories;
	}

	public Options getRandomEffect(int itemId)
	{
		List<OptionDataCategory> exclutions = new();
		Options result = null;
		do
		{
			double random = Rnd.nextDouble() * 100.0;
			foreach (OptionDataCategory category in _categories)
			{
				if (!category.getItemIds().isEmpty() && !category.getItemIds().Contains(itemId))
				{
					if (!exclutions.Contains(category))
					{
						exclutions.add(category);
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
		} while ((result == null) && (exclutions.size() < _categories.size()));

		return result;
	}
}