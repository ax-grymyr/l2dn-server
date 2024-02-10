using L2Dn.GameServer.Model.Holders;
using L2Dn.GameServer.Utilities;
using NLog;

namespace L2Dn.GameServer.Data.Xml;

/**
 * @author JoeAlisson
 */
public class ElementalSpiritData
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
	
	private static readonly Map<Byte, Map<Byte, ElementalSpiritTemplateHolder>> SPIRIT_DATA = new();
	
	protected ElementalSpiritData()
	{
		load();
	}
	
	public void load()
	{
		parseDatapackFile("data/ElementalSpiritData.xml");
		LOGGER.Info(GetType().Name + ": Loaded " + SPIRIT_DATA.size() + " elemental spirit templates.");
	}
	
	public void parseDocument(Document doc, File f)
	{
		forEach(doc, "list", list => forEach(list, "spirit", this::parseSpirit));
	}
	
	private void parseSpirit(Node spiritNode)
	{
		NamedNodeMap attributes = spiritNode.getAttributes();
		byte type = parseByte(attributes, "type");
		byte stage = parseByte(attributes, "stage");
		int npcId = parseInteger(attributes, "npcId");
		int extractItem = parseInteger(attributes, "extractItem");
		int maxCharacteristics = parseInteger(attributes, "maxCharacteristics");
		ElementalSpiritTemplateHolder template = new ElementalSpiritTemplateHolder(type, stage, npcId, extractItem, maxCharacteristics);
		SPIRIT_DATA.computeIfAbsent(type, HashMap::new).put(stage, template);
		
		forEach(spiritNode, "level", levelNode =>
		{
			NamedNodeMap levelInfo = levelNode.getAttributes();
			int level = parseInteger(levelInfo, "id");
			int attack = parseInteger(levelInfo, "atk");
			int defense = parseInteger(levelInfo, "def");
			int criticalRate = parseInteger(levelInfo, "critRate");
			int criticalDamage = parseInteger(levelInfo, "critDam");
			long maxExperience = Parse(levelInfo, "maxExp");
			template.addLevelInfo(level, attack, defense, criticalRate, criticalDamage, maxExperience);
		});
		
		forEach(spiritNode, "itemToEvolve", itemNode =>
		{
			NamedNodeMap itemInfo = itemNode.getAttributes();
			int itemId = parseInteger(itemInfo, "id");
			int count = parseInteger(itemInfo, "count", 1);
			template.addItemToEvolve(itemId, count);
		});
		
		forEach(spiritNode, "absorbItem", absorbItemNode =>
		{
			NamedNodeMap absorbInfo = absorbItemNode.getAttributes();
			int itemId = parseInteger(absorbInfo, "id");
			int experience = parseInteger(absorbInfo, "experience");
			template.addAbsorbItem(itemId, experience);
		});
	}
	
	public ElementalSpiritTemplateHolder getSpirit(byte type, byte stage)
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