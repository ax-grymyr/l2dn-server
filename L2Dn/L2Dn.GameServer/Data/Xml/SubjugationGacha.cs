using System.Xml.Linq;
using L2Dn.Extensions;
using L2Dn.GameServer.Utilities;
using L2Dn.Utilities;
using NLog;

namespace L2Dn.GameServer.Data.Xml;

/**
 * @author Berezkin Nikolay
 */
public class SubjugationGacha: DataReaderBase
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
		
		XDocument document = LoadXmlDocument(DataFileLocation.Data, "SubjugationGacha.xml");
		document.Elements("list").Elements("purge").ForEach(parseElement);
		
		LOGGER.Info(GetType().Name + ": Loaded " + _subjugations.size() + " data.");
	}

	private void parseElement(XElement element)
	{
		int category = element.Attribute("category").GetInt32();
		Map<int, Double> items = new();
		element.Elements("item").ForEach(el =>
		{
			int itemId = el.Attribute("id").GetInt32();
			double rate = el.Attribute("rate").GetDouble();
			items.put(itemId, rate);
		});

		_subjugations.put(category, items);
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