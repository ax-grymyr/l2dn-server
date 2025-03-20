using System.Collections.Frozen;
using System.Globalization;
using System.Xml.Serialization;
using L2Dn.Collections;
using L2Dn.Extensions;
using L2Dn.GameServer.Configuration;
using L2Dn.GameServer.Enums;
using L2Dn.GameServer.Handlers;
using L2Dn.GameServer.Model.Skills;
using L2Dn.GameServer.StaticData;
using L2Dn.GameServer.StaticData.Xml.Skills;
using L2Dn.GameServer.Utilities;
using L2Dn.Parsing;
using NLog;

namespace L2Dn.GameServer.Data.Xml;

/// <summary>
/// Skill data parser.
/// </summary>
public sealed class SkillData: DataReaderBase
{
    private static readonly Logger _logger = LogManager.GetLogger(nameof(SkillData));

    private FrozenDictionary<long, Skill> _skills = FrozenDictionary<long, Skill>.Empty;
    private FrozenDictionary<int, int> _skillsMaxLevel = FrozenDictionary<int, int>.Empty;

    private SkillData()
    {
        Load();
    }

    public static SkillData Instance { get; } = new();

    public FrozenDictionary<long, Skill> Skills => _skills;

    public int GetMaxLevel(int skillId) => _skillsMaxLevel.GetValueOrDefault(skillId);

    public Skill? GetSkill(int skillId, int level, int subLevel = 0)
    {
        Skill? result = _skills.GetValueOrDefault(Skill.GetSkillHashCode(skillId, level, subLevel));
        if (result != null)
            return result;

        // skill/level not found, fix for transformation scripts
        int maxLevel = GetMaxLevel(skillId);

        // requested level too high
        if (maxLevel > 0 && level > maxLevel)
        {
            _logger.Warn($"{nameof(SkillData)}: Call to unexisting skill level id {skillId} requested level {level}, " +
                $"max level {maxLevel}.");

            return _skills.GetValueOrDefault(Skill.GetSkillHashCode(skillId, maxLevel));
        }

        _logger.Warn($"{nameof(SkillData)}: No skill info found for skill id {skillId} and skill level {level}");
        return null;
    }

    /**
     * @param addNoble
     * @param hasCastle
     * @return an array with siege skills. If addNoble == true, will add also Advanced headquarters.
     */
    public List<Skill> GetSiegeSkills(bool addNoble, bool hasCastle)
    {
        List<Skill?> temp =
        [
            _skills.GetValueOrDefault(Skill.GetSkillHashCode((int)CommonSkill.SEAL_OF_RULER, 1)),
            _skills.GetValueOrDefault(Skill.GetSkillHashCode(247, 1)), // Build Headquarters
        ];

        if (addNoble)
        {
            temp.Add(_skills.GetValueOrDefault(Skill.GetSkillHashCode(326, 1))); // Build Advanced Headquarters
        }

        if (hasCastle)
        {
            temp.Add(_skills.GetValueOrDefault(Skill.GetSkillHashCode(844, 1))); // Outpost Construction
            temp.Add(_skills.GetValueOrDefault(Skill.GetSkillHashCode(845, 1))); // Outpost Demolition
        }

        return temp.Where(s => s != null).ToList()!; // TODO: review code and refactor
    }

    public void Load()
    {
        IEnumerable<(string FilePath, XmlSkillList Document)> skillLists =
            LoadXmlDocuments<XmlSkillList>(DataFileLocation.Data, "stats/skills");

        if (Config.General.CUSTOM_SKILLS_LOAD)
        {
            skillLists = skillLists.Concat(LoadXmlDocuments<XmlSkillList>(DataFileLocation.Data,
                "stats/skills/custom"));
        }

        _skills = skillLists.SelectMany(pair => pair.Document.Skills).SelectMany(LoadSkill).
            ToFrozenDictionary(skill => skill.GetSkillHashCode());

        _skillsMaxLevel = _skills.Values.GroupBy(skill => skill.Id).
            ToFrozenDictionary(g => g.Key, g => g.Max(s => s.Level));

        _logger.Info(GetType().Name + ": Loaded " + _skills.Count + " Skills.");
    }

