using System.Xml.Linq;
using L2Dn.Extensions;
using L2Dn.GameServer.Model.Holders;
using L2Dn.GameServer.Utilities;
using L2Dn.Utilities;
using NLog;

namespace L2Dn.GameServer.Data.Xml;

/**
 * @author Berezkin Nikolay
 */
public class SubjugationData: DataReaderBase
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
		
		XDocument document = LoadXmlDocument(DataFileLocation.Data, "SubjugationData.xml");
		document.Elements("list").Elements("purge").ForEach(parseElement);
		
		LOGGER.Info(GetType().Name + ": Loaded " + _subjugations.size() + " data.");
	}

	private void parseElement(XElement element)
	{
		int category = element.GetAttributeValueAsInt32("category");
		List<int[]> hottimes = element.GetAttributeValueAsString("hottimes").Split(";")
			.Select(it => it.Split("-").Select(int.Parse).ToArray()).ToList();
		
		Map<int, int> npcs = new();
		element.Elements("npc").ForEach(el =>
		{
			int npcId = el.GetAttributeValueAsInt32("id");
			int points = el.GetAttributeValueAsInt32("points");
			npcs.put(npcId, points);
		});

		_subjugations.add(new SubjugationHolder(category, hottimes, npcs));
	}

	public SubjugationHolder getSubjugation(int category)
	{
		return _subjugations.FirstOrDefault(it => it.getCategory() == category);
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