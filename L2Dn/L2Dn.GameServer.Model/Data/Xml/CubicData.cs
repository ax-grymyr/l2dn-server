using System.Collections.Frozen;
using L2Dn.Extensions;
using L2Dn.GameServer.Model.Actor.Templates;
using L2Dn.GameServer.Model.Cubics;
using L2Dn.GameServer.Model.Cubics.Conditions;
using L2Dn.GameServer.StaticData;
using L2Dn.GameServer.Utilities;
using L2Dn.Model.Xml;
using NLog;

namespace L2Dn.GameServer.Data.Xml;

public sealed class CubicData: DataReaderBase
{
	private static readonly Logger _logger = LogManager.GetLogger(nameof(CubicData));

	// key is Cubic id and level pair
	private static FrozenDictionary<(int, int), CubicTemplate> _cubics =
		FrozenDictionary<(int, int), CubicTemplate>.Empty;
	
	private CubicData()
	{
		load();
	}
	
	public void load()
	{
		Dictionary<(int, int), CubicTemplate> cubics = new();
		LoadXmlDocuments<XmlCubicData>(DataFileLocation.Data, "stats/cubics", true)
			.SelectMany(t => t.Document.Cubics)
			.ForEach(c => ParseCubic(c, cubics));

		_cubics = cubics.ToFrozenDictionary();

		int count = cubics.Select(p => p.Key.Item1).Distinct().Count();
		
		_logger.Info(GetType().Name + ": Loaded " + count + " cubics.");
	}
	
	/**
	 * @param cubicNode
	 * @param template
	 */
	private static void ParseCubic(XmlCubic xmlCubic, Dictionary<(int, int), CubicTemplate> cubics)
	{
		CubicTemplate template = new(xmlCubic);

		XmlCubicBaseConditions? conditions = xmlCubic.Conditions;
		if (conditions != null)
		{
			XmlCubicConditionHp? hpCondition = conditions.Hp;
			if (hpCondition != null)
				template.addCondition(new HpCondition(hpCondition.Type, hpCondition.Percent));

			XmlCubicConditionRange? rangeCondition = conditions.Range;
			if (rangeCondition != null)
				template.addCondition(new RangeCondition(rangeCondition.Value));
		}

		foreach (XmlCubicSkill xmlCubicSkill in xmlCubic.Skills)
		{
			CubicSkill skill = new(xmlCubicSkill);
			XmlCubicSkillConditions? skillConditions = xmlCubicSkill.Conditions;
			if (skillConditions != null)
			{
				XmlCubicConditionHp? hpCondition = skillConditions.Hp;
				if (hpCondition != null)
					skill.addCondition(new HpCondition(hpCondition.Type, hpCondition.Percent));

				XmlCubicConditionRange? rangeCondition = skillConditions.Range;
				if (rangeCondition != null)
					skill.addCondition(new RangeCondition(rangeCondition.Value));

				XmlCubicConditionHealthPercent? healthCondition = skillConditions.HealthPercent;
				if (healthCondition != null)
					skill.addCondition(new HealthCondition(healthCondition.Min, healthCondition.Max));
			}
			
			template.getCubicSkills().Add(skill);
		}

		if (!cubics.TryAdd((template.getId(), template.getLevel()), template))
			_logger.Info($"{nameof(CubicData)}: Duplicated cubic data id={template.getId()}, level={template.getLevel()}.");
	}
	
	/**
	 * @param id
	 * @param level
	 * @return the CubicTemplate for specified id and level
	 */
	public CubicTemplate? getCubicTemplate(int id, int level)
	{
		return _cubics.GetValueOrDefault((id, level));
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