    private static IEnumerable<Skill> LoadSkill(XmlSkill xmlSkill)
    {
        SkillDataParser parser = new(xmlSkill);
        for (int level = 1; level <= parser.MaxLevel; level++)
        {
            SortedSet<int> subLevels = parser.GetSubLevelsForLevel(level);
            foreach (int subLevel in subLevels)
            {
                SkillParameters parameters = parser.GetParameters(level, subLevel);
                yield return new Skill(parameters);
            }
        }
    }

    private sealed class SkillDataParser
    {
        private readonly int _skillId;
        private readonly string _skillName;
        private readonly int? _skillDisplayId;
        private readonly int? _skillDisplayLevel;
        private readonly int? _referenceId;
        private readonly Dictionary<string, List<Variable>> _variables = [];
        private readonly Dictionary<XmlSkillParameterType, List<Parameter>> _parameters = [];
        private readonly Dictionary<SkillConditionScope, List<Condition>> _conditions = [];
        private readonly Dictionary<SkillEffectScope, List<Effect>> _effects = [];
        private readonly int _maxLevel;

        public SkillDataParser(XmlSkill xmlSkill)
        {
            _skillId = xmlSkill.Id;
            _skillName = xmlSkill.Name;
            _skillDisplayId = xmlSkill.DisplayIdSpecified ? xmlSkill.DisplayId : null;
            _skillDisplayLevel = xmlSkill.DisplayLevelSpecified ? xmlSkill.DisplayLevel : null;
            _referenceId = xmlSkill.ReferenceIdSpecified ? xmlSkill.ReferenceId : null;
            CollectSkillParameters(xmlSkill);
            _maxLevel = GetMaxLevel(xmlSkill.ToLevel);
        }

        public int MaxLevel => _maxLevel;

        public SortedSet<int> GetSubLevelsForLevel(int level)
        {
            SortedSet<int> subLevels = [0];
            _variables.Values.SelectMany(x => x).ForEach(x => x.GetSubLevels(subLevels, level));
            _parameters.Values.SelectMany(x => x).ForEach(x => x.GetSubLevels(subLevels, level));
            _conditions.Values.SelectMany(x => x).ForEach(x => x.GetSubLevels(subLevels, level));
            _effects.Values.SelectMany(x => x).ForEach(x => x.GetSubLevels(subLevels, level));
            return subLevels;
        }

