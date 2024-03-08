using System.Xml.Linq;
using L2Dn.Extensions;
using L2Dn.GameServer.Enums;
using L2Dn.GameServer.Model.Holders;
using L2Dn.GameServer.Utilities;
using L2Dn.Utilities;
using NLog;

namespace L2Dn.GameServer.Data.Xml;

/**
 * @author JoeAlisson
 */
public class ElementalSpiritData: DataReaderBase
{
	private static readonly Logger LOGGER = LogManager.GetLogger(nameof(ElementalSpiritData));
	
	public static readonly float FRAGMENT_XP_CONSUME = 50000.0f;
	public const int TALENT_INIT_FEE = 50000;
	public static int[] EXTRACT_FEES =
	{
		100000,
		200000,
		300000,
		600000,
		1500000
	};
	
	private static readonly Map<ElementalType, Map<byte, ElementalSpiritTemplateHolder>> SPIRIT_DATA = new();
	
	protected ElementalSpiritData()
	{
		load();
	}
	
	public void load()
	{
		XDocument document = LoadXmlDocument(DataFileLocation.Data, "ElementalSpiritData.xml");
		document.Elements("list").Elements("spirit").ForEach(parseSpirit);
		
		LOGGER.Info(GetType().Name + ": Loaded " + SPIRIT_DATA.size() + " elemental spirit templates.");
	}
	
	private void parseSpirit(XElement spiritNode)
	{
		ElementalType type = (ElementalType)spiritNode.Attribute("type").GetByte();
		byte stage = spiritNode.Attribute("stage").GetByte();
		int npcId = spiritNode.GetAttributeValueAsInt32("npcId");
		int extractItem = spiritNode.GetAttributeValueAsInt32("extractItem");
		int maxCharacteristics = spiritNode.GetAttributeValueAsInt32("maxCharacteristics");
		ElementalSpiritTemplateHolder template = new ElementalSpiritTemplateHolder(type, stage, npcId, extractItem, maxCharacteristics);
		SPIRIT_DATA.computeIfAbsent(type, x => new()).put(stage, template);

		spiritNode.Elements("level").ForEach(levelNode =>
		{
			int level = levelNode.GetAttributeValueAsInt32("id");
			int attack = levelNode.GetAttributeValueAsInt32("atk");
			int defense = levelNode.GetAttributeValueAsInt32("def");
			int criticalRate = levelNode.GetAttributeValueAsInt32("critRate");
			int criticalDamage = levelNode.GetAttributeValueAsInt32("critDam");
			long maxExperience = levelNode.GetAttributeValueAsInt64("maxExp");
			template.addLevelInfo(level, attack, defense, criticalRate, criticalDamage, maxExperience);
		});
		
		spiritNode.Elements("itemToEvolve").ForEach(itemNode =>
		{
			int itemId = itemNode.GetAttributeValueAsInt32("id");
			int count = itemNode.Attribute("count").GetInt32(1);
			template.addItemToEvolve(itemId, count);
		});
		
		spiritNode.Elements("absorbItem").ForEach(absorbItemNode =>
		{
			int itemId = absorbItemNode.GetAttributeValueAsInt32("id");
			int experience = absorbItemNode.GetAttributeValueAsInt32("experience");
			template.addAbsorbItem(itemId, experience);
		});
	}
	
	public ElementalSpiritTemplateHolder getSpirit(ElementalType type, byte stage)
	{
		if (SPIRIT_DATA.containsKey(type))
		{
			return SPIRIT_DATA.get(type).get(stage);
		}
		return null;
	}
	
	public static ElementalSpiritData getInstance()
	{
		return Singleton.INSTANCE;
	}
	
	private static class Singleton
	{
		public static readonly ElementalSpiritData INSTANCE = new ElementalSpiritData();
	}
}