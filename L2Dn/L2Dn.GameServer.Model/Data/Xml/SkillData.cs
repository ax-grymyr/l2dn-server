using System.Runtime.CompilerServices;
using System.Xml.Linq;
using L2Dn.Extensions;
using L2Dn.GameServer.Handlers;
using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Effects;
using L2Dn.GameServer.Model.Skills;
using L2Dn.GameServer.Utilities;
using L2Dn.Parsing;
using L2Dn.Utilities;
using NLog;

namespace L2Dn.GameServer.Data.Xml;

/**
 * Skill data parser.
 * @author NosBit
 */
public class SkillData: DataReaderBase
{
	private static readonly Logger LOGGER = LogManager.GetLogger(nameof(SkillData));

	private readonly Map<long, Skill> _skills = new();
	private readonly Map<int, int> _skillsMaxLevel = new();

	private class NamedParamInfo
	{
		private readonly string _name;
		private readonly int? _fromLevel;
		private readonly int? _toLevel;
		private readonly int? _fromSubLevel;
		private readonly int? _toSubLevel;
		private readonly Map<int, Map<int, StatSet>> _info;

		public NamedParamInfo(string name, int? fromLevel, int? toLevel, int? fromSubLevel, int? toSubLevel,
			Map<int, Map<int, StatSet>> info)
		{
			_name = name;
			_fromLevel = fromLevel;
			_toLevel = toLevel;
			_fromSubLevel = fromSubLevel;
			_toSubLevel = toSubLevel;
			_info = info;
		}

		public string getName()
		{
			return _name;
		}

		public int? getFromLevel()
		{
			return _fromLevel;
		}

		public int? getToLevel()
		{
			return _toLevel;
		}

		public int? getFromSubLevel()
		{
			return _fromSubLevel;
		}

		public int? getToSubLevel()
		{
			return _toSubLevel;
		}

		public Map<int, Map<int, StatSet>> getInfo()
		{
			return _info;
		}
	}

	protected SkillData()
	{
		load();
	}

	/**
	 * Provides the skill hash
	 * @param skill The Skill to be hashed
	 * @return getSkillHashCode(skill.getId(), skill.getLevel())
	 */
	public static long getSkillHashCode(Skill skill)
	{
		return getSkillHashCode(skill.getId(), skill.getLevel(), skill.getSubLevel());
	}

	/**
	 * Centralized method for easier change of the hashing sys
	 * @param skillId The Skill Id
	 * @param skillLevel The Skill Level
	 * @return The Skill hash number
	 */
	public static long getSkillHashCode(int skillId, int skillLevel)
	{
		return getSkillHashCode(skillId, skillLevel, 0);
	}

	/**
	 * Centralized method for easier change of the hashing sys
	 * @param skillId The Skill Id
	 * @param skillLevel The Skill Level
	 * @param subSkillLevel The skill sub level
	 * @return The Skill hash number
	 */
	public static long getSkillHashCode(int skillId, int skillLevel, int subSkillLevel)
	{
		return skillId * 4294967296L + subSkillLevel * 65536 + skillLevel;
	}

	public Skill? getSkill(int skillId, int level)
	{
		return getSkill(skillId, level, 0);
	}

	public Skill? getSkill(int skillId, int level, int subLevel)
	{
		Skill? result = _skills.get(getSkillHashCode(skillId, level, subLevel));
		if (result != null)
		{
			return result;
		}

		// skill/level not found, fix for transformation scripts
		int maxLevel = getMaxLevel(skillId);
		// requested level too high
		if (maxLevel > 0 && level > maxLevel)
		{
			LOGGER.Warn(GetType().Name + ": Call to unexisting skill level id: " + skillId + " requested level: " +
			            level + " max level: " + maxLevel + ".");
			return _skills.get(getSkillHashCode(skillId, maxLevel));
		}

		LOGGER.Warn(GetType().Name + ": No skill info found for skill id " + skillId + " and skill level " + level);
		return null;
	}

	public int getMaxLevel(int skillId)
    {
        return _skillsMaxLevel.GetValueOrDefault(skillId);
	}

	/**
	 * @param addNoble
	 * @param hasCastle
	 * @return an array with siege skills. If addNoble == true, will add also Advanced headquarters.
	 */
	public List<Skill> getSiegeSkills(bool addNoble, bool hasCastle)
	{
		List<Skill?> temp =
        [
            _skills.get(getSkillHashCode((int)CommonSkill.SEAL_OF_RULER, 1)),
            _skills.get(getSkillHashCode(247, 1)), // Build Headquarters
        ];

        if (addNoble)
		{
			temp.Add(_skills.get(getSkillHashCode(326, 1))); // Build Advanced Headquarters
		}

		if (hasCastle)
		{
			temp.Add(_skills.get(getSkillHashCode(844, 1))); // Outpost Construction
			temp.Add(_skills.get(getSkillHashCode(845, 1))); // Outpost Demolition
		}

		return temp.Where(s => s != null).ToList()!; // TODO: review code and refactor
	}