        public SkillParameters GetParameters(int level, int subLevel)
        {
            SkillParameters parameters = new()
            {
                Id = _skillId,
                Level = level,
                SubLevel = subLevel,
                Name = _skillName,
                DisplayId = _skillDisplayId,
                DisplayLevel = _skillDisplayLevel,
                ReferenceId = _referenceId,
            };

            // Calculate variables
            Dictionary<string, decimal> variables = [];
            foreach ((string variableName, List<Variable> variableList) in _variables)
            {
                FilterByLevel(variableList, level, subLevel).
                    ForEach(v =>
                    {
                        object value = ParseValue(variables, v, level, subLevel, v.Value);
                        if (value is not decimal decimalValue && !decimal.TryParse(value.ToString(),
                                CultureInfo.InvariantCulture, out decimalValue))
                        {
                            throw new InvalidOperationException($"Invalid skill id={_skillId} definition: invalid " +
                                $"variable '{variableName}' value '{value}'");
                        }

                        variables[variableName] = decimalValue;
                    });
            }

            // Calculate parameters
            foreach ((XmlSkillParameterType type, List<Parameter> parameterList) in _parameters)
            {
                FilterByLevel(parameterList, level, subLevel).
                    ForEach(parameter =>
                        parameters.Parameters[type] =
                            ParseValue(variables, parameter, level, subLevel, parameter.Value));
            }

            // Calculate conditions
            foreach ((SkillConditionScope conditionScope, List<Condition> conditionList) in _conditions)
            {
                FilterByLevel(conditionList, level, subLevel, false).ForEach(condition =>
                {
                    // Calculate condition parameters
                    SkillConditionParameterSet conditionParameters = new();
                    if (!string.IsNullOrEmpty(condition.Type))
                        conditionParameters[XmlSkillConditionParameterType.Type] = condition.Type;

                    foreach ((XmlSkillConditionParameterType type, List<Parameter> parameterList) in condition.
                                 Parameters)
                    {
                        FilterByLevel(parameterList, level, subLevel).
                            ForEach(parameter =>
                            {
                                conditionParameters[type] = ParseValue(variables, parameter, level, subLevel,
                                    parameter.Value);
                            });
                    }

                    Func<SkillConditionParameterSet, ISkillConditionBase>? handlerFactory =
                        SkillConditionHandler.getInstance().getHandlerFactory(condition.Name);

                    if (handlerFactory is null)
                        throw new InvalidOperationException(
                            $"Invalid skill id={_skillId}: condition handler '{condition.Name}' not found");

                    ISkillConditionBase skillCondition = handlerFactory(conditionParameters);

                    parameters.Conditions.GetOrAdd(conditionScope, _ => []).Add(skillCondition);
                });
            }

            // Calculate effects
            foreach ((SkillEffectScope effectScope, List<Effect> effectList) in _effects)
            {
                FilterByLevel(effectList, level, subLevel, false).ForEach(effect =>
                {
                    // Calculate condition parameters
                    EffectParameterSet effectParameters = new();
                    foreach ((XmlSkillEffectParameterType type, List<Parameter> parameterList) in effect.Parameters)
                    {
                        FilterByLevel(parameterList, level, subLevel).
                            ForEach(parameter =>
                            {
                                effectParameters[type] = ParseValue(variables, parameter, level, subLevel,
                                    parameter.Value);
                            });
                    }

                    Func<EffectParameterSet, IAbstractEffect>? handlerFactory =
                        EffectHandler.getInstance().getHandlerFactory(effect.Name);

                    if (handlerFactory is null)
                        throw new InvalidOperationException(
                            $"Invalid skill id={_skillId}: effect handler '{effect.Name}' not found");

                    IAbstractEffect skillEffect = handlerFactory(effectParameters);

                    parameters.Effects.GetOrAdd(effectScope, _ => []).Add(skillEffect);
                });
            }

            return parameters;
        }

        private void CollectSkillParameters(XmlSkill xmlSkill)
        {
            if (xmlSkill.ParameterTypes is null || xmlSkill.Parameters is null)
                return;

            foreach ((XmlSkillParameterType type, object value) in xmlSkill.ParameterTypes.Zip(xmlSkill.Parameters))
            {
                switch (value)
                {
                    case XmlSkillVariable xmlSkillVariable:
                    {
                        if (type != XmlSkillParameterType.Variable)
                            throw new InvalidOperationException($"Invalid skill id={_skillId} definition");

                        List<Variable> variables = _variables.GetOrAdd(xmlSkillVariable.Name, _ => []);
                        if (!string.IsNullOrWhiteSpace(xmlSkillVariable.Value))
                            variables.Add(new Variable(null, null, xmlSkillVariable.Value));

                        variables.AddRange(xmlSkillVariable.Values.Select(xmlSkillLevelValue =>
                            new Variable(GetLevelRange(xmlSkillLevelValue), GetSubLevelRange(xmlSkillLevelValue),
                                xmlSkillLevelValue.Value)));

                        break;
                    }

                    case XmlSkillConditionList xmlSkillConditionList:
                    {
                        SkillConditionScope conditionScope = GetConditionScope(type);
                        List<Condition> conditions = _conditions.GetOrAdd(conditionScope, _ => []);

                        foreach (XmlSkillCondition xmlSkillCondition in xmlSkillConditionList.Conditions)
                        {
                            Dictionary<XmlSkillConditionParameterType, List<Parameter>> parameters =
                                CollectSkillConditionParameters(xmlSkillCondition);

                            conditions.Add(new Condition(GetLevelRange(xmlSkillCondition),
                                GetSubLevelRange(xmlSkillCondition), xmlSkillCondition.Name, xmlSkillCondition.Type,
                                parameters));
                        }

                        break;
                    }

                    case XmlSkillEffectList xmlSkillEffectList:
                    {
                        SkillEffectScope effectScope = GetEffectScope(type);
                        List<Effect> effects = _effects.GetOrAdd(effectScope, _ => []);

                        foreach (XmlSkillEffect xmlSkillEffect in xmlSkillEffectList.Effects)
                        {
                            Dictionary<XmlSkillEffectParameterType, List<Parameter>> parameters =
                                CollectSkillEffectParameters(xmlSkillEffect);

                            effects.Add(new Effect(GetLevelRange(xmlSkillEffect), GetSubLevelRange(xmlSkillEffect),
                                xmlSkillEffect.Name, parameters));
                        }

                        break;
                    }

                    case IXmlSkillValue xmlSkillValue:
                    {
                        List<Parameter> parameters = _parameters.GetOrAdd(type, _ => []);
                        if (!string.IsNullOrWhiteSpace(xmlSkillValue.Value))
                            parameters.Add(new Parameter(null, null, xmlSkillValue.Value));

                        parameters.AddRange(xmlSkillValue.Values.Select(xmlSkillLevelValue =>
                            new Parameter(GetLevelRange(xmlSkillLevelValue), GetSubLevelRange(xmlSkillLevelValue),
                                xmlSkillLevelValue.Value)));

                        break;
                    }

                    default:
                        _parameters.GetOrAdd(type, _ => []).Add(new Parameter(null, null, value));
                        break;
                }
            }

            // Sort parameters
            _parameters.Values.ForEach(SortValues);
        }

