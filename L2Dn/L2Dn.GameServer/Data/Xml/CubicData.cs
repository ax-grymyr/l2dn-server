using System.Xml.Linq;
using L2Dn.Extensions;
using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor.Templates;
using L2Dn.GameServer.Model.Cubics;
using L2Dn.GameServer.Model.Cubics.Conditions;
using L2Dn.GameServer.Utilities;
using L2Dn.Utilities;
using NLog;

namespace L2Dn.GameServer.Data.Xml;

/**
 * @author UnAfraid
 */
public class CubicData: DataReaderBase
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
		
		LoadXmlDocuments(DataFileLocation.Data, "stats/cubics", true).ForEach(t =>
		{
			t.Document.Elements("list").Elements("cubic").ForEach(x => loadElement(t.FilePath, x));
		});
		
		LOGGER.Info(GetType().Name + ": Loaded " + _cubics.size() + " cubics.");
	}
	
	private void loadElement(string filePath, XElement element)
	{
		StatSet set = new StatSet(element);
		parseTemplate(element, new CubicTemplate(set));
	}
	
	/**
	 * @param cubicNode
	 * @param template
	 */
	private void parseTemplate(XElement cubicNode, CubicTemplate template)
	{
		cubicNode.Elements("conditions").ForEach(e => parseConditions(e, template, template));
		cubicNode.Elements("skills").ForEach(e => parseSkills(e, template));
		_cubics.computeIfAbsent(template.getId(), key => new()).put(template.getLevel(), template);
	}
	
	/**
	 * @param cubicNode
	 * @param template
	 * @param holder
	 */
	private void parseConditions(XElement element, CubicTemplate template, ICubicConditionHolder holder)
	{
		element.Elements().ForEach(el =>
		{
			switch (el.Name.LocalName)
			{
				case "hp":
					HpCondition.HpConditionType type = el.Attribute("type").GetEnum<HpCondition.HpConditionType>();
					int hpPer = el.GetAttributeValueAsInt32("percent");
					holder.addCondition(new HpCondition(type, hpPer));
					break;
				case "range":
					int range = el.GetAttributeValueAsInt32("value");
					holder.addCondition(new RangeCondition(range));
					break;
				case "healthPercent":
					int min = el.GetAttributeValueAsInt32("min");
					int max = el.GetAttributeValueAsInt32("max");
					holder.addCondition(new HealthCondition(min, max));
					break;
				default:
					LOGGER.Error("Attempting to use not implemented condition: " + el.Name.LocalName +
					             " for cubic id: " + template.getId() + " level: " + template.getLevel());
					
					break;
			}
		});
	}
	
	/**
	 * @param cubicNode
	 * @param template
	 */
	private void parseSkills(XElement element, CubicTemplate template)
	{
		element.Elements("skill").ForEach(el =>
		{
			StatSet set = new StatSet(element);
			CubicSkill skill = new CubicSkill(set);
			el.Elements("conditions").ForEach(e => parseConditions(e, template, skill));
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