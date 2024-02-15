using System.Xml.Linq;
using L2Dn.Extensions;
using L2Dn.Utilities;
using NLog;

namespace L2Dn.GameServer.Data.Xml;

/**
 * This class holds the Player Xp Percent Lost Data for each level for players.
 * @author Zealar
 */
public class PlayerXpPercentLostData: DataReaderBase
{
	private static readonly Logger LOGGER = LogManager.GetLogger(nameof(PlayerXpPercentLostData));
	
	private readonly int _maxlevel;
	private readonly double[] _playerXpPercentLost;
	
	protected PlayerXpPercentLostData()
	{
		_maxlevel = ExperienceData.getInstance().getMaxLevel();
		_playerXpPercentLost = new double[_maxlevel + 1];
		Array.Fill(_playerXpPercentLost, 1.0);
		
		load();
	}
	
	private void load()
	{
		XDocument document = LoadXmlDocument(DataFileLocation.Data, "stats/chars/playerXpPercentLost.xml");
		document.Elements("list").Elements("xpLost").ForEach(parseElement);
	}

	private void parseElement(XElement element)
	{
		int level = element.Attribute("level").GetInt32();
		if (level > _maxlevel)
			return;

		double val = element.Attribute("val").GetDouble();
		_playerXpPercentLost[level] = val;
	}

	public double getXpPercent(int level)
	{
		if (level > _maxlevel)
		{
			LOGGER.Warn("Require to high level inside PlayerXpPercentLostData (" + level + ")");
			return _playerXpPercentLost[_maxlevel];
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