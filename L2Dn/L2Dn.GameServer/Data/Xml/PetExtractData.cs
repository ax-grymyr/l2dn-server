using L2Dn.GameServer.Model.Holders;
using L2Dn.GameServer.Utilities;
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
		parseDatapackFile("data/PetExtractData.xml");
		LOGGER.Info(GetType().Name + ": Loaded " + _extractionData.size() + " pet extraction data.");
	}
	
	public void parseDocument(Document doc, File f)
	{
		for (Node n = doc.getFirstChild(); n != null; n = n.getNextSibling())
		{
			if ("list".equalsIgnoreCase(n.getNodeName()))
			{
				for (Node d = n.getFirstChild(); d != null; d = d.getNextSibling())
				{
					if ("extraction".equalsIgnoreCase(d.getNodeName()))
					{
						NamedNodeMap attrs = d.getAttributes();
						int petId = parseInteger(attrs, "petId");
						int petLevel = parseInteger(attrs, "petLevel");
						long extractExp = Parse(attrs, "extractExp");
						int extractItem = parseInteger(attrs, "extractItem");
						int defaultCostId = parseInteger(attrs, "defaultCostId");
						int defaultCostCount = parseInteger(attrs, "defaultCostCount");
						int extractCostId = parseInteger(attrs, "extractCostId");
						int extractCostCount = parseInteger(attrs, "extractCostCount");
						Map<int, PetExtractionHolder> data = _extractionData.get(petId);
						if (data == null)
						{
							data = new();
							_extractionData.put(petId, data);
						}
						data.put(petLevel, new PetExtractionHolder(petId, petLevel, extractExp, extractItem, new ItemHolder(defaultCostId, defaultCostCount), new ItemHolder(extractCostId, extractCostCount)));
					}
				}
			}
		}
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