	public bool isValidating()
	{
		return false;
	}

	[MethodImpl(MethodImplOptions.Synchronized)]
	public void load()
	{
		_skills.Clear();
		_skillsMaxLevel.Clear();

		LoadXmlDocuments(DataFileLocation.Data, "stats/skills").ForEach(t =>
		{
			t.Document.Elements("list").Elements("skill").ForEach(x => loadElement(t.FilePath, x));
		});

		if (Config.CUSTOM_SKILLS_LOAD)
		{
			LoadXmlDocuments(DataFileLocation.Data, "stats/skills/custom").ForEach(t =>
			{
				t.Document.Elements("list").Elements("skill").ForEach(x => loadElement(t.FilePath, x));
			});
		}

		LOGGER.Info(GetType().Name + ": Loaded " + _skills.Count + " Skills.");
	}

	public void reload()
	{
		load();
		// Reload Skill Tree as well.
		SkillTreeData.getInstance().load();
	}

	private void loadElement(string filePath, XElement element)
	{
		Map<int, Set<int>> levels = new();
		Map<int, Map<int, StatSet>> skillInfo = new();
		StatSet generalSkillInfo = skillInfo.computeIfAbsent(-1, k => new()).computeIfAbsent(-1, k => new StatSet());
		parseAttributes(element, "", generalSkillInfo);

		Map<string, Map<int, Map<int, object>>> variableValues = new();
		Map<EffectScope, List<NamedParamInfo>> effectParamInfo = new();
		Map<SkillConditionScope, List<NamedParamInfo>> conditionParamInfo = new();

		foreach (XElement skillNode in element.Elements())
		{
			string skillNodeName = skillNode.Name.LocalName;
			switch (skillNodeName.toLowerCase())
			{
				case "variable":
				{
					string name = "@" + skillNode.GetAttributeValueAsString("name");
					variableValues.put(name, parseValues(skillNode));
					break;
				}

				default:
				{
					EffectScope? effectScope = EffectScopeUtil.FindByName(skillNodeName);
					if (effectScope != null)
					{
						skillNode.Elements("effect").ForEach(effectsNode =>
						{
							effectParamInfo.computeIfAbsent(effectScope.Value, k => new())
								.Add(parseNamedParamInfo(effectsNode, variableValues));
						});

						break;
					}

					SkillConditionScope? skillConditionScope = SkillConditionScopeUtil.FindByXmlName(skillNodeName);
					if (skillConditionScope != null)
					{
						skillNode.Elements("condition").ForEach(conditionNode =>
						{
							conditionParamInfo.computeIfAbsent(skillConditionScope.Value, k => new())
								.Add(parseNamedParamInfo(conditionNode, variableValues));
						});
					}
					else
					{
						parseInfo(skillNode, variableValues, skillInfo);
					}

					break;
				}
			}
		}

		int fromLevel = generalSkillInfo.getInt(".fromLevel", 1);
		int toLevel = generalSkillInfo.getInt(".toLevel", 0);
		for (int i = fromLevel; i <= toLevel; i++)
		{
			levels.computeIfAbsent(i, k => new()).add(0);
		}

		skillInfo.ForEach(kvp =>
		{
			int level = kvp.Key;
			Map<int, StatSet> subLevelMap = kvp.Value;
			if (level == -1)
			{
				return;
			}

			subLevelMap.ForEach(kvp2 =>
			{
				int subLevel = kvp2.Key;
				StatSet statSet = kvp2.Value;
				if (subLevel == -1)
				{
					return;
				}

				levels.computeIfAbsent(level, k => new()).add(subLevel);
			});
		});

		effectParamInfo.Values.Concat(conditionParamInfo.Values).ForEach(namedParamInfos =>
			namedParamInfos.ForEach(namedParamInfo =>
			{
				namedParamInfo.getInfo().ForEach(kvp =>
				{
					var (level, subLevelMap) = kvp;
					if (level == -1)
					{
						return;
					}

					subLevelMap.ForEach(kvp2 =>
					{
						var (subLevel, statSet) = kvp2;
						if (subLevel == -1)
						{
							return;
						}

						levels.computeIfAbsent(level, k => new()).add(subLevel);
					});
				});

                int? fromLevel = namedParamInfo.getFromLevel();
                int? toLevel = namedParamInfo.getToLevel();
				if (fromLevel != null && toLevel != null)
				{
					for (int i = fromLevel.Value; i <= toLevel.Value; i++)
                    {
                        int? fromSubLevel = namedParamInfo.getFromSubLevel();
                        int? toSubLevel = namedParamInfo.getToSubLevel();
						if (fromSubLevel != null && toSubLevel != null)
						{
							for (int j = fromSubLevel.Value; j <= toSubLevel.Value; j++)
							{
								levels.computeIfAbsent(i, k => new()).add(j);
							}
						}
						else
						{
							levels.computeIfAbsent(i, k => new()).add(0);
						}
					}
				}
			}));

		levels.ForEach(kvp => kvp.Value.ForEach(subLevel =>
		{
			// TODO: review code and refactor
			var (level, subLevels) = kvp;
			StatSet statSet = skillInfo.GetValueOrDefault(level, []).get(subLevel) ?? new StatSet();
			skillInfo.GetValueOrDefault(level, new()).GetValueOrDefault(-1, StatSet.EMPTY_STATSET).getSet()
				.ForEach(x => statSet.getSet().TryAdd(x.Key, x.Value));
			skillInfo.GetValueOrDefault(-1, new()).GetValueOrDefault(-1, StatSet.EMPTY_STATSET).getSet()
				.ForEach(x => statSet.getSet().TryAdd(x.Key, x.Value));
			statSet.set(".level", level);
			statSet.set(".subLevel", subLevel);
			Skill skill = new Skill(statSet);
			forEachNamedParamInfoParam(effectParamInfo, level, subLevel, (effectScope, @params) =>
            {
                string effectName = @params.getString(".name");
                @params.remove(".name");
                try
                {
                    Func<StatSet, AbstractEffect>? effectFunction =
                        EffectHandler.getInstance().getHandlerFactory(effectName);
                    if (effectFunction != null)
                    {
                        skill.addEffect(effectScope, effectFunction(@params));
                    }
                    else
                    {
                        LOGGER.Warn(GetType().Name + ": Missing effect for Skill Id[" + statSet.getInt(".id") +
                            "] Level[" + level + "] SubLevel[" + subLevel + "] Effect Scope[" + effectScope +
                            "] Effect Name[" + effectName + "]");
                    }
                }
                catch (Exception e)
                {
                    LOGGER.Warn(
                        GetType().Name + ": Failed loading effect for Skill Id[" + statSet.getInt(".id") + "] Level[" +
                        level + "] SubLevel[" + subLevel + "] Effect Scope[" + effectScope + "] Effect Name[" +
                        effectName + "]", e);
                }
            });

			forEachNamedParamInfoParam(conditionParamInfo, level, subLevel, (skillConditionScope, @params) =>
            {
                string conditionName = @params.getString(".name");
                @params.remove(".name");
                try
                {
                    Func<StatSet, ISkillCondition>? conditionFunction =
                        SkillConditionHandler.getInstance().getHandlerFactory(conditionName);
                    if (conditionFunction != null)
                    {
                        if (skill.isPassive())
                        {
                            if (skillConditionScope != SkillConditionScope.PASSIVE)
                            {
                                LOGGER.Warn(GetType().Name + ": Non passive condition for passive Skill Id[" +
                                    statSet.getInt(".id") + "] Level[" + level + "] SubLevel[" + subLevel +
                                    "]");
                            }
                        }
                        else if (skillConditionScope == SkillConditionScope.PASSIVE)
                        {
                            LOGGER.Warn(GetType().Name + ": Passive condition for non passive Skill Id[" +
                                statSet.getInt(".id") + "] Level[" + level + "] SubLevel[" + subLevel + "]");
                        }

                        skill.addCondition(skillConditionScope, conditionFunction(@params));
                    }
                    else
                    {
                        LOGGER.Warn(GetType().Name + ": Missing condition for Skill Id[" + statSet.getInt(".id") +
                            "] Level[" + level + "] SubLevel[" + subLevel + "] Effect Scope[" +
                            skillConditionScope + "] Effect Name[" + conditionName + "]");
                    }
                }
                catch (Exception e)
                {
                    LOGGER.Warn(
                        GetType().Name + ": Failed loading condition for Skill Id[" + statSet.getInt(".id") +
                        "] Level[" + level + "] SubLevel[" + subLevel + "] Condition Scope[" + skillConditionScope +
                        "] Condition Name[" + conditionName + "]", e);
                }
            });

			_skills.put(getSkillHashCode(skill), skill);
			_skillsMaxLevel.merge(skill.getId(), skill.getLevel(), Math.Max);
			if (skill.getSubLevel() % 1000 == 1)
			{
				EnchantSkillGroupsData.getInstance()
					.addRouteForSkill(skill.getId(), skill.getLevel(), skill.getSubLevel());
			}
		}));
	}

