using System.Xml.Linq;
using L2Dn.Extensions;
using L2Dn.GameServer.Model.Holders;
using L2Dn.GameServer.Utilities;
using L2Dn.Utilities;
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
		string filePath = Path.Combine(Config.DATAPACK_ROOT_PATH, "data/ElementalSpiritData.xml");
		using FileStream stream = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read);
		XDocument document = XDocument.Load(stream);
		document.Elements("list").Elements("spirit").ForEach(parseSpirit);
		
		LOGGER.Info(GetType().Name + ": Loaded " + SPIRIT_DATA.size() + " elemental spirit templates.");
	}
	
	private void parseSpirit(XElement spiritNode)
	{
		byte type = spiritNode.Attribute("type").GetByte();
		byte stage = spiritNode.Attribute("stage").GetByte();
		int npcId = spiritNode.Attribute("npcId").GetInt32();
		int extractItem = spiritNode.Attribute("extractItem").GetInt32();
		int maxCharacteristics = spiritNode.Attribute("maxCharacteristics").GetInt32();
		ElementalSpiritTemplateHolder template = new ElementalSpiritTemplateHolder(type, stage, npcId, extractItem, maxCharacteristics);
		SPIRIT_DATA.computeIfAbsent(type, x => new()).put(stage, template);

		spiritNode.Elements("level").ForEach(levelNode =>
		{
			int level = levelNode.Attribute("id").GetInt32();
			int attack = levelNode.Attribute("atk").GetInt32();
			int defense = levelNode.Attribute("def").GetInt32();
			int criticalRate = levelNode.Attribute("critRate").GetInt32();
			int criticalDamage = levelNode.Attribute("critDam").GetInt32();
			long maxExperience = levelNode.Attribute("maxExp").GetInt64();
			template.addLevelInfo(level, attack, defense, criticalRate, criticalDamage, maxExperience);
		});
		
		spiritNode.Elements("itemToEvolve").ForEach(itemNode =>
		{
			int itemId = itemNode.Attribute("id").GetInt32();
			int count = itemNode.Attribute("count").GetInt32(1);
			template.addItemToEvolve(itemId, count);
		});
		
		spiritNode.Elements("absorbItem").ForEach(absorbItemNode =>
		{
			int itemId = absorbItemNode.Attribute("id").GetInt32();
			int experience = absorbItemNode.Attribute("experience").GetInt32();
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