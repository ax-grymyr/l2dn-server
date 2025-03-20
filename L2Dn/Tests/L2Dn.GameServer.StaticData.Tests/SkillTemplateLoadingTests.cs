using System.Collections;
using System.Collections.Frozen;
using System.Collections.Immutable;
using System.Globalization;
using System.Xml.Linq;
using L2Dn.Extensions;
using L2Dn.GameServer.Data.Xml;
using L2Dn.GameServer.Enums;
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
        private readonly Map<long, OldSkill> _skills = new();

        public Map<long, OldSkill> Skills => _skills;

        public void Load()
        {
            LoadXmlDocuments(DataFileLocation.Data, "stats/skills").ForEach(t =>
            {
                t.Document.Elements("list").Elements("skill").ForEach(LoadElement);
            });

            if (Configuration.Config.General.CUSTOM_SKILLS_LOAD)
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

        private static long GetSkillHashCode(OldSkill skill)
        {
            return GetSkillHashCode(skill.Id, skill.getLevel(), skill.getSubLevel());
        }

        private void LoadElement(XElement element)
        {
            Map<int, Set<int>> levels = new(); // key - level, value - sublevel set
            Map<int, Map<int, StatSet>> skillInfo = new();
            StatSet generalSkillInfo = skillInfo.GetOrAdd(-1, _ => []).GetOrAdd(-1, _ => new StatSet());
            ParseAttributes(element, string.Empty, generalSkillInfo);

            Map<string, Map<int, Map<int, object>>> variableValues = new(); // key - name
            Map<SkillEffectScope, List<NamedParamInfo>> effectParamInfo = new();
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
                        SkillEffectScope? effectScope = FindEffectScopeByName(skillNodeName);
                        if (effectScope != null)
                        {
                            skillNode.Elements("effect").ForEach(effectsNode =>
                            {
                                effectParamInfo.GetOrAdd(effectScope.Value, _ => []).
                                    Add(ParseNamedParamInfo(effectsNode, variableValues));
                            });

                            break;
                        }

                        SkillConditionScope? skillConditionScope = FindConditionScopeByXmlName(skillNodeName);
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

                    int? fromLevel2 = namedParamInfo.FromLevel;
                    int? toLevel2 = namedParamInfo.ToLevel;
                    if (fromLevel2 != null && toLevel2 != null)
                    {
                        for (int i = fromLevel2.Value; i <= toLevel2.Value; i++)
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
                OldSkill skill = new(statSet);
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
                                if (skillConditionScope != SkillConditionScope.Passive)
                                {
                                    Assert.Fail(GetType().Name + ": Non passive condition for passive Skill Id[" +
                                        statSet.getInt(".id") + "] Level[" + level + "] SubLevel[" + subLevel +
                                        "]");
                                }
                            }
                            else if (skillConditionScope == SkillConditionScope.Passive)
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
                if (skill.getSubLevel() % 1000 == 1)
                {
                    EnchantSkillGroupsData.getInstance().
                        addRouteForSkill(skill.Id, skill.getLevel(), skill.getSubLevel());
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
                        list ??= new();

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
        public static void CompareSkills(Map<long, OldSkill> oldSkills, Map<long, Skill> newSkills)
        {
            List<SkillPair> skillPairs =
                oldSkills.Values.Select(s => new SkillPair(s.Id, s.getLevel(), s.getSubLevel(), [s], [])).Concat(
                        newSkills.Values.Select(s => new SkillPair(s.Id, s.Level, s.SubLevel, [], [s]))).
                    GroupBy(sp => new SkillId(sp.Id, sp.Level, sp.SubLevel)).
                    Select(g => new SkillPair(g.Key.Id, g.Key.Level, g.Key.SubLevel,
                        g.SelectMany(sp => sp.Old).ToList(), g.SelectMany(sp => sp.New).ToList())).ToList();

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

        private static void CompareSkill(OldSkill oldSkill, Skill newSkill)
        {
            CompareValue(oldSkill, "allowOnTransform", oldSkill.allowOnTransform(), newSkill.AllowOnTransform);
            CompareValue(oldSkill, "canBeDispelled", oldSkill.canBeDispelled(), newSkill.CanBeDispelled);
            CompareValue(oldSkill, "canDoubleCast", oldSkill.canDoubleCast(), newSkill.CanDoubleCast);
            CompareValue(oldSkill, "canCastWhileDisabled", oldSkill.canCastWhileDisabled(), newSkill.CanCastWhileDisabled);
            CompareValue(oldSkill, "getAbnormalLevel", oldSkill.getAbnormalLevel(), newSkill.AbnormalLevel);
            CompareValue(oldSkill, "getAbnormalResists", oldSkill.getAbnormalResists(), newSkill.AbnormalResists);
            CompareValue(oldSkill, "getAbnormalTime", oldSkill.getAbnormalTime(), newSkill.AbnormalTime);
            CompareValue(oldSkill, "getAbnormalType", oldSkill.getAbnormalType(), newSkill.AbnormalType);
            CompareValue(oldSkill, "getAbnormalVisualEffects", oldSkill.getAbnormalVisualEffects(), newSkill.AbnormalVisualEffects);
            CompareValue(oldSkill, "getActivateRate", oldSkill.getActivateRate(), newSkill.ActivateRate);
            CompareValue(oldSkill, "getAffectHeightMax", oldSkill.getAffectHeightMax(), newSkill.AffectHeightMax);
            CompareValue(oldSkill, "getAffectHeightMin", oldSkill.getAffectHeightMin(), newSkill.AffectHeightMin);
            CompareValue(oldSkill, "getAffectObject", oldSkill.getAffectObject(), newSkill.AffectObject);
            CompareValue(oldSkill, "getAffectRange", oldSkill.getAffectRange(), newSkill.AffectRange);
            CompareValue(oldSkill, "getAffectScope", oldSkill.getAffectScope(), newSkill.AffectScope);
            CompareValue(oldSkill, "getAttachSkills", oldSkill.getAttachSkills(), newSkill.AttachSkills);
            CompareValue(oldSkill, "getAttachToggleGroupId", oldSkill.getAttachToggleGroupId(), newSkill.AttachToggleGroupId);
            CompareValue(oldSkill, "getAttributeType", oldSkill.getAttributeType(), newSkill.AttributeType);
            CompareValue(oldSkill, "getAttributeValue", oldSkill.getAttributeValue(), newSkill.AttributeValue);
            CompareValue(oldSkill, "getBasicProperty", oldSkill.getBasicProperty(), newSkill.BasicProperty);
            CompareValue(oldSkill, "getBuffType", oldSkill.getBuffType(), newSkill.BuffType);
            CompareValue(oldSkill, "getCastRange", oldSkill.getCastRange(), newSkill.CastRange);
            CompareValue(oldSkill, "getChannelingSkillId", oldSkill.getChannelingSkillId(), newSkill.ChannelingSkillId);
            CompareValue(oldSkill, "getChannelingTickInitialDelay", oldSkill.getChannelingTickInitialDelay(), newSkill.ChannelingTickInitialDelay);
            CompareValue(oldSkill, "getChannelingTickInterval", oldSkill.getChannelingTickInterval(), newSkill.ChannelingTickInterval);
            CompareValue(oldSkill, "getChargeConsumeCount", oldSkill.getChargeConsumeCount(), newSkill.ChargeConsumeCount);
            CompareValue(oldSkill, "getClanRepConsume", oldSkill.getClanRepConsume(), newSkill.ClanRepConsume);
            CompareValue(oldSkill, "getCoolTime", oldSkill.getCoolTime(), newSkill.CoolTime);
            CompareValue(oldSkill, "getDisplayId", oldSkill.getDisplayId(), newSkill.DisplayId);
            CompareValue(oldSkill, "getDisplayLevel", oldSkill.getDisplayLevel(), newSkill.DisplayLevel);
            CompareValue(oldSkill, "getDoubleCastSkill", oldSkill.getDoubleCastSkill(), newSkill.DoubleCastSkill);
            CompareValue(oldSkill, "getEffectPoint", oldSkill.getEffectPoint(), newSkill.EffectPoint);
            CompareValue(oldSkill, "getEffectRange", oldSkill.getEffectRange(), newSkill.EffectRange);
            CompareValue(oldSkill, "getEffects[General]", oldSkill.getEffects(SkillEffectScope.General)?.ToImmutableArray(), newSkill.GetEffects(SkillEffectScope.General));
            CompareValue(oldSkill, "getEffects[Start]", oldSkill.getEffects(SkillEffectScope.Start)?.ToImmutableArray(), newSkill.GetEffects(SkillEffectScope.Start));
            CompareValue(oldSkill, "getEffects[Self]", oldSkill.getEffects(SkillEffectScope.Self)?.ToImmutableArray(), newSkill.GetEffects(SkillEffectScope.Self));
            CompareValue(oldSkill, "getEffects[Channeling]", oldSkill.getEffects(SkillEffectScope.Channeling)?.ToImmutableArray(), newSkill.GetEffects(SkillEffectScope.Channeling));
            CompareValue(oldSkill, "getEffects[Pvp]", oldSkill.getEffects(SkillEffectScope.Pvp)?.ToImmutableArray(), newSkill.GetEffects(SkillEffectScope.Pvp));
            CompareValue(oldSkill, "getEffects[Pve]", oldSkill.getEffects(SkillEffectScope.Pve)?.ToImmutableArray(), newSkill.GetEffects(SkillEffectScope.Pve));
            CompareValue(oldSkill, "getEffects[End]", oldSkill.getEffects(SkillEffectScope.End)?.ToImmutableArray(), newSkill.GetEffects(SkillEffectScope.End));
            CompareValue(oldSkill, "getFamePointConsume", oldSkill.getFamePointConsume(), newSkill.FamePointConsume);
            CompareValue(oldSkill, "getFanRange", new ReadOnlySpan<int>(oldSkill.getFanRange()), newSkill.FanRange);
            CompareValue(oldSkill, "getHitCancelTime", oldSkill.getHitCancelTime(), newSkill.HitCancelTime);
            CompareValue(oldSkill, "getHitTime", oldSkill.getHitTime(), newSkill.HitTime);
            CompareValue(oldSkill, "getHpConsume", oldSkill.getHpConsume(), newSkill.HpConsume);
            CompareValue(oldSkill, "getIcon", oldSkill.getIcon(), newSkill.Icon);
            CompareValue(oldSkill, "getItemConsumeCount", oldSkill.getItemConsumeCount(), newSkill.ItemConsumeCount);
            CompareValue(oldSkill, "getItemConsumeId", oldSkill.getItemConsumeId(), newSkill.ItemConsumeId);
            CompareValue(oldSkill, "getLvlBonusRate", oldSkill.getLvlBonusRate(), newSkill.LevelBonusRate);
            CompareValue(oldSkill, "getMagicCriticalRate", oldSkill.getMagicCriticalRate(), newSkill.MagicCriticalRate);
            CompareValue(oldSkill, "getMagicLevel", oldSkill.getMagicLevel(), newSkill.MagicLevel);
            CompareValue(oldSkill, "getMagicType", oldSkill.getMagicType(), (int)newSkill.MagicType);
            CompareValue(oldSkill, "getMaxChance", oldSkill.getMaxChance(), newSkill.MaxChance);
            CompareValue(oldSkill, "getMaxLightSoulConsumeCount", oldSkill.getMaxLightSoulConsumeCount(), newSkill.MaxLightSoulConsumeCount);
            CompareValue(oldSkill, "getMaxShadowSoulConsumeCount", oldSkill.getMaxShadowSoulConsumeCount(), newSkill.MaxShadowSoulConsumeCount);
            CompareValue(oldSkill, "getMinChance", oldSkill.getMinChance(), newSkill.MinChance);
            CompareValue(oldSkill, "getMinPledgeClass", oldSkill.getMinPledgeClass(), newSkill.MinPledgeClass);
            CompareValue(oldSkill, "getMpConsume", oldSkill.getMpConsume(), newSkill.MpConsume);
            CompareValue(oldSkill, "getMpInitialConsume", oldSkill.getMpInitialConsume(), newSkill.MpInitialConsume);
            CompareValue(oldSkill, "getMpPerChanneling", oldSkill.getMpPerChanneling(), newSkill.MpPerChanneling);
            CompareValue(oldSkill, "getName", oldSkill.getName(), newSkill.Name);
            CompareValue(oldSkill, "getNextAction", oldSkill.getNextAction(), newSkill.NextAction);
            CompareValue(oldSkill, "getOperateType", oldSkill.getOperateType(), newSkill.OperateType);
            CompareValue(oldSkill, "getReferenceItemId", oldSkill.getReferenceItemId(), newSkill.ReferenceItemId);
            CompareValue(oldSkill, "getReuseDelay", oldSkill.getReuseDelay(), newSkill.ReuseDelay);
            CompareValue(oldSkill, "getReuseDelayGroup", oldSkill.getReuseDelayGroup(), newSkill.ReuseDelayGroup);
            CompareValue(oldSkill, "getReuseHashCode", oldSkill.getReuseHashCode(), newSkill.ReuseHashCode);
            CompareValue(oldSkill, "getSubordinationAbnormalType", oldSkill.getSubordinationAbnormalType(), newSkill.SubordinationAbnormalType);
            CompareValue(oldSkill, "getTargetType", oldSkill.getTargetType(), newSkill.TargetType);
            CompareValue(oldSkill, "getToggleGroupId", oldSkill.getToggleGroupId(), newSkill.ToggleGroupId);
            CompareValue(oldSkill, "getTraitType", oldSkill.getTraitType(), newSkill.TraitType);
            CompareValue(oldSkill, "hasAbnormalVisualEffects", oldSkill.hasAbnormalVisualEffects(), newSkill.HasAbnormalVisualEffects);
            CompareValue(oldSkill, "is7Signs", oldSkill.is7Signs(), newSkill.Is7Signs);
            CompareValue(oldSkill, "isActive", oldSkill.isActive(), newSkill.IsActive);
            CompareValue(oldSkill, "isAura", oldSkill.isAura(), newSkill.IsAura);
            CompareValue(oldSkill, "isAbnormalInstant", oldSkill.isAbnormalInstant(), newSkill.IsAbnormalInstant);
            CompareValue(oldSkill, "isAOE", oldSkill.isAOE(), newSkill.IsAoe);
            CompareValue(oldSkill, "isBad", oldSkill.isBad(), newSkill.IsBad);
            CompareValue(oldSkill, "isBlockActionUseSkill", oldSkill.isBlockActionUseSkill(), newSkill.IsBlockActionUseSkill);
            CompareValue(oldSkill, "isBlockedInOlympiad", oldSkill.isBlockedInOlympiad(), newSkill.IsBlockedInOlympiad);
            CompareValue(oldSkill, "isChanneling", oldSkill.isChanneling(), newSkill.IsChanneling);
            CompareValue(oldSkill, "isContinuous", oldSkill.isContinuous(), newSkill.IsContinuous);
            CompareValue(oldSkill, "isDance", oldSkill.isDance(), newSkill.IsDance);
            CompareValue(oldSkill, "isDebuff", oldSkill.isDebuff(), newSkill.IsDebuff);
            CompareValue(oldSkill, "isDeleteAbnormalOnLeave", oldSkill.isDeleteAbnormalOnLeave(), newSkill.IsDeleteAbnormalOnLeave);
            CompareValue(oldSkill, "isDisplayInList", oldSkill.isDisplayInList(), newSkill.IsDisplayInList);
            CompareValue(oldSkill, "isExcludedFromCheck", oldSkill.isExcludedFromCheck(), newSkill.IsExcludedFromCheck);
            CompareValue(oldSkill, "isFlyType", oldSkill.isFlyType(), newSkill.IsFlyType);
            CompareValue(oldSkill, "isHealingPotionSkill", oldSkill.isHealingPotionSkill(), newSkill.IsHealingPotionSkill);
            CompareValue(oldSkill, "isHidingMessages", oldSkill.isHidingMessages(), newSkill.IsHidingMessages);
            CompareValue(oldSkill, "isMagic", oldSkill.isMagic(), newSkill.IsMagic);
            CompareValue(oldSkill, "isMentoring", oldSkill.isMentoring(), newSkill.IsMentoring);
            CompareValue(oldSkill, "isNecessaryToggle", oldSkill.isNecessaryToggle(), newSkill.IsNecessaryToggle);
            CompareValue(oldSkill, "isNotBroadcastable", oldSkill.isNotBroadcastable(), newSkill.IsNotBroadcastable);
            CompareValue(oldSkill, "isPassive", oldSkill.isPassive(), newSkill.IsPassive);
            CompareValue(oldSkill, "isPhysical", oldSkill.isPhysical(), newSkill.IsPhysical);
            CompareValue(oldSkill, "isRecoveryHerb", oldSkill.isRecoveryHerb(), newSkill.IsRecoveryHerb);
            CompareValue(oldSkill, "isRemovedOnDamage", oldSkill.isRemovedOnDamage(), newSkill.IsRemovedOnDamage);
            CompareValue(oldSkill, "isRemovedOnUnequipWeapon", oldSkill.isRemovedOnUnequipWeapon(), newSkill.IsRemovedOnUnequipWeapon);
            CompareValue(oldSkill, "isRemovedOnAnyActionExceptMove", oldSkill.isRemovedOnAnyActionExceptMove(), newSkill.IsRemovedOnAnyActionExceptMove);
            CompareValue(oldSkill, "isStatic", oldSkill.isStatic(), newSkill.IsStatic);
            CompareValue(oldSkill, "isSelfContinuous", oldSkill.isSelfContinuous(), newSkill.IsSelfContinuous);
            CompareValue(oldSkill, "isStaticReuse", oldSkill.isStaticReuse(), newSkill.IsStaticReuse);
            CompareValue(oldSkill, "isSynergySkill", oldSkill.isSynergySkill(), newSkill.IsSynergy);
            CompareValue(oldSkill, "isStayAfterDeath", oldSkill.isStayAfterDeath(), newSkill.IsStayAfterDeath);
            CompareValue(oldSkill, "isTriggeredSkill", oldSkill.isTriggeredSkill(), newSkill.IsTriggeredSkill);
            CompareValue(oldSkill, "isToggle", oldSkill.isToggle(), newSkill.IsToggle);
            CompareValue(oldSkill, "isTransformation", oldSkill.isTransformation(), newSkill.IsTransformation);
            CompareValue(oldSkill, "isWithoutAction", oldSkill.isWithoutAction(), newSkill.IsWithoutAction);
            CompareValue(oldSkill, "useFishShot", oldSkill.useFishShot(), newSkill.UseFishShot);
            CompareValue(oldSkill, "useSoulShot", oldSkill.useSoulShot(), newSkill.UseSoulShot);
            CompareValue(oldSkill, "useSpiritShot", oldSkill.useSpiritShot(), newSkill.UseSpiritShot);
        }

        private static void CompareValue<T>(OldSkill skill, string propertyName, FrozenSet<T> oldValue,
            FrozenSet<T> newValue)
        {
            bool equal = oldValue.Order().SequenceEqual(newValue.Order());
            if (!equal)
            {
                Assert.Fail($"Skill id={skill.Id}, level={skill.getLevel()}, sublevel={skill.getSubLevel()}" +
                    $": property '{propertyName}' old values '{string.Join(", ", oldValue.Order())}', " +
                    $"new values '{string.Join(", ", newValue.Order())}'");
            }
        }

        private static void CompareValue<T>(OldSkill skill, string propertyName, ReadOnlySpan<T> oldValue,
            ReadOnlySpan<T> newValue)
        {
            CompareValue(skill, propertyName + ".Count", oldValue.Length, newValue.Length);
            if (oldValue.Length == newValue.Length)
            {
                for (int i = 0; i < oldValue.Length; i++)
                    CompareValue(skill, $"{propertyName}[{i}]", oldValue[i], newValue[i]);
            }
        }

        private static void CompareValue<T>(OldSkill skill, string propertyName, T oldValue, T newValue)
        {
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
                    $"Skill id={skill.Id}, level={skill.getLevel()}, sublevel={skill.getSubLevel()}" +
                    $": property '{propertyName}{index}' old value '{oldValueString}', new value '{newValueString}'");
            }
        }

        private readonly record struct SkillId(int Id, int Level, int SubLevel): IComparable<SkillId>
        {
            public int CompareTo(SkillId other) =>
                (Id, Level, SubLevel).CompareTo((other.Id, other.Level, other.SubLevel));
        }

        private readonly record struct SkillPair(int Id, int Level, int SubLevel, List<OldSkill> Old, List<Skill> New);
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

    private static string GetName(SkillConditionScope skillConditionScope) =>
        skillConditionScope switch
        {
            SkillConditionScope.General => "conditions",
            SkillConditionScope.Target => "targetConditions",
            SkillConditionScope.Passive => "passiveConditions",
            _ => throw new ArgumentOutOfRangeException(nameof(skillConditionScope)),
        };

    private static SkillConditionScope? FindConditionScopeByXmlName(string name)
    {
        foreach (SkillConditionScope value in EnumUtil.GetValues<SkillConditionScope>())
        {
            if (GetName(value) == name)
                return value;
        }

        return null;
    }

    private static string GetName(SkillEffectScope effectScope) =>
        effectScope switch
        {
            SkillEffectScope.General => "effects",
            SkillEffectScope.Start => "startEffects",
            SkillEffectScope.Self => "selfEffects",
            SkillEffectScope.Channeling => "channelingEffects",
            SkillEffectScope.Pvp => "pvpEffects",
            SkillEffectScope.Pve => "pveEffects",
            SkillEffectScope.End => "endEffects",
            _ => throw new ArgumentOutOfRangeException(nameof(effectScope)),
        };

    private static SkillEffectScope? FindEffectScopeByName(string name)
    {
        foreach (SkillEffectScope effectScope in EnumUtil.GetValues<SkillEffectScope>())
        {
            if (string.Equals(name, GetName(effectScope)))
                return effectScope;
        }

        return null;
    }
}