	private void forEachNamedParamInfoParam<T>(Map<T, List<NamedParamInfo>> paramInfo, int level, int subLevel,
		Action<T, StatSet> consumer)
		where T: notnull
	{
		paramInfo.ForEach(kvp => kvp.Value.ForEach(namedParamInfo =>
		{
			// TODO: review code and refactor
			var (scope, namedParamInfos) = kvp;
			if (((namedParamInfo.getFromLevel() == null && namedParamInfo.getToLevel() == null) ||
			     (namedParamInfo.getFromLevel() <= level && namedParamInfo.getToLevel() >= level)) //
			    && ((namedParamInfo.getFromSubLevel() == null && namedParamInfo.getToSubLevel() == null) ||
			        (namedParamInfo.getFromSubLevel() <= subLevel && namedParamInfo.getToSubLevel() >= subLevel)))
			{
				StatSet @params = namedParamInfo.getInfo().GetValueOrDefault(level, new()).get(subLevel) ?? new StatSet();

				namedParamInfo.getInfo().GetValueOrDefault(level, new())
					.GetValueOrDefault(-1, StatSet.EMPTY_STATSET).getSet()
					.ForEach(x => @params.getSet().TryAdd(x.Key, x.Value));
				namedParamInfo.getInfo().GetValueOrDefault(-1, new())
					.GetValueOrDefault(-1, StatSet.EMPTY_STATSET).getSet()
					.ForEach(x => @params.getSet().TryAdd(x.Key, x.Value));
				@params.set(".name", namedParamInfo.getName());
				consumer(scope, @params);
			}
		}));
	}

