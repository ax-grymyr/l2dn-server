using L2Dn.GameServer.Model.Actor;

namespace L2Dn.GameServer.Model.Stats;

/**
 * @author DS
 */
public enum BaseStat
{
	STR = Stat.STAT_STR,
	INT = Stat.STAT_INT,
	DEX = Stat.STAT_DEX,
	WIT = Stat.STAT_WIT,
	CON = Stat.STAT_CON,
	MEN = Stat.STAT_MEN
}

public static class BaseStatUtil
{
	static BaseStatUtil()
	{
		// TODO: load bonus values
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
			return (int) creature.getStat().getValue(stat.GetStat());
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
			return _bonus[value];
		}
		
		return 1;
	}
	
	// static
	// {
	// 	new IXmlReader()
	// 	{
	// 		final Logger LOGGER = Logger.getLogger(BaseStat.class.getName());
	// 		
	// 		@Override
	// 		public void load()
	// 		{
	// 			parseDatapackFile("data/stats/statBonus.xml");
	// 		}
	// 		
	// 		@Override
	// 		public void parseDocument(Document doc, File f)
	// 		{
	// 			forEach(doc, "list", listNode -> forEach(listNode, IXmlReader::isNode, statNode ->
	// 			{
	// 				final BaseStat baseStat;
	// 				try
	// 				{
	// 					baseStat = valueOf(statNode.getNodeName());
	// 				}
	// 				catch (Exception e)
	// 				{
	// 					LOGGER.severe("Invalid base stats type: " + statNode.getNodeValue() + ", skipping");
	// 					return;
	// 				}
	// 				
	// 				forEach(statNode, "stat", statValue ->
	// 				{
	// 					final NamedNodeMap attrs = statValue.getAttributes();
	// 					final int val = parseInteger(attrs, "value");
	// 					final double bonus = parseDouble(attrs, "bonus");
	// 					baseStat.setValue(val, bonus);
	// 				});
	// 			}));
	// 		}
	// 	}.load();
	}
}
