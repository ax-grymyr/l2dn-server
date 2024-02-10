using L2Dn.GameServer.Model.Holders;
using NLog;

namespace L2Dn.GameServer.Data.Xml;

/**
 * @author Serenitty
 */
public class MagicLampData
{
	private static readonly Logger LOGGER = LogManager.GetLogger(nameof(MagicLampData));
	private static readonly List<MagicLampDataHolder> LAMPS = new();
	
	protected MagicLampData()
	{
		load();
	}
	
	@Override
	public void load()
	{
		LAMPS.clear();
		parseDatapackFile("data/MagicLampData.xml");
		LOGGER.Info("MagicLampData: Loaded " + LAMPS.size() + " magic lamps exp types.");
	}
	
	public void parseDocument(Document doc, File f)
	{
		NodeList list = doc.getFirstChild().getChildNodes();
		for (int i = 0; i < list.getLength(); i++)
		{
			Node n = list.item(i);
			if ("levelRange".equalsIgnoreCase(n.getNodeName()))
			{
				int minLevel = parseInteger(n.getAttributes(), "fromLevel");
				int maxLevel = parseInteger(n.getAttributes(), "toLevel");
				NodeList lamps = n.getChildNodes();
				for (int j = 0; j < lamps.getLength(); j++)
				{
					Node d = lamps.item(j);
					if ("lamp".equalsIgnoreCase(d.getNodeName()))
					{
						NamedNodeMap attrs = d.getAttributes();
						StatSet set = new StatSet();
						set.set("type", parseString(attrs, "type"));
						set.set("exp", parseInteger(attrs, "exp"));
						set.set("sp", parseInteger(attrs, "sp"));
						set.set("chance", parseInteger(attrs, "chance"));
						set.set("minLevel", minLevel);
						set.set("maxLevel", maxLevel);
						LAMPS.add(new MagicLampDataHolder(set));
					}
				}
			}
		}
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
