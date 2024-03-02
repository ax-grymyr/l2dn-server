using System.Collections.Immutable;
using System.Xml.Linq;
using L2Dn.GameServer.Model.Actor;
using L2Dn.Utilities;

namespace L2Dn.GameServer.Model.Stats;

/**
 * @author DS
 */
public enum BaseStat
{
	STR = Stat.STAT_STR,
	CON = Stat.STAT_CON,
	DEX = Stat.STAT_DEX,
	INT = Stat.STAT_INT,
	WIT = Stat.STAT_WIT,
	MEN = Stat.STAT_MEN
}

public static class BaseStatUtil
{
	public const int MAX_STAT_VALUE = 201;

	private static readonly ImmutableArray<ImmutableArray<double>> _bonuses;

	static BaseStatUtil()
	{
		// TODO: load bonus values
		string filePath = Path.Combine(Config.DATAPACK_ROOT_PATH, "stats/statBonus.xml");
		using FileStream stream = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read);
		XDocument document = XDocument.Load(stream);

		ImmutableArray<double>[] arrays = new ImmutableArray<double>[6];
		foreach (XElement element in document.Elements("list").Elements())
		{
			BaseStat baseStat = Enum.Parse<BaseStat>(element.Name.LocalName);

			Dictionary<int, double> values = new Dictionary<int, double>();
			foreach (XElement el in element.Elements("stat"))
			{
				int value = el.Attribute("value").GetInt32();
				double bonus = el.Attribute("bonus").GetDouble();
				values.Add(value, bonus);
			}

			int max = values.Count == 0 ? -1 : values.Keys.Max();
			double[] arr = new double[max + 1];
			foreach (var pair in values)
				arr[pair.Key] = pair.Value;

			arrays[baseStat - BaseStat.STR] = arr.ToImmutableArray();
		}

		_bonuses = arrays.ToImmutableArray();
	}

	public static Stat GetStat(this BaseStat stat)
	{
		return (Stat)stat;
	}

	public static BaseStat GetStat(this Stat stat)
	{
		BaseStat baseStat = (BaseStat)stat;
		if (Enum.IsDefined(baseStat))
			return baseStat;

		throw new ArgumentOutOfRangeException(nameof(stat), "Unknown base stat '" + stat + "' for enum BaseStats");
	}

	public static int calcValue(this BaseStat stat, Creature creature)
	{
		if ((creature != null))
		{
			// return (int) Math.min(_stat.finalize(creature, Optional.empty()), MAX_STAT_VALUE - 1);
			return (int)creature.getStat().getValue(stat.GetStat());
		}

		return 0;
	}

	public static double calcBonus(this BaseStat stat, Creature creature)
	{
		if (creature != null)
		{
			int value = stat.calcValue(creature);
			if (value < 1)
			{
				return 1;
			}

			return _bonuses[stat - BaseStat.STR][value];
		}

		return 1;
	}
}