        private Dictionary<XmlSkillConditionParameterType, List<Parameter>> CollectSkillConditionParameters(
            XmlSkillCondition xmlSkillCondition)
        {
            Dictionary<XmlSkillConditionParameterType, List<Parameter>> parameters = [];
            if (xmlSkillCondition.ParameterTypes is null || xmlSkillCondition.Parameters is null)
                return parameters;

            foreach ((XmlSkillConditionParameterType type, object value) in xmlSkillCondition.ParameterTypes.Zip(
                         xmlSkillCondition.Parameters))
            {
                List<Parameter> parameterList = parameters.GetOrAdd(type, _ => []);
                switch (value)
                {
                    case XmlSkillStringList xmlSkillStringList:
                        parameterList.Add(new Parameter(null, null, xmlSkillStringList.Items));
                        break;

                    case XmlSkillIntList xmlSkillIntList:
                        parameterList.Add(new Parameter(null, null, xmlSkillIntList.Items));
                        break;

                    case IXmlSkillValue xmlSkillValue:
                    {
                        if (!string.IsNullOrWhiteSpace(xmlSkillValue.Value))
                            parameterList.Add(new Parameter(null, null, xmlSkillValue.Value));

                        parameterList.AddRange(xmlSkillValue.Values.Select(xmlSkillLevelValue =>
                            new Parameter(GetLevelRange(xmlSkillLevelValue), GetSubLevelRange(xmlSkillLevelValue),
                                xmlSkillLevelValue.Value)));

                        break;
                    }

                    default:
                        parameterList.Add(new Parameter(null, null, value));
                        break;
                }
            }

            // Sort parameters
            parameters.Values.ForEach(SortValues);

            return parameters;
        }

