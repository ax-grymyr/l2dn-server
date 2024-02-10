using L2Dn.GameServer.Model;
using L2Dn.GameServer.Utilities;
using NLog;

namespace L2Dn.GameServer.Data.Xml;

/**
 * @author Berezkin Nikolay
 */
public class SubjugationGacha
{
	private static readonly Logger LOGGER = LogManager.GetLogger(nameof(SubjugationGacha));
	
	private static readonly Map<int, Map<int, Double>> _subjugations = new();
	
	public SubjugationGacha()
	{
		load();
	}
	
	public void load()
	{
		_subjugations.clear();
		parseDatapackFile("data/SubjugationGacha.xml");
		LOGGER.Info(GetType().Name + ": Loaded " + _subjugations.size() + " data.");
	}
	
	public void parseDocument(Document doc, File f)
	{
		forEach(doc, "list", listNode => forEach(listNode, "purge", purgeNode =>
		{
			StatSet set = new StatSet(parseAttributes(purgeNode));
			int category = set.getInt("category");
			Map<int, Double> items = new();
			forEach(purgeNode, "item", npcNode =>
			{
				StatSet stats = new StatSet(parseAttributes(npcNode));
				int itemId = stats.getInt("id");
				double rate = stats.getDouble("rate");
				items.put(itemId, rate);
			});
			_subjugations.put(category, items);
		}));
	}
	
	public Map<int, Double> getSubjugation(int category)
	{
		return _subjugations.get(category);
	}
	
	public static SubjugationGacha getInstance()
	{
		return SingletonHolder.INSTANCE;
	}
	
	private static class SingletonHolder
	{
		public static readonly SubjugationGacha INSTANCE = new();
	}
}