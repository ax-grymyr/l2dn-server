using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Holders;
using L2Dn.GameServer.Utilities;
using NLog;

namespace L2Dn.GameServer.Data.Xml;

/**
 * @author Berezkin Nikolay
 */
public class SubjugationData
{
	private static readonly Logger LOGGER = LogManager.GetLogger(nameof(SubjugationData));
	
	private static readonly List<SubjugationHolder> _subjugations = new();
	
	public SubjugationData()
	{
		load();
	}
	
	public void load()
	{
		_subjugations.Clear();
		parseDatapackFile("data/SubjugationData.xml");
		LOGGER.Info(GetType().Name + ": Loaded " + _subjugations.size() + " data.");
	}
	
	public void parseDocument(Document doc, File f)
	{
		forEach(doc, "list", listNode => forEach(listNode, "purge", purgeNode =>
		{
			StatSet set = new StatSet(parseAttributes(purgeNode));
			int category = set.getInt("category");
			List<int[]> hottimes = Arrays.stream(set.getString("hottimes").split(";")).map(it => Arrays.stream(it.split("-")).mapToInt(int::Parse).toArray()).collect(Collectors.toList());
			Map<int, int> npcs = new();
			forEach(purgeNode, "npc", npcNode =>
			{
				StatSet stats = new StatSet(parseAttributes(npcNode));
				int npcId = stats.getInt("id");
				int points = stats.getInt("points");
				npcs.put(npcId, points);
			});
			_subjugations.add(new SubjugationHolder(category, hottimes, npcs));
		}));
	}
	
	public SubjugationHolder getSubjugation(int category)
	{
		return _subjugations.stream().filter(it => it.getCategory() == category).findFirst().orElse(null);
	}
	
	public static SubjugationData getInstance()
	{
		return SingletonHolder.INSTANCE;
	}
	
	private static class SingletonHolder
	{
		public static readonly SubjugationData INSTANCE = new();
	}
}