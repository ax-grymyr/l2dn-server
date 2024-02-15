using System.Reflection.Metadata;
using System.Runtime.CompilerServices;
using System.Xml.Linq;
using L2Dn.Extensions;
using L2Dn.GameServer.Utilities;
using L2Dn.Utilities;
using NLog;

namespace L2Dn.GameServer.Data.Xml;

/**
 * @author UnAfraid
 */
public class KarmaData: DataReaderBase
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
		
		XDocument document = LoadXmlDocument(DataFileLocation.Data, "stats/chars/pcKarmaIncrease.xml");
		document.Elements("pcKarmaIncrease").Elements("increase").ForEach(parseElement);

		LOGGER.Info(GetType().Name + ": Loaded " + _karmaTable.size() + " karma modifiers.");
	}

	private void parseElement(XElement element)
	{
		int level = element.Attribute("lvl").GetInt32();
		if (level >= Config.PLAYER_MAXIMUM_LEVEL)
			return;

		double val = element.Attribute("val").GetDouble();
		_karmaTable.put(level, val);
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