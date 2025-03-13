using System.Collections;
using System.Collections.Frozen;
using System.Globalization;
using System.Linq.Expressions;
using System.Xml.Linq;
using L2Dn.Extensions;
using L2Dn.GameServer.Data;
using L2Dn.GameServer.Data.Xml;
using L2Dn.GameServer.Handlers;
using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Effects;
using L2Dn.GameServer.Model.Skills;
using L2Dn.GameServer.Scripts.Handlers.EffectHandlers;
using L2Dn.GameServer.Utilities;
using L2Dn.Parsing;
using L2Dn.Utilities;
using Expression = L2Dn.Parsing.Expression;

namespace L2Dn.GameServer.StaticData.Tests;

public sealed class SkillTemplateLoadingTests
{
    [Fact]
    public void CompareSkillTemplates()
    {
        // Register effect and condition handlers
        Scripts.Scripts.RegisterHandlers();

        // Loading skill templates old way
        OldLoader oldLoader = new();
        oldLoader.Load();

        // Load skill templates
        SkillData skillData = SkillData.getInstance();

        // Compare skills
        SkillComparer.CompareSkills(oldLoader.Skills, skillData.Skills);
    }

    private sealed class OldLoader: DataReaderBase
    {
        private readonly Map<long, Skill> _skills = new();
        private readonly Map<int, int> _skillsMaxLevel = new();

        public Map<long, Skill> Skills => _skills;
        public Map<int, int> SkillsMaxLevel => _skillsMaxLevel;

        public void Load()
        {
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
        }

        private static long GetSkillHashCode(int skillId, int skillLevel, int subSkillLevel = 0)
        {
            return skillId * 4294967296L + subSkillLevel * 65536 + skillLevel;
        }

        private static long GetSkillHashCode(Skill skill)
        {
            return GetSkillHashCode(skill.getId(), skill.getLevel(), skill.getSubLevel());
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
                                effectParamInfo.GetOrAdd(effectScope.Value, _ => []).
                                    Add(ParseNamedParamInfo(effectsNode, variableValues));
                            });

