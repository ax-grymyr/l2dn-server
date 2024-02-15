using System.Xml.Linq;
using L2Dn.Extensions;
using L2Dn.GameServer.Enums;
using L2Dn.GameServer.Model.Holders;
using L2Dn.GameServer.Utilities;
using L2Dn.Utilities;
using NLog;

namespace L2Dn.GameServer.Data.Xml;

/**
 * @author Serenitty
 */
public class MagicLampData: DataReaderBase
{
	private static readonly Logger LOGGER = LogManager.GetLogger(nameof(MagicLampData));
	private static readonly List<MagicLampDataHolder> LAMPS = new();
	
	protected MagicLampData()
	{
		load();
	}
	
	public void load()
	{
		LAMPS.Clear();
		
		XDocument document = LoadXmlDocument(DataFileLocation.Data, "MagicLampData.xml");
		document.Elements("list").Elements("levelRange").ForEach(parseElement);
		
		LOGGER.Info("MagicLampData: Loaded " + LAMPS.size() + " magic lamps exp types.");
	}

	private void parseElement(XElement element)
	{
		int minLevel = element.Attribute("fromLevel").GetInt32();
		int maxLevel = element.Attribute("toLevel").GetInt32();
		
		element.Elements("lamp").ForEach(el =>
		{
			LampType type = el.Attribute("type").GetEnum<LampType>();
			long exp = el.Attribute("exp").GetInt64();
			long sp = el.Attribute("sp").GetInt64();
			double chance = el.Attribute("chance").GetDouble();
			
			LAMPS.add(new MagicLampDataHolder(type, exp, sp, chance, minLevel, maxLevel));
		});
	}

	public List<MagicLampDataHolder> getLamps()
	{
		return LAMPS;
	}
	
	public static MagicLampData getInstance()
	{
		return Singleton.INSTANCE;
	}
	
	private static class Singleton
	{
		public static readonly MagicLampData INSTANCE = new();
	}
}
