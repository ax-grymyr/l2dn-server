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
 */
public class SkillData: DataReaderBase
{
	private static readonly Logger LOGGER = LogManager.GetLogger(nameof(SkillData));

	private readonly Map<long, Skill> _skills = new();
	private readonly Map<int, int> _skillsMaxLevel = new();

	private sealed class NamedParamInfo(string name, int? fromLevel, int? toLevel, int? fromSubLevel, int? toSubLevel,
        Map<int, Map<int, StatSet>> info)
    {
        public string Name => name;
        public int? FromLevel => fromLevel;
        public int? ToLevel => toLevel;
        public int? FromSubLevel => fromSubLevel;
        public int? ToSubLevel => toSubLevel;
        public Map<int, Map<int, StatSet>> Info => info;
    }

    private SkillData()
	{
		Load();
	}

	/**
	 * Provides the skill hash
	 * @param skill The Skill to be hashed
	 * @return getSkillHashCode(skill.getId(), skill.getLevel())
	 */
    private static long GetSkillHashCode(Skill skill)
	{
		return getSkillHashCode(skill.getId(), skill.getLevel(), skill.getSubLevel());
	}

	/**
	 * Centralized method for easier change of the hashing sys
	 * @param skillId The Skill Id
	 * @param skillLevel The Skill Level
	 * @param subSkillLevel The skill sub level
	 * @return The Skill hash number
	 */
	public static long getSkillHashCode(int skillId, int skillLevel, int subSkillLevel = 0)
	{
		return skillId * 4294967296L + subSkillLevel * 65536 + skillLevel;
	}

	public Skill? getSkill(int skillId, int level, int subLevel = 0)
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

    private void Load()
    {
		_skills.Clear();
		_skillsMaxLevel.Clear();

		LoadXmlDocuments(DataFileLocation.Data, "stats/skills").ForEach(t =>
		{
			t.Document.Elements("list").Elements("skill").ForEach(LoadElement);
		});

		if (Config.CUSTOM_SKILLS_LOAD)
		{
			LoadXmlDocuments(DataFileLocation.Data, "stats/skills/custom").ForEach(t =>
			{
				t.Document.Elements("list").Elements("skill").ForEach(LoadElement);
			});
		}

		LOGGER.Info(GetType().Name + ": Loaded " + _skills.Count + " Skills.");
	}

	public void Reload()
	{
		Load();

        // Reload Skill Tree as well.
		SkillTreeData.getInstance().load();
	}

	private void LoadElement(XElement element)
	{
		Map<int, Set<int>> levels = new(); // key - level, value - sublevel set
		Map<int, Map<int, StatSet>> skillInfo = new();
		StatSet generalSkillInfo = skillInfo.GetOrAdd(-1, _ => []).GetOrAdd(-1, _ => new StatSet());
		ParseAttributes(element, string.Empty, generalSkillInfo);

		Map<string, Map<int, Map<int, object>>> variableValues = new(); // key - name
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
					variableValues.put(name, ParseValues(skillNode));
					break;
				}