                            break;
                        }

                        SkillConditionScope? skillConditionScope = SkillConditionScopeUtil.FindByXmlName(skillNodeName);
                        if (skillConditionScope != null)
                        {
                            skillNode.Elements("condition").ForEach(conditionNode =>
                            {
                                conditionParamInfo.GetOrAdd(skillConditionScope.Value, _ => []).
                                    Add(ParseNamedParamInfo(conditionNode, variableValues));
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
                levels.GetOrAdd(i, _ => []).Add(0);
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

                    levels.GetOrAdd(level, _ => []).Add(subLevel);
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

                            levels.GetOrAdd(level, _ => []).Add(subLevel);
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
                                    levels.GetOrAdd(i, _ => []).Add(j);
                                }
                            }
                            else
                            {
                                levels.GetOrAdd(i, _ => []).Add(0);
                            }
                        }
                    }
                }));

            levels.ForEach(kvp => kvp.Value.ForEach(subLevel =>
            {
                // TODO: review code and refactor
                (int level, _) = kvp;
                StatSet statSet = skillInfo.GetValueOrDefault(level, []).get(subLevel) ?? new StatSet();
                skillInfo.GetValueOrDefault(level, []).GetValueOrDefault(-1, new StatSet()).getSet().
                    ForEach(x => statSet.getSet().TryAdd(x.Key, x.Value));

                skillInfo.GetValueOrDefault(-1, []).GetValueOrDefault(-1, new StatSet()).getSet().
                    ForEach(x => statSet.getSet().TryAdd(x.Key, x.Value));

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
                            Assert.Fail(GetType().Name + ": Missing effect for Skill Id[" + statSet.getInt(".id") +
                                "] Level[" + level + "] SubLevel[" + subLevel + "] Effect Scope[" + effectScope +
                                "] Effect Name[" + effectName + "]");
                        }
                    }
                    catch (Exception e)
                    {
                        Assert.Fail(GetType().Name + ": Failed loading effect for Skill Id[" + statSet.getInt(".id") +
                            "] Level[" +
                            level + "] SubLevel[" + subLevel + "] Effect Scope[" + effectScope + "] Effect Name[" +
                            effectName + "]: " + e);
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
                                    Assert.Fail(GetType().Name + ": Non passive condition for passive Skill Id[" +
                                        statSet.getInt(".id") + "] Level[" + level + "] SubLevel[" + subLevel +
                                        "]");
                                }
                            }
                            else if (skillConditionScope == SkillConditionScope.PASSIVE)
                            {
                                Assert.Fail(GetType().Name + ": Passive condition for non passive Skill Id[" +
                                    statSet.getInt(".id") + "] Level[" + level + "] SubLevel[" + subLevel + "]");
                            }

                            skill.addCondition(skillConditionScope, conditionFunction(@params));
                        }
                        else
                        {
                            Assert.Fail(GetType().Name + ": Missing condition for Skill Id[" + statSet.getInt(".id") +
                                "] Level[" + level + "] SubLevel[" + subLevel + "] Effect Scope[" +
                                skillConditionScope + "] Effect Name[" + conditionName + "]");
                        }
                    }
                    catch (Exception e)
                    {
                        Assert.Fail(GetType().Name + ": Failed loading condition for Skill Id[" +
                            statSet.getInt(".id") +
                            "] Level[" + level + "] SubLevel[" + subLevel + "] Condition Scope[" + skillConditionScope +
                            "] Condition Name[" + conditionName + "]: " + e);
                    }
                });

                _skills.put(GetSkillHashCode(skill), skill);
                _skillsMaxLevel.merge(skill.getId(), skill.getLevel(), Math.Max);
                if (skill.getSubLevel() % 1000 == 1)
                {
                    EnchantSkillGroupsData.getInstance().
                        addRouteForSkill(skill.getId(), skill.getLevel(), skill.getSubLevel());
                }
            }));
        }

        private static void ForEachNamedParamInfoParam<T>(Map<T, List<NamedParamInfo>> paramInfo, int level,
            int subLevel,
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

                    namedParamInfo.Info.GetValueOrDefault(level, []).GetValueOrDefault(-1, new StatSet()).getSet().
                        ForEach(x => @params.getSet().TryAdd(x.Key, x.Value));

                    namedParamInfo.Info.GetValueOrDefault(-1, []).GetValueOrDefault(-1, new StatSet()).getSet().
                        ForEach(x => @params.getSet().TryAdd(x.Key, x.Value));

                    @params.set(".name", namedParamInfo.Name);
                    consumer(scope, @params);
                }
            }));
        }

        private NamedParamInfo ParseNamedParamInfo(XElement element,
            Map<string, Map<int, Map<int, object>>> variableValues)
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
                    info.GetOrAdd(level, _ => []).GetOrAdd(subLevel, _ => new StatSet()).
                        set(element.Name.LocalName, value);
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
                                    Map<string, decimal> variables = new();
                                    variables.put("index", i - fromLevel + 1);
                                    variables.put("subIndex", j - fromSubLevel + 1);
                                    object? @base = values.GetValueOrDefault(i)?.GetValueOrDefault(-1);
                                    string baseText = @base?.ToString() ?? string.Empty;
                                    if (@base != null && @base is not StatSet && !baseText.equalsIgnoreCase("true") &&
                                        !baseText.equalsIgnoreCase("false"))
                                    {
                                        variables.put("base", decimal.Parse(baseText, CultureInfo.InvariantCulture));
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

        private static object? ParseValue(XElement element, bool blockValue, bool parseAttributes,
            Map<string, decimal> variables)
        {
            StatSet? statSet = null;
            List<object>? list = null;
            object? text = null;
            if (parseAttributes && (!element.Name.LocalName.equals("value") || !blockValue) &&
                element.Attributes().Any())
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
            Map<string, decimal>? variables = null)
        {
            foreach (XAttribute attribute in element.Attributes())
            {
                string name = attribute.Name.LocalName;
                string value = attribute.Value;
                statSet.set(prefix + "." + name, ParseNodeValue(value, variables));
            }
        }

        private static object ParseNodeValue(string value, Map<string, decimal>? variables)
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
    }


    private static class SkillComparer
    {
        public static void CompareSkills(Map<long, Skill> oldSkills, Map<long, Skill> newSkills)
        {
            List<SkillPair> skillPairs = oldSkills.Values.Select(s => (false, s)).
                Concat(newSkills.Values.Select(s => (true, s))).
                GroupBy(t => new SkillId(t.s.getId(), t.s.getLevel(), t.s.getSubLevel())).
                OrderBy(g => g.Key).
                Select(g => new SkillPair(g.Key.Id, g.Key.Level, g.Key.SubLevel,
                    g.Where(t => !t.Item1).Select(t => t.s).ToList(),
                    g.Where(t => t.Item1).Select(t => t.s).ToList())).
                ToList();

            foreach (SkillPair skillPair in skillPairs)
            {
                if (skillPair.Old.Count != 1 || skillPair.Old.Count != skillPair.New.Count)
                {
                    Assert.Fail($"Skill id={skillPair.Id}, level={skillPair.Level}, sublevel={skillPair.SubLevel}: " +
                        $"old count={skillPair.Old.Count}, new count={skillPair.New.Count}");

                    continue;
                }

                CompareSkill(skillPair.Old[0], skillPair.New[0]);
            }
        }

        private static void CompareSkill(Skill oldSkill, Skill newSkill)
        {
            CompareValue(oldSkill, newSkill, s => s.allowOnTransform());
            CompareValue(oldSkill, newSkill, s => s.canBeDispelled());
            //CompareValue(oldSkill, newSkill, s => s.canBeStolen()); // Uses SkillTreeData
            CompareValue(oldSkill, newSkill, s => s.canDoubleCast());
            CompareValue(oldSkill, newSkill, s => s.canCastWhileDisabled());
            CompareValue(oldSkill, newSkill, s => s.getAbnormalLevel());
            CompareValue(oldSkill, newSkill, s => s.getAbnormalResists());
            CompareValue(oldSkill, newSkill, s => s.getAbnormalTime());
            CompareValue(oldSkill, newSkill, s => s.getAbnormalType());
            CompareValue(oldSkill, newSkill, s => s.getAbnormalVisualEffects());
            CompareValue(oldSkill, newSkill, s => s.getActivateRate());
            //CompareValue(oldSkill, newSkill, s => s.getAffectLimit()); // Random value
            CompareValue(oldSkill, newSkill, s => s.getAffectHeightMax());
            CompareValue(oldSkill, newSkill, s => s.getAffectHeightMin());
            CompareValue(oldSkill, newSkill, s => s.getAffectObject());
            CompareValue(oldSkill, newSkill, s => s.getAffectRange());
            CompareValue(oldSkill, newSkill, s => s.getAffectScope());
            CompareValue(oldSkill, newSkill, s => s.getAttachSkills());
            CompareValue(oldSkill, newSkill, s => s.getAttachToggleGroupId());
            CompareValue(oldSkill, newSkill, s => s.getAttributeType());
            CompareValue(oldSkill, newSkill, s => s.getAttributeValue());
            CompareValue(oldSkill, newSkill, s => s.getBasicProperty());
            CompareValue(oldSkill, newSkill, s => s.getBuffType());
            CompareValue(oldSkill, newSkill, s => s.getCastRange());
            CompareValue(oldSkill, newSkill, s => s.getChannelingSkillId());
            CompareValue(oldSkill, newSkill, s => s.getChannelingTickInitialDelay());
            CompareValue(oldSkill, newSkill, s => s.getChannelingTickInterval());
            CompareValue(oldSkill, newSkill, s => s.getChargeConsumeCount());
            CompareValue(oldSkill, newSkill, s => s.getClanRepConsume());
            CompareValue(oldSkill, newSkill, s => s.getCoolTime());
            CompareValue(oldSkill, newSkill, s => s.getDisplayId());
            CompareValue(oldSkill, newSkill, s => s.getDisplayLevel());
            CompareValue(oldSkill, newSkill, s => s.getDoubleCastSkill());
            CompareValue(oldSkill, newSkill, s => s.getEffectPoint());
            CompareValue(oldSkill, newSkill, s => s.getEffectRange());
            CompareValue(oldSkill, newSkill, s => s.getEffects(EffectScope.GENERAL));
            CompareValue(oldSkill, newSkill, s => s.getEffects(EffectScope.START));
            CompareValue(oldSkill, newSkill, s => s.getEffects(EffectScope.SELF));
            CompareValue(oldSkill, newSkill, s => s.getEffects(EffectScope.CHANNELING));
            CompareValue(oldSkill, newSkill, s => s.getEffects(EffectScope.PVP));
            CompareValue(oldSkill, newSkill, s => s.getEffects(EffectScope.PVE));
            CompareValue(oldSkill, newSkill, s => s.getEffects(EffectScope.END));
            CompareValue(oldSkill, newSkill, s => s.getFamePointConsume());
            CompareValue(oldSkill, newSkill, s => s.getFanRange());
            CompareValue(oldSkill, newSkill, s => s.getHitCancelTime());
            CompareValue(oldSkill, newSkill, s => s.getHitTime());
            CompareValue(oldSkill, newSkill, s => s.getHpConsume());
            CompareValue(oldSkill, newSkill, s => s.getIcon());
            CompareValue(oldSkill, newSkill, s => s.getItemConsumeCount());
            CompareValue(oldSkill, newSkill, s => s.getItemConsumeId());
            CompareValue(oldSkill, newSkill, s => s.getLvlBonusRate());
            CompareValue(oldSkill, newSkill, s => s.getMagicCriticalRate());
            CompareValue(oldSkill, newSkill, s => s.getMagicLevel());
            CompareValue(oldSkill, newSkill, s => s.getMagicType());
            CompareValue(oldSkill, newSkill, s => s.getMaxChance());
            CompareValue(oldSkill, newSkill, s => s.getMaxLightSoulConsumeCount());
            CompareValue(oldSkill, newSkill, s => s.getMaxShadowSoulConsumeCount());
            CompareValue(oldSkill, newSkill, s => s.getMinChance());
            CompareValue(oldSkill, newSkill, s => s.getMinPledgeClass());
            CompareValue(oldSkill, newSkill, s => s.getMpConsume());
            CompareValue(oldSkill, newSkill, s => s.getMpInitialConsume());
            CompareValue(oldSkill, newSkill, s => s.getMpPerChanneling());
            CompareValue(oldSkill, newSkill, s => s.getName());
            CompareValue(oldSkill, newSkill, s => s.getNextAction());
            CompareValue(oldSkill, newSkill, s => s.getOperateType());
            CompareValue(oldSkill, newSkill, s => s.getReferenceItemId());
            CompareValue(oldSkill, newSkill, s => s.getReuseDelay());
            CompareValue(oldSkill, newSkill, s => s.getReuseDelayGroup());
            CompareValue(oldSkill, newSkill, s => s.getReuseHashCode());
            CompareValue(oldSkill, newSkill, s => s.getSubordinationAbnormalType());
            CompareValue(oldSkill, newSkill, s => s.getTargetType());
            CompareValue(oldSkill, newSkill, s => s.getToggleGroupId());
            CompareValue(oldSkill, newSkill, s => s.getTraitType());
            CompareValue(oldSkill, newSkill, s => s.hasAbnormalVisualEffects());
            CompareValue(oldSkill, newSkill, s => s.is7Signs());
            CompareValue(oldSkill, newSkill, s => s.isActive());
            CompareValue(oldSkill, newSkill, s => s.isAura());
            CompareValue(oldSkill, newSkill, s => s.isAbnormalInstant());
            CompareValue(oldSkill, newSkill, s => s.isAOE());
            CompareValue(oldSkill, newSkill, s => s.isBad());
            CompareValue(oldSkill, newSkill, s => s.isBlockActionUseSkill());
            CompareValue(oldSkill, newSkill, s => s.isBlockedInOlympiad());
            CompareValue(oldSkill, newSkill, s => s.isChanneling());
            //CompareValue(oldSkill, newSkill, s => s.isClanSkill()); // Uses SkillTreeData
            CompareValue(oldSkill, newSkill, s => s.isContinuous());
            CompareValue(oldSkill, newSkill, s => s.isDance());
            CompareValue(oldSkill, newSkill, s => s.isDebuff());
            CompareValue(oldSkill, newSkill, s => s.isDeleteAbnormalOnLeave());
            CompareValue(oldSkill, newSkill, s => s.isDisplayInList());
            CompareValue(oldSkill, newSkill, s => s.isEnchantable());
            CompareValue(oldSkill, newSkill, s => s.isExcludedFromCheck());
            CompareValue(oldSkill, newSkill, s => s.isFlyType());
            //CompareValue(oldSkill, newSkill, s => s.isGMSkill()); // Uses SkillTreeData
            //CompareValue(oldSkill, newSkill, s => s.isHeroSkill()); // Uses SkillTreeData
            CompareValue(oldSkill, newSkill, s => s.isHealingPotionSkill());
            CompareValue(oldSkill, newSkill, s => s.isHidingMessages());
            CompareValue(oldSkill, newSkill, s => s.isMagic());
            CompareValue(oldSkill, newSkill, s => s.isMentoring());
            CompareValue(oldSkill, newSkill, s => s.isNecessaryToggle());
            CompareValue(oldSkill, newSkill, s => s.isNotBroadcastable());
            CompareValue(oldSkill, newSkill, s => s.isPassive());
            CompareValue(oldSkill, newSkill, s => s.isPhysical());
            CompareValue(oldSkill, newSkill, s => s.isRecoveryHerb());
            CompareValue(oldSkill, newSkill, s => s.isRemovedOnDamage());
            CompareValue(oldSkill, newSkill, s => s.isRemovedOnUnequipWeapon());
            CompareValue(oldSkill, newSkill, s => s.isRemovedOnAnyActionExceptMove());
            CompareValue(oldSkill, newSkill, s => s.isStatic());
            CompareValue(oldSkill, newSkill, s => s.isSelfContinuous());
            CompareValue(oldSkill, newSkill, s => s.isStaticReuse());
            CompareValue(oldSkill, newSkill, s => s.isSynergySkill());
            CompareValue(oldSkill, newSkill, s => s.isStayAfterDeath());
            CompareValue(oldSkill, newSkill, s => s.isTriggeredSkill());
            CompareValue(oldSkill, newSkill, s => s.isToggle());
            CompareValue(oldSkill, newSkill, s => s.isTransformation());
            CompareValue(oldSkill, newSkill, s => s.isWithoutAction());
            CompareValue(oldSkill, newSkill, s => s.useFishShot());
            CompareValue(oldSkill, newSkill, s => s.useSoulShot());
            CompareValue(oldSkill, newSkill, s => s.useSpiritShot());
        }

        private static void CompareValue<T>(Skill oldSkill, Skill newSkill, Expression<Func<Skill, FrozenSet<T>>> func)
        {
            Func<Skill, FrozenSet<T>> f = func.Compile();
            FrozenSet<T> oldValue = f(oldSkill);
            FrozenSet<T> newValue = f(newSkill);

            bool equal = oldValue.Order().SequenceEqual(newValue.Order());

            if (!equal)
            {
                Assert.Fail(
                    $"Skill id={oldSkill.getId()}, level={oldSkill.getLevel()}, sublevel={oldSkill.getSubLevel()}" +
                    $": property '{func}' old values '{string.Join(", ", oldValue.Order())}', " +
                    $"new values '{string.Join(", ", newValue.Order())}'");
            }
        }

        private static void CompareValue<T>(Skill oldSkill, Skill newSkill, Expression<Func<Skill, T>> func)
        {
            Func<Skill, T> f = func.Compile();
            T oldValue = f(oldSkill);
            T newValue = f(newSkill);

            string index = string.Empty;
            bool equal;
            if (oldValue is IEnumerable oldEnumerable && newValue is IEnumerable newEnumerable)
            {
                List<object?> oldEnum = oldEnumerable.Cast<object?>().ToList();
                List<object?> newEnum = newEnumerable.Cast<object?>().ToList();
                equal = oldEnum.SequenceEqual(newEnum);
                if (!equal && oldEnum.Count == 1 && newEnum.Count == 1 &&
                    oldEnum[0] is RestorationRandom oldRestorationRandom &&
                    newEnum[0] is RestorationRandom newRestorationRandom)
                {
                    // Allow old values to be approximate because of double rounding
                    equal = oldRestorationRandom.EqualsApproximately(newRestorationRandom);
                }

                if (!equal && oldEnum.Count == newEnum.Count)
                {
                    for (int i = 0; i < oldEnum.Count; i++)
                    {
                        object? oldV = oldEnum[i];
                        object? newV = newEnum[i];
                        if (oldV is null)
                        {
                            if (newV is not null)
                            {
                                index = $"[{i}]";
                                break;
                            }
                        }
                        else if (newV is null)
                        {
                            index = $"[{i}]";
                            break;
                        }
                        else if (!oldV.Equals(newV))
                        {
                            index = $"[{i}]";
                            break;
                        }
                    }
                }
            }
            else
                equal = EqualityComparer<T>.Default.Equals(oldValue, newValue);

            if (!equal)
            {
                if (typeof(T) == typeof(TimeSpan))
                {
                    // Allow old value to be approximate, ending with 9999's for example (double rounding).
                    TimeSpan oldTimeSpan = (TimeSpan)(object)oldValue!;
                    TimeSpan newTimeSpan = (TimeSpan)(object)newValue!;

                    if (newTimeSpan.Ticks.ToString(CultureInfo.InvariantCulture).EndsWith("0000"))
                    {
                        equal = oldTimeSpan - TimeSpan.FromMilliseconds(1) <= newTimeSpan &&
                            newTimeSpan <= oldTimeSpan + TimeSpan.FromMilliseconds(1);
                    }
                }
            }

            if (!equal)
            {
                string? oldValueString;
                if (oldValue is IEnumerable enumerable)
                    oldValueString = string.Join(", ", enumerable.Cast<object?>());
                else
                    oldValueString = oldValue?.ToString();

                string? newValueString;
                if (newValue is IEnumerable enumerable2)
                    newValueString = string.Join(", ", enumerable2.Cast<object?>());
                else
                    newValueString = newValue?.ToString();

                Assert.Fail(
                    $"Skill id={oldSkill.getId()}, level={oldSkill.getLevel()}, sublevel={oldSkill.getSubLevel()}" +
                    $": property '{func}{index}' old value '{oldValueString}', new value '{newValueString}'");
            }
        }

        private readonly record struct SkillId(int Id, int Level, int SubLevel): IComparable<SkillId>
        {
            public int CompareTo(SkillId other) =>
                (Id, Level, SubLevel).CompareTo((other.Id, other.Level, other.SubLevel));
        }

        private readonly record struct SkillPair(int Id, int Level, int SubLevel, List<Skill> Old, List<Skill> New);
    }

    private sealed class NamedParamInfo(
        string name, int? fromLevel, int? toLevel, int? fromSubLevel, int? toSubLevel,
        Map<int, Map<int, StatSet>> info)
    {
        public string Name => name;
        public int? FromLevel => fromLevel;
        public int? ToLevel => toLevel;
        public int? FromSubLevel => fromSubLevel;
        public int? ToSubLevel => toSubLevel;
        public Map<int, Map<int, StatSet>> Info => info;
    }
}