        private Dictionary<XmlSkillEffectParameterType, List<Parameter>> CollectSkillEffectParameters(
            XmlSkillEffect xmlSkillEffect)
        {
            Dictionary<XmlSkillEffectParameterType, List<Parameter>> parameters = [];
            if (xmlSkillEffect.ParameterTypes is null || xmlSkillEffect.Parameters is null)
                return parameters;

            foreach ((XmlSkillEffectParameterType type, object value) in xmlSkillEffect.ParameterTypes.Zip(
                         xmlSkillEffect.
                             Parameters))
            {
                List<Parameter> parameterList = parameters.GetOrAdd(type, _ => []);
                switch (value)
                {
                    case XmlSkillStringList xmlSkillStringList:
                        parameterList.Add(new Parameter(null, null, xmlSkillStringList.Items));
                        break;

                    case XmlSkillIntList xmlSkillIntList:
                        parameterList.Add(new Parameter(null, null, xmlSkillIntList.Items));
                        break;

                    case XmlSkillEffectItemList xmlSkillEffectItemList:
                    {
                        if (xmlSkillEffectItemList.Items.Count != 0)
                            parameterList.Add(new Parameter(null, null, xmlSkillEffectItemList.Items));

                        parameterList.AddRange(xmlSkillEffectItemList.Values.Select(xmlSkillEffectLevelItemList =>
                            new Parameter(GetLevelRange(xmlSkillEffectLevelItemList),
                                GetSubLevelRange(xmlSkillEffectLevelItemList), xmlSkillEffectLevelItemList.Items)));

                        break;
                    }

                    case IXmlSkillValue xmlSkillValue:
                    {
                        if (!string.IsNullOrWhiteSpace(xmlSkillValue.Value))
                            parameterList.Add(new Parameter(null, null, xmlSkillValue.Value));

                        parameterList.AddRange(xmlSkillValue.Values.Select(xmlSkillLevelValue =>
                            new Parameter(GetLevelRange(xmlSkillLevelValue), GetSubLevelRange(xmlSkillLevelValue),
                                xmlSkillLevelValue.Value)));

                        break;
                    }

                    default:
                        parameterList.Add(new Parameter(null, null, value));
                        break;
                }
            }

            // Sort parameters
            parameters.Values.ForEach(SortValues);

            return parameters;
        }

        private int GetMaxLevel(int toLevel) =>
            new[] { 1, toLevel }.
                Concat(_variables.Values.Select(GetMaxLevel)).
                Concat(_parameters.Values.Select(GetMaxLevel)).
                Concat(_conditions.Values.Select(GetMaxLevel)).
                Concat(_effects.Values.Select(GetMaxLevel)).
                Max();

        private Range<byte>? GetLevelRange(IXmlSkillLevelRestriction restriction)
        {
            if (restriction.LevelSpecified)
            {
                if (restriction.Level == 0)
                    throw new ArgumentOutOfRangeException(nameof(restriction), restriction.Level,
                        $"Invalid skill id={_skillId} definition: invalid level {restriction.Level}");

                if (restriction.FromLevelSpecified || restriction.ToLevelSpecified)
                {
                    throw new ArgumentException($"Invalid skill id={_skillId} definition: level cannot be specified " +
                        "with fromLevel or with toLevel.");
                }

                return Range.Create(restriction.Level, restriction.Level);
            }

            if (restriction.FromLevelSpecified && restriction.ToLevelSpecified)
                return Range.Create(restriction.FromLevel, restriction.ToLevel);

            if (restriction.FromLevelSpecified || restriction.ToLevelSpecified)
            {
                throw new InvalidOperationException($"Invalid skill id={_skillId} definition: fromLevel and toLevel " +
                    "must be both specified or both skipped.");
            }

            return null;
        }

        private Range<ushort>? GetSubLevelRange(IXmlSkillLevelRestriction restriction)
        {
            if (restriction.FromSubLevelSpecified && restriction.ToSubLevelSpecified)
                return Range.Create(restriction.FromSubLevel, restriction.ToSubLevel);

            if (restriction.FromSubLevelSpecified || restriction.ToSubLevelSpecified)
            {
                throw new InvalidOperationException($"Invalid skill id={_skillId} definition: fromSubLevel and " +
                    "toSubLevel must be both specified or both skipped.");
            }

            return null;
        }

        private static SkillConditionScope GetConditionScope(XmlSkillParameterType type) =>
            type switch
            {
                XmlSkillParameterType.Conditions => SkillConditionScope.General,
                XmlSkillParameterType.PassiveConditions => SkillConditionScope.Passive,
                XmlSkillParameterType.TargetConditions => SkillConditionScope.Target,
                _ => throw new ArgumentException($"Invalid condition list type: {type}"),
            };

