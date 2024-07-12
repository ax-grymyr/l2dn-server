using System.Xml.Linq;
using L2Dn.Extensions;
using L2Dn.GameServer.Model.Holders;
using L2Dn.GameServer.Utilities;
using L2Dn.Utilities;
using NLog;

namespace L2Dn.GameServer.Data.Xml;

/**
 * @author Geremy
 */
public class PetExtractData: DataReaderBase
{
	private static readonly Logger LOGGER = LogManager.GetLogger(nameof(PetExtractData));
	// <Pet_Id, <Pet_Level, Cost>>
	private readonly Map<int, Map<int, PetExtractionHolder>> _extractionData = new();
	
	protected PetExtractData()
	{
		load();
	}
	
	public void load()
	{
		_extractionData.Clear();
		
		XDocument document = LoadXmlDocument(DataFileLocation.Data, "PetExtractData.xml");
		document.Elements("list").Elements("extraction").ForEach(parseElement);
		
		LOGGER.Info(GetType().Name + ": Loaded " + _extractionData.Count + " pet extraction data.");
	}

	private void parseElement(XElement element)
	{
		int petId = element.GetAttributeValueAsInt32("petId");
		int petLevel = element.GetAttributeValueAsInt32("petLevel");
		long extractExp = element.GetAttributeValueAsInt64("extractExp");
		int extractItem = element.GetAttributeValueAsInt32("extractItem");
		int defaultCostId = element.GetAttributeValueAsInt32("defaultCostId");
		int defaultCostCount = element.GetAttributeValueAsInt32("defaultCostCount");
		int extractCostId = element.GetAttributeValueAsInt32("extractCostId");
		int extractCostCount = element.GetAttributeValueAsInt32("extractCostCount");
		
		Map<int, PetExtractionHolder> data = _extractionData.get(petId);
		if (data == null)
		{
			data = new();
			_extractionData.put(petId, data);
		}

		data.put(petLevel,
			new PetExtractionHolder(petId, petLevel, extractExp, extractItem,
				new ItemHolder(defaultCostId, defaultCostCount), new ItemHolder(extractCostId, extractCostCount)));
	}

	public PetExtractionHolder getExtraction(int petId, int petLevel)
	{
		Map<int, PetExtractionHolder> map = _extractionData.get(petId);
		if (map == null)
		{
			LOGGER.Warn(GetType().Name + ": Missing pet extraction data: [PetId: " + petId + "] [PetLevel: " + petLevel + "]");
			return null;
		}
		return map.get(petLevel);
	}
	
	public static PetExtractData getInstance()
	{
		return SingletonHolder.INSTANCE;
	}
	
	private static class SingletonHolder
	{
		public static readonly PetExtractData INSTANCE = new();
	}
}