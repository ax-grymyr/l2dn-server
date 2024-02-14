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
public class PetExtractData
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
		_extractionData.clear();
		
		string filePath = Path.Combine(Config.DATAPACK_ROOT_PATH, "data/PetExtractData.xml");
		using FileStream stream = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read);
		XDocument document = XDocument.Load(stream);
		document.Elements("list").Elements("extraction").ForEach(parseElement);
		
		LOGGER.Info(GetType().Name + ": Loaded " + _extractionData.size() + " pet extraction data.");
	}

	private void parseElement(XElement element)
	{
		int petId = element.Attribute("petId").GetInt32();
		int petLevel = element.Attribute("petLevel").GetInt32();
		long extractExp = element.Attribute("extractExp").GetInt64();
		int extractItem = element.Attribute("extractItem").GetInt32();
		int defaultCostId = element.Attribute("defaultCostId").GetInt32();
		int defaultCostCount = element.Attribute("defaultCostCount").GetInt32();
		int extractCostId = element.Attribute("extractCostId").GetInt32();
		int extractCostCount = element.Attribute("extractCostCount").GetInt32();
		
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