        private static SkillEffectScope GetEffectScope(XmlSkillParameterType type) =>
            type switch
            {
                XmlSkillParameterType.Effects => SkillEffectScope.General,
                XmlSkillParameterType.ChannelingEffects => SkillEffectScope.Channeling,
                XmlSkillParameterType.SelfEffects => SkillEffectScope.Self,
                XmlSkillParameterType.StartEffects => SkillEffectScope.Start,
                XmlSkillParameterType.EndEffects => SkillEffectScope.End,
                XmlSkillParameterType.PveEffects => SkillEffectScope.Pve,
                XmlSkillParameterType.PvpEffects => SkillEffectScope.Pvp,
                _ => throw new ArgumentException($"Invalid effect list type: {type}"),
            };

        private static long GetSortOrder<T>(T value)
            where T: struct, ILevelRange
        {
            // 1 byte (5) - from level
            // 1 byte (4) - to level
            // 2 bytes (2-3) - from sublevel
            // 2 bytes (0-1) - to sublevel
            long result = 0;
            if (value.LevelRange != null)
                result |= ((long)value.LevelRange.Value.Left << 40) | ((long)value.LevelRange.Value.Right << 32);

            if (value.SubLevelRange != null)
                result |= ((long)value.SubLevelRange.Value.Left << 16) | value.SubLevelRange.Value.Right;

            return result;
        }

        private static void SortValues<T>(List<T> list)
            where T: struct, ILevelRange =>
            list.Sort((v1, v2) => GetSortOrder(v1).CompareFast(GetSortOrder(v2)));

        private static int GetMaxLevel(Range<byte>? levelRange) =>
            levelRange is null ? 1 : Math.Max(levelRange.Value.Left, levelRange.Value.Right);

        private static int GetMaxLevel<T>(IEnumerable<T> values)
            where T: ILevelRange =>
            values.Select(x => x.MaxLevel).Append(1).Max();

        private static bool ContainsLevel(Range<byte>? levelRange, int level) =>
            levelRange is null || (levelRange.Value.Left <= level && levelRange.Value.Right >= level);

        private static bool ContainsSubLevel(Range<ushort>? subLevelRange, int subLevel) =>
            subLevelRange is null || (subLevelRange.Value.Left <= subLevel && subLevelRange.Value.Right >= subLevel);

        private static void CollectSubLevels(SortedSet<int> subLevels, Range<ushort>? subLevelRange)
        {
            if (subLevelRange is null)
                return;

            int right = subLevelRange.Value.Right;
            for (int subLevel = subLevelRange.Value.Left; subLevel <= right; subLevel++)
                subLevels.Add(subLevel);
        }

        private static IEnumerable<T> FilterByLevel<T>(IEnumerable<T> collection, int level, int subLevel,
            bool sort = true)
            where T: ILevelRange
        {
            IEnumerable<T> filtered = collection.Where(x =>
                ContainsLevel(x.LevelRange, level) && ContainsSubLevel(x.SubLevelRange, subLevel));

            if (!sort)
                return filtered;

            return filtered.Select((x, i) => (Item: x, Index: i)).OrderBy(tuple =>
            {
                (T x, int index) = tuple;

                // Order items by the level and sublevel match
                // Lowest priority when level range and sublevel range not defined.
                // Highest priority when level range has a single value and sublevel range has a single value.
                int rank = (x.LevelRange, x.SubLevelRange) switch
                {
                    ({ } levelRange, { } subLevelRange) when levelRange.Left == levelRange.Right &&
                        subLevelRange.Left == subLevelRange.Right => 8,
                    ({ } levelRange, not null) when levelRange.Left == levelRange.Right => 7,
                    (not null, { } subLevelRange) when subLevelRange.Left == subLevelRange.Right => 6,
                    (not null, not null) => 5,
                    ({ } levelRange, null) when levelRange.Left == levelRange.Right => 4,
                    (not null, null) => 3,
                    (null, { } subLevelRange) when subLevelRange.Left == subLevelRange.Right => 2,
                    (null, not null) => 1,
                    (null, null) => 0,
                };

                return ((long)rank << 32) | (uint)index;
            }).Select(t => t.Item);
        }

