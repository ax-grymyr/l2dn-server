using L2Dn.GameServer.Utilities;
using NLog;

namespace L2Dn.GameServer.Data.Xml;

/**
 * This class holds the Player Xp Percent Lost Data for each level for players.
 * @author Zealar
 */
public class PlayerXpPercentLostData
{
	private static readonly Logger LOGGER = LogManager.GetLogger(nameof(PlayerXpPercentLostData));
	
	private readonly int _maxlevel = ExperienceData.getInstance().getMaxLevel();
	private readonly double[] _playerXpPercentLost = new double[_maxlevel + 1];
	
	protected PlayerXpPercentLostData()
	{
		Arrays.fill(_playerXpPercentLost, 1.0);
		load();
	}
	
	public void load()
	{
		parseDatapackFile("data/stats/chars/playerXpPercentLost.xml");
	}
	
	public void parseDocument(Document doc, File f)
	{
		for (Node n = doc.getFirstChild(); n != null; n = n.getNextSibling())
		{
			if ("list".equalsIgnoreCase(n.getNodeName()))
			{
				for (Node d = n.getFirstChild(); d != null; d = d.getNextSibling())
				{
					if ("xpLost".equalsIgnoreCase(d.getNodeName()))
					{
						NamedNodeMap attrs = d.getAttributes();
						int level = parseInteger(attrs, "level");
						if (level > _maxlevel)
						{
							break;
						}
						_playerXpPercentLost[level] = parseDouble(attrs, "val");
					}
				}
			}
		}
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