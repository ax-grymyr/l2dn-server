using System.Collections.Immutable;
using L2Dn.Extensions;
using L2Dn.Model.Xml;
using NLog;

namespace L2Dn.GameServer.Data.Xml;

/**
 * @author UnAfraid
 */
public class KarmaData: DataReaderBase
{
	private static readonly Logger _logger = LogManager.GetLogger(nameof(KarmaData));
	private static ImmutableArray<double> _karmaTable = ImmutableArray<double>.Empty;
	
	private KarmaData()
	{
		load();
	}
	
	public void load()
	{
		Dictionary<int, double> values =
			LoadXmlDocument<XmlPcKarmaIncreaseData>(DataFileLocation.Data, "stats/chars/pcKarmaIncrease.xml")
				.Levels.ToDictionary(el => el.Level, el => el.Value);

		_karmaTable = values.ToValueArray().ToImmutableArray();
		
		_logger.Info(GetType().Name + ": Loaded " + values.Count + " karma modifiers.");
	}

	/**
	 * @param level
	 * @return {@code double} modifier used to calculate karma lost upon death.
	 */
	public double getMultiplier(int level)
	{
		return _karmaTable[level];
	}
	
	/**
	 * Gets the single instance of KarmaData.
	 * @return single instance of KarmaData
	 */
	public static KarmaData getInstance()
	{
		return SingletonHolder.INSTANCE;
	}
	
	private static class SingletonHolder
	{
		public static readonly KarmaData INSTANCE = new();
	}
}