        private object ParseValue<T>(Dictionary<string, decimal> variables, T levelRange, int currentLevel,
            int currentSubLevel, object value)
            where T: ILevelRange
        {
            if (value is not string s)
                return value;

            s = s.Trim();
            if (s.StartsWith('@'))
            {
                string variableName = s[1..];
                if (!variables.TryGetValue(variableName, out decimal variableValue))
                    throw new InvalidOperationException(
                        $"Invalid skill id={_skillId} definition: variable {variableName} not found");

                return variableValue;
            }

            if (!s.StartsWith('{') || !s.EndsWith('}'))
                return value;

            bool indexVariableExists = false;
            decimal oldIndex = 0;
            bool subIndexVariableExists = false;
            decimal oldSubIndex = 0;
            if (levelRange.LevelRange is not null)
            {
                indexVariableExists = variables.TryGetValue("index", out oldIndex);
                variables["index"] = currentLevel - levelRange.LevelRange.Value.Left + 1;
            }

            if (levelRange.SubLevelRange is not null)
            {
                subIndexVariableExists = variables.TryGetValue("subIndex", out oldSubIndex);
                variables["subIndex"] = currentSubLevel - levelRange.SubLevelRange.Value.Left + 1;
            }

            string expression = s.Substring(1, s.Length - 2);
            ParserResult<Expression> result = ExpressionParser.Parser(expression);
            if (!result.Success)
                throw new InvalidOperationException(
                    $"Invalid skill id={_skillId} definition: invalid expression '{value}'");

            decimal newValue = result.Result.Evaluate(variables);

            // restore old index and subIndex
            if (indexVariableExists)
                variables["index"] = oldIndex;

            if (subIndexVariableExists)
                variables["subIndex"] = oldSubIndex;

            return newValue;

        }

        private interface ILevelRange
        {
            Range<byte>? LevelRange { get; }
            Range<ushort>? SubLevelRange { get; }
            int MaxLevel { get; }
        }

        private readonly record struct Variable(
            Range<byte>? LevelRange, Range<ushort>? SubLevelRange, string Value): ILevelRange
        {
            public int MaxLevel => GetMaxLevel(LevelRange);

            public void GetSubLevels(SortedSet<int> subLevels, int level)
            {
                if (!ContainsLevel(LevelRange, level))
                    return;

                CollectSubLevels(subLevels, SubLevelRange);
            }
        }

        private readonly record struct Parameter(
            Range<byte>? LevelRange, Range<ushort>? SubLevelRange, object Value): ILevelRange
        {
            public int MaxLevel => GetMaxLevel(LevelRange);

            public void GetSubLevels(SortedSet<int> subLevels, int level)
            {
                if (!ContainsLevel(LevelRange, level))
                    return;

                CollectSubLevels(subLevels, SubLevelRange);
            }
        }

        private readonly record struct Condition(
            Range<byte>? LevelRange, Range<ushort>? SubLevelRange, string Name, string Type,
            Dictionary<XmlSkillConditionParameterType, List<Parameter>> Parameters): ILevelRange
        {
            public int MaxLevel =>
                Parameters.Values.SelectMany(x => x).Select(x => x.MaxLevel).Append(GetMaxLevel(LevelRange)).Max();

            public void GetSubLevels(SortedSet<int> subLevels, int level)
            {
                if (!ContainsLevel(LevelRange, level))
                    return;

                CollectSubLevels(subLevels, SubLevelRange);
                Parameters.Values.SelectMany(x => x).ForEach(x => x.GetSubLevels(subLevels, level));
            }
        }

        private readonly record struct Effect(
            Range<byte>? LevelRange, Range<ushort>? SubLevelRange, string Name,
            Dictionary<XmlSkillEffectParameterType, List<Parameter>> Parameters): ILevelRange
        {
            public int MaxLevel =>
                Parameters.Values.SelectMany(x => x).Select(x => x.MaxLevel).Append(GetMaxLevel(LevelRange)).Max();

            public void GetSubLevels(SortedSet<int> subLevels, int level)
            {
                if (!ContainsLevel(LevelRange, level))
                    return;

                CollectSubLevels(subLevels, SubLevelRange);
                Parameters.Values.SelectMany(x => x).ForEach(x => x.GetSubLevels(subLevels, level));
            }
        }
    }
}