	private NamedParamInfo parseNamedParamInfo(XElement element, Map<string, Map<int, Map<int, object>>> variableValues)
	{
		string name = element.GetAttributeValueAsString("name");
		int? level = element.GetAttributeValueAsInt32OrNull("level");
		int? fromLevel = element.GetAttributeValueAsInt32OrNull("fromLevel") ?? level;
		int? toLevel = element.GetAttributeValueAsInt32OrNull("toLevel") ?? level;
		int? subLevel = element.GetAttributeValueAsInt32OrNull("subLevel");
		int? fromSubLevel = element.GetAttributeValueAsInt32OrNull("fromSubLevel") ?? subLevel;
		int? toSubLevel = element.GetAttributeValueAsInt32OrNull("toSubLevel") ?? subLevel;

		Map<int, Map<int, StatSet>> info = new();
		element.Elements().ForEach(el => parseInfo(el, variableValues, info));

		return new NamedParamInfo(name, fromLevel, toLevel, fromSubLevel, toSubLevel, info);
	}

	private void parseInfo(XElement element, Map<string, Map<int, Map<int, object>>> variableValues,
		Map<int, Map<int, StatSet>> info)
	{
		Map<int, Map<int, object>> values = parseValues(element);
		object? generalValue = values.GetValueOrDefault(-1)?.GetValueOrDefault(-1);
		if (generalValue != null)
		{
			string stringGeneralValue = generalValue?.ToString() ?? string.Empty;
			if (stringGeneralValue.startsWith("@"))
			{
				Map<int, Map<int, object>>? variableValue = variableValues.GetValueOrDefault(stringGeneralValue);
				if (variableValue != null)
				{
					values = variableValue;
				}
				else
				{
					throw new InvalidOperationException("undefined variable " + stringGeneralValue);
				}
			}
		}

		values.ForEach(kvp =>
		{
			var (level, subLevelMap) = kvp;
			kvp.Value.ForEach(kvp2 =>
			{
				var (subLevel, value) = kvp2;
				info.computeIfAbsent(level, k => new()).computeIfAbsent(subLevel, k => new StatSet())
					.set(element.Name.LocalName, value);
			});
		});
	}

