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
	
	private static readonly Map<int, Map<int, double>> _subjugations = new();
	
	public SubjugationGacha()
	{
		load();
	}
	
	public void load()
	{
		_subjugations.Clear();
		
		XDocument document = LoadXmlDocument(DataFileLocation.Data, "SubjugationGacha.xml");
		document.Elements("list").Elements("purge").ForEach(parseElement);
		
		LOGGER.Info(GetType().Name + ": Loaded " + _subjugations.Count + " data.");
	}

	private void parseElement(XElement element)
	{
		int category = element.GetAttributeValueAsInt32("category");
		Map<int, double> items = new();
		element.Elements("item").ForEach(el =>
		{
			int itemId = el.GetAttributeValueAsInt32("id");
			double rate = el.GetAttributeValueAsDouble("rate");
			items.put(itemId, rate);
		});

		_subjugations.put(category, items);
	}

	public Map<int, double> getSubjugation(int category)
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