using System.Runtime.CompilerServices;
using L2Dn.GameServer.Utilities;
using NLog;

namespace L2Dn.GameServer.Data.Xml;

/**
 * @author UnAfraid
 */
public class KarmaData
{
	private static readonly Logger LOGGER = LogManager.GetLogger(nameof(KarmaData));
	
	private readonly Map<int, Double> _karmaTable = new();
	
	public KarmaData()
	{
		load();
	}
	
	[MethodImpl(MethodImplOptions.Synchronized)] 
	public void load()
	{
		_karmaTable.clear();
		parseDatapackFile("data/stats/chars/pcKarmaIncrease.xml");
		LOGGER.Info(GetType().Name + ": Loaded " + _karmaTable.size() + " karma modifiers.");
	}
	
	public void parseDocument(Document doc, File f)
	{
		for (Node n = doc.getFirstChild(); n != null; n = n.getNextSibling())
		{
			if ("pcKarmaIncrease".equalsIgnoreCase(n.getNodeName()))
			{
				for (Node d = n.getFirstChild(); d != null; d = d.getNextSibling())
				{
					if ("increase".equalsIgnoreCase(d.getNodeName()))
					{
						NamedNodeMap attrs = d.getAttributes();
						int level = parseInteger(attrs, "lvl");
						if (level >= Config.PLAYER_MAXIMUM_LEVEL)
						{
							break;
						}
						_karmaTable.put(level, parseDouble(attrs, "val"));
					}
				}
			}
		}
	}
	
	/**
	 * @param level
	 * @return {@code double} modifier used to calculate karma lost upon death.
	 */
	public double getMultiplier(int level)
	{
		return _karmaTable.get(level);
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