	private Map<int, Map<int, object>> parseValues(XElement element)
	{
		Map<int, Map<int, object>> values = new();
		object parsedValue = parseValue(element, true, false, new());
		if (parsedValue != null)
		{
			values.computeIfAbsent(-1, k => new()).put(-1, parsedValue);
		}
		else
		{
			foreach (XElement n in element.Elements())
			{
				if (n.Name.LocalName.equalsIgnoreCase("value"))
				{
					int level = n.Attribute("level").GetInt32(-1);
					if (level >= 0)
					{
						parsedValue = parseValue(n, false, false, new());
						if (parsedValue != null)
						{
							int subLevel = n.Attribute("subLevel").GetInt32(-1);
							values.computeIfAbsent(level, k => new()).put(subLevel, parsedValue);
						}
					}
					else
					{
						int fromLevel = n.GetAttributeValueAsInt32("fromLevel");
						int toLevel = n.GetAttributeValueAsInt32("toLevel");
						int fromSubLevel = n.Attribute("fromSubLevel").GetInt32(-1);
						int toSubLevel = n.Attribute("toSubLevel").GetInt32(-1);
						for (int i = fromLevel; i <= toLevel; i++)
						{
							for (int j = fromSubLevel; j <= toSubLevel; j++)
							{
								Map<int, object> subValues = values.computeIfAbsent(i, k => new());
								Map<string, double> variables = new();
								variables.put("index", i - fromLevel + 1d);
								variables.put("subIndex", j - fromSubLevel + 1d);
								object? @base = values.GetValueOrDefault(i)?.GetValueOrDefault(-1);
								string baseText = @base?.ToString() ?? string.Empty;
								if (@base != null && !(@base is StatSet) && !baseText.equalsIgnoreCase("true") &&
                                    !baseText.equalsIgnoreCase("false"))
								{
									variables.put("base", double.Parse(baseText));
								}

								parsedValue = parseValue(n, false, false, variables);
								if (parsedValue != null)
								{
									subValues.put(j, parsedValue);
								}
							}
						}
					}
				}
			}
		}

		return values;
	}

    private object? parseValue(XElement element, bool blockValue, bool parseAttributes, Map<string, double> variables)
	{
		StatSet? statSet = null;
		List<object>? list = null;
		object? text = null;
		if (parseAttributes && (!element.Name.LocalName.equals("value") || !blockValue) && element.Attributes().Any())
		{
			statSet = new StatSet();
			this.parseAttributes(element, "", statSet, variables);
		}

		if (!element.HasElements && !string.IsNullOrEmpty(element.Value))
		{
			string value = element.Value.Trim();
			if (!string.IsNullOrEmpty(value))
			{
				text = parseNodeValue(value, variables);
			}
		}

		foreach (XElement n in element.Elements())
		{
			string nodeName = n.Name.LocalName;
			switch (nodeName)
			{
				case "item":
				{
					if (list == null)
					{
						list = new();
					}

					object? value = parseValue(n, false, true, variables);
					if (value != null)
					{
						list.Add(value);
					}

					break;
				}
				case "value":
				{
					if (blockValue)
					{
						break;
					}

					// fallthrough
					goto default;
				}
				default:
				{
					object? value = parseValue(n, false, true, variables);
					if (value != null)
                    {
                        statSet ??= new StatSet();
                        statSet.set(nodeName, value);
                    }

					break;
				}
			}
		}

		if (list != null)
		{
			if (text != null)
			{
				throw new InvalidOperationException("Text and list in same node are not allowed. Node[" + element +
				                                    "]");
			}

			if (statSet != null)
			{
				statSet.set(".", list);
			}
			else
			{
				return list;
			}
		}

		if (text != null)
		{
			if (list != null)
			{
				throw new InvalidOperationException("Text and list in same node are not allowed. Node[" + element +
				                                    "]");
			}

			if (statSet != null)
			{
				statSet.set(".", text);
			}
			else
			{
				return text;
			}
		}

        return statSet;
    }

	private void parseAttributes(XElement element, string prefix, StatSet statSet, Map<string, double> variables)
	{
		foreach (XAttribute attribute in element.Attributes())
		{
			string name = attribute.Name.LocalName;
			string value = attribute.Value;
			statSet.set(prefix + "." + name, parseNodeValue(value, variables));
		}
	}

	private void parseAttributes(XElement element, string prefix, StatSet statSet)
	{
		parseAttributes(element, prefix, statSet, new());
	}

	private static object parseNodeValue(string value, Map<string, double> variables)
	{
		if (value.startsWith("{") && value.endsWith("}"))
		{
			ParserResult<Expression> result = ExpressionParser.Parser(value.Substring(1, value.Length - 2));
			if (!result.Success)
				throw new InvalidOperationException($"Invalid expression '{value}'");

			return result.Result.Evaluate(variables);
		}

		return value;
	}

	public static SkillData getInstance()
	{
		return SingletonHolder.INSTANCE;
	}

	private static class SingletonHolder
	{
		public static readonly SkillData INSTANCE = new();
	}
}