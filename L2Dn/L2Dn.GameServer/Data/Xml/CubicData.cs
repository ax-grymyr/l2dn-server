using L2Dn.GameServer.Model.Actor.Templates;
using L2Dn.GameServer.Model.Cubics;
using L2Dn.GameServer.Model.Cubics.Conditions;
using L2Dn.GameServer.Utilities;
using NLog;

namespace L2Dn.GameServer.Data.Xml;

/**
 * @author UnAfraid
 */
public class CubicData
{
	private static readonly Logger LOGGER = LogManager.GetLogger(nameof(CubicData));
	
	private readonly Map<int, Map<int, CubicTemplate>> _cubics = new();
	
	protected CubicData()
	{
		load();
	}
	
	public void load()
	{
		_cubics.clear();
		parseDatapackDirectory("data/stats/cubics", true);
		LOGGER.Info(GetType().Name + ": Loaded " + _cubics.size() + " cubics.");
	}
	
	public void parseDocument(Document doc, File f)
	{
		forEach(doc, "list", listNode => forEach(listNode, "cubic", cubicNode => parseTemplate(cubicNode, new CubicTemplate(new StatSet(parseAttributes(cubicNode))))));
	}
	
	/**
	 * @param cubicNode
	 * @param template
	 */
	private void parseTemplate(Node cubicNode, CubicTemplate template)
	{
		forEach(cubicNode, IXmlReader::isNode, innerNode =>
		{
			switch (innerNode.getNodeName())
			{
				case "conditions":
				{
					parseConditions(innerNode, template, template);
					break;
				}
				case "skills":
				{
					parseSkills(innerNode, template);
					break;
				}
			}
		});
		_cubics.computeIfAbsent(template.getId(), key => new()).put(template.getLevel(), template);
	}
	
	/**
	 * @param cubicNode
	 * @param template
	 * @param holder
	 */
	private void parseConditions(Node cubicNode, CubicTemplate template, ICubicConditionHolder holder)
	{
		forEach(cubicNode, IXmlReader::isNode, conditionNode =>
		{
			switch (conditionNode.getNodeName())
			{
				case "hp":
				{
					HpConditionType type = parseEnum(conditionNode.getAttributes(), HpCondition.HpConditionType.class, "type");
					int hpPer = parseInteger(conditionNode.getAttributes(), "percent");
					holder.addCondition(new HpCondition(type, hpPer));
					break;
				}
				case "range":
				{
					int range = parseInteger(conditionNode.getAttributes(), "value");
					holder.addCondition(new RangeCondition(range));
					break;
				}
				case "healthPercent":
				{
					int min = parseInteger(conditionNode.getAttributes(), "min");
					int max = parseInteger(conditionNode.getAttributes(), "max");
					holder.addCondition(new HealthCondition(min, max));
					break;
				}
				default:
				{
					LOGGER.Warn("Attempting to use not implemented condition: " + conditionNode.getNodeName() + " for cubic id: " + template.getId() + " level: " + template.getLevel());
					break;
				}
			}
		});
	}
	
	/**
	 * @param cubicNode
	 * @param template
	 */
	private void parseSkills(Node cubicNode, CubicTemplate template)
	{
		forEach(cubicNode, "skill", skillNode =>
		{
			CubicSkill skill = new CubicSkill(new StatSet(parseAttributes(skillNode)));
			forEach(cubicNode, "conditions", conditionNode => parseConditions(cubicNode, template, skill));
			template.getCubicSkills().add(skill);
		});
	}
	
	/**
	 * @param id
	 * @param level
	 * @return the CubicTemplate for specified id and level
	 */
	public CubicTemplate getCubicTemplate(int id, int level)
	{
		return _cubics.getOrDefault(id, new()).get(level);
	}
	
	/**
	 * Gets the single instance of CubicData.
	 * @return single instance of CubicData
	 */
	public static CubicData getInstance()
	{
		return SingletonHolder.INSTANCE;
	}
	
	private static class SingletonHolder
	{
		public static readonly CubicData INSTANCE = new();
	}
}