using System.Collections.Immutable;
using L2Dn.Extensions;
using L2Dn.GameServer.StaticData;
using L2Dn.GameServer.Utilities;
using L2Dn.Model.Xml;
using NLog;

namespace L2Dn.GameServer.Data.Xml;

/**
 * This class holds the Player Xp Percent Lost Data for each level for players.
 * @author Zealar
 */
public class PlayerXpPercentLostData: DataReaderBase
{
	private static readonly Logger _logger = LogManager.GetLogger(nameof(PlayerXpPercentLostData));
	private static ImmutableArray<double> _playerXpPercentLost = ImmutableArray<double>.Empty;
	
	private PlayerXpPercentLostData()
	{
		load();
	}
	
	private void load()
	{
		_playerXpPercentLost =
			LoadXmlDocument<XmlPlayerXpPercentLostData>(DataFileLocation.Data, "stats/chars/playerXpPercentLost.xml")
				.Levels
				.ToDictionary(x => x.Level, x => x.Value)
				.ToValueArray()
				.ToImmutableArray();
	}

	public double getXpPercent(int level)
	{
		if (level > _playerXpPercentLost.Length)
		{
			_logger.Warn("Require to high level inside PlayerXpPercentLostData (" + level + ")");
			return 1.0;
		}
		
		return _playerXpPercentLost[level];
	}
	
	/**
	 * Gets the single instance of PlayerXpPercentLostData.
	 * @return single instance of PlayerXpPercentLostData.
	 */
	public static PlayerXpPercentLostData getInstance()
	{
		return SingletonHolder.INSTANCE;
	}
	
	private static class SingletonHolder
	{
		public static readonly PlayerXpPercentLostData INSTANCE = new();
	}
}