				default:
				{
					EffectScope? effectScope = EffectScopeUtil.FindByName(skillNodeName);
					if (effectScope != null)
					{
						skillNode.Elements("effect").ForEach(effectsNode =>
						{
							effectParamInfo.GetOrAdd(effectScope.Value, _ => [])
								.Add(ParseNamedParamInfo(effectsNode, variableValues));
						});

						break;
					}

					SkillConditionScope? skillConditionScope = SkillConditionScopeUtil.FindByXmlName(skillNodeName);
					if (skillConditionScope != null)
					{
						skillNode.Elements("condition").ForEach(conditionNode =>
						{
							conditionParamInfo.GetOrAdd(skillConditionScope.Value, _ => [])
								.Add(ParseNamedParamInfo(conditionNode, variableValues));
						});
					}
					else
					{
						ParseInfo(skillNode, variableValues, skillInfo);
					}

					break;
				}
			}
		}

		int fromLevel = generalSkillInfo.getInt(".fromLevel", 1);
		int toLevel = generalSkillInfo.getInt(".toLevel", 0);
		for (int i = fromLevel; i <= toLevel; i++)
		{
			levels.GetOrAdd(i, _ => []).add(0);
		}

		skillInfo.ForEach(kvp =>
		{
			int level = kvp.Key;
			Map<int, StatSet> subLevelMap = kvp.Value;
			if (level == -1)
				return;

			subLevelMap.ForEach(kvp2 =>
			{
				int subLevel = kvp2.Key;
				if (subLevel == -1)
					return;

				levels.GetOrAdd(level, _ => []).add(subLevel);
			});
		});

		effectParamInfo.Values.Concat(conditionParamInfo.Values).ForEach(namedParamInfos =>
			namedParamInfos.ForEach(namedParamInfo =>
			{
				namedParamInfo.Info.ForEach(kvp =>
				{
					(int level, Map<int, StatSet> subLevelMap) = kvp;
					if (level == -1)
						return;

					subLevelMap.ForEach(kvp2 =>
					{
						(int subLevel, _) = kvp2;
						if (subLevel == -1)
							return;

						levels.GetOrAdd(level, _ => []).add(subLevel);
					});
				});

                int? fromLevel = namedParamInfo.FromLevel;
                int? toLevel = namedParamInfo.ToLevel;
				if (fromLevel != null && toLevel != null)
				{
					for (int i = fromLevel.Value; i <= toLevel.Value; i++)
                    {
                        int? fromSubLevel = namedParamInfo.FromSubLevel;
                        int? toSubLevel = namedParamInfo.ToSubLevel;
						if (fromSubLevel != null && toSubLevel != null)
						{
							for (int j = fromSubLevel.Value; j <= toSubLevel.Value; j++)
							{
								levels.GetOrAdd(i, _ => []).add(j);
							}
						}
						else
						{
							levels.GetOrAdd(i, _ => []).add(0);
						}
					}
				}
			}));

		levels.ForEach(kvp => kvp.Value.ForEach(subLevel =>
		{
			// TODO: review code and refactor
			(int level, _) = kvp;
			StatSet statSet = skillInfo.GetValueOrDefault(level, []).get(subLevel) ?? new StatSet();
			skillInfo.GetValueOrDefault(level, []).GetValueOrDefault(-1, new StatSet()).getSet()
				.ForEach(x => statSet.getSet().TryAdd(x.Key, x.Value));
			skillInfo.GetValueOrDefault(-1, []).GetValueOrDefault(-1, new StatSet()).getSet()
				.ForEach(x => statSet.getSet().TryAdd(x.Key, x.Value));
			statSet.set(".level", level);
			statSet.set(".subLevel", subLevel);
			Skill skill = new(statSet);
			ForEachNamedParamInfoParam(effectParamInfo, level, subLevel, (effectScope, @params) =>
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

			ForEachNamedParamInfoParam(conditionParamInfo, level, subLevel, (skillConditionScope, @params) =>
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

			_skills.put(GetSkillHashCode(skill), skill);
			_skillsMaxLevel.merge(skill.getId(), skill.getLevel(), Math.Max);
			if (skill.getSubLevel() % 1000 == 1)
			{
				EnchantSkillGroupsData.getInstance()
					.addRouteForSkill(skill.getId(), skill.getLevel(), skill.getSubLevel());
			}
		}));
	}

	private static void ForEachNamedParamInfoParam<T>(Map<T, List<NamedParamInfo>> paramInfo, int level, int subLevel,
		Action<T, StatSet> consumer)
		where T: notnull
	{
		paramInfo.ForEach(kvp => kvp.Value.ForEach(namedParamInfo =>
		{
			// TODO: review code and refactor
			(T scope, _) = kvp;
			if (((namedParamInfo.FromLevel == null && namedParamInfo.ToLevel == null) ||
			     (namedParamInfo.FromLevel <= level && namedParamInfo.ToLevel >= level)) //
			    && ((namedParamInfo.FromSubLevel == null && namedParamInfo.ToSubLevel == null) ||
			        (namedParamInfo.FromSubLevel <= subLevel && namedParamInfo.ToSubLevel >= subLevel)))
			{
				StatSet @params = namedParamInfo.Info.GetValueOrDefault(level, []).get(subLevel) ?? new StatSet();

				namedParamInfo.Info.GetValueOrDefault(level, [])
					.GetValueOrDefault(-1, new StatSet()).getSet()
					.ForEach(x => @params.getSet().TryAdd(x.Key, x.Value));
				namedParamInfo.Info.GetValueOrDefault(-1, [])
					.GetValueOrDefault(-1, new StatSet()).getSet()
					.ForEach(x => @params.getSet().TryAdd(x.Key, x.Value));
				@params.set(".name", namedParamInfo.Name);
				consumer(scope, @params);
			}
		}));
	}

	private NamedParamInfo ParseNamedParamInfo(XElement element, Map<string, Map<int, Map<int, object>>> variableValues)
	{
		string name = element.GetAttributeValueAsString("name");
		int? level = element.GetAttributeValueAsInt32OrNull("level");
		int? fromLevel = element.GetAttributeValueAsInt32OrNull("fromLevel") ?? level;
		int? toLevel = element.GetAttributeValueAsInt32OrNull("toLevel") ?? level;
		int? subLevel = element.GetAttributeValueAsInt32OrNull("subLevel");
		int? fromSubLevel = element.GetAttributeValueAsInt32OrNull("fromSubLevel") ?? subLevel;
		int? toSubLevel = element.GetAttributeValueAsInt32OrNull("toSubLevel") ?? subLevel;

		Map<int, Map<int, StatSet>> info = [];
		element.Elements().ForEach(el => ParseInfo(el, variableValues, info));

		return new NamedParamInfo(name, fromLevel, toLevel, fromSubLevel, toSubLevel, info);
	}

	private static void ParseInfo(XElement element, Map<string, Map<int, Map<int, object>>> variableValues,
		Map<int, Map<int, StatSet>> info)
	{
		Map<int, Map<int, object>> values = ParseValues(element);
		object? generalValue = values.GetValueOrDefault(-1)?.GetValueOrDefault(-1);
		if (generalValue != null)
		{
			string stringGeneralValue = generalValue.ToString() ?? string.Empty;
			if (stringGeneralValue.startsWith("@"))
            {
                values = variableValues.GetValueOrDefault(stringGeneralValue) ??
                    throw new InvalidOperationException("undefined variable " + stringGeneralValue);
			}
		}

		values.ForEach(kvp =>
		{
			int level = kvp.Key;
			kvp.Value.ForEach(kvp2 =>
			{
				(int subLevel, object value) = kvp2;
				info.GetOrAdd(level, _ => []).GetOrAdd(subLevel, _ => new StatSet())
					.set(element.Name.LocalName, value);
			});
		});
	}

	private static Map<int, Map<int, object>> ParseValues(XElement element)
	{
		Map<int, Map<int, object>> values = [];
		object? parsedValue = ParseValue(element, true, false, []);
		if (parsedValue != null)
		{
			values.GetOrAdd(-1, _ => []).put(-1, parsedValue);
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
						parsedValue = ParseValue(n, false, false, []);
						if (parsedValue != null)
						{
							int subLevel = n.Attribute("subLevel").GetInt32(-1);
							values.GetOrAdd(level, _ => []).put(subLevel, parsedValue);
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
								Map<int, object> subValues = values.GetOrAdd(i, _ => []);
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

								parsedValue = ParseValue(n, false, false, variables);
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

    private static object? ParseValue(XElement element, bool blockValue, bool parseAttributes, Map<string, double> variables)
	{
		StatSet? statSet = null;
		List<object>? list = null;
		object? text = null;
		if (parseAttributes && (!element.Name.LocalName.equals("value") || !blockValue) && element.Attributes().Any())
		{
			statSet = new StatSet();
			ParseAttributes(element, string.Empty, statSet, variables);
		}

		if (!element.HasElements && !string.IsNullOrEmpty(element.Value))
		{
			string value = element.Value.Trim();
			if (!string.IsNullOrEmpty(value))
			{
				text = ParseNodeValue(value, variables);
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

					object? value = ParseValue(n, false, true, variables);
					if (value != null)
						list.Add(value);

					break;
				}
				case "value":
				{
					if (blockValue)
						break;

					// fallthrough
					goto default;
				}
				default:
				{
					object? value = ParseValue(n, false, true, variables);
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

    private static void ParseAttributes(XElement element, string prefix, StatSet statSet,
        Map<string, double>? variables = null)
    {
        foreach (XAttribute attribute in element.Attributes())
        {
            string name = attribute.Name.LocalName;
            string value = attribute.Value;
            statSet.set(prefix + "." + name, ParseNodeValue(value, variables));
        }
    }

    private static object ParseNodeValue(string value, Map<string, double>? variables)
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

    public static SkillData getInstance() => SingletonHolder.INSTANCE;

    private static class SingletonHolder
    {
        public static readonly SkillData INSTANCE = new();
    }
}