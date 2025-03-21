using System.Collections.Immutable;
using L2Dn.Extensions;
using L2Dn.GameServer.Dto;
using L2Dn.GameServer.Enums;
using L2Dn.GameServer.Handlers;
using L2Dn.GameServer.StaticData.Xml.Options;
using L2Dn.GameServer.StaticData.Xml.Skills;
using L2Dn.GameServer.Templates;
using L2Dn.GameServer.Utilities;
using NLog;

namespace L2Dn.GameServer.StaticData;

public sealed class OptionData
{
    private static readonly Logger _logger = LogManager.GetLogger(nameof(OptionData));
    private static ImmutableArray<Option?> _options = ImmutableArray<Option?>.Empty;

    private OptionData()
    {
    }

    public static OptionData Instance { get; } = new();

    public void Load()
    {
        Dictionary<int, Option> optionMap = new();
        XmlLoader.LoadXmlDocuments<XmlOptionData>("stats/augmentation/options").SelectMany(doc => doc.Options).
            ForEach(option => LoadOption(option, optionMap));

        if (optionMap.Count == 0)
            _options = ImmutableArray<Option?>.Empty;
        else
        {
            int maxKey = optionMap.Keys.Max();
            Option?[] options = new Option?[maxKey + 1];
            foreach (KeyValuePair<int, Option> option in optionMap)
                options[option.Key] = option.Value;

            _options = options.ToImmutableArray();
        }

        _logger.Info($"{nameof(OptionData)}: Loaded {_options.Length} options.");
    }

    private void LoadOption(XmlOption xmlOption, Dictionary<int, Option> optionMap)
    {
        int id = xmlOption.Id;
        if (optionMap.ContainsKey(id))
        {
            _logger.Error($"{nameof(OptionData)}: Duplicated option {id}.");
            return;
        }

        List<IAbstractEffect> effects = [];
        foreach (XmlOptionEffect xmlOptionEffect in xmlOption.Effects)
        {
            string name = xmlOptionEffect.Name;
            EffectParameterSet parameters = new()
            {
                [XmlSkillEffectParameterType.Amount] = xmlOptionEffect.Amount,
                [XmlSkillEffectParameterType.Attribute] = xmlOptionEffect.Attribute,
                [XmlSkillEffectParameterType.MagicType] = xmlOptionEffect.MagicType,
                [XmlSkillEffectParameterType.Mode] = xmlOptionEffect.Mode,
                [XmlSkillEffectParameterType.Stat] = xmlOptionEffect.Stat,
            };

            IAbstractEffect? effect = AbstractEffectFactory.Instance.Create(name, parameters);
            if (effect is null)
                _logger.Error($"{nameof(OptionData)}: Could not find effect handler '{name}' used by option {id}.");
            else
                effects.Add(effect);
        }

        List<Skill> activeSkills = [];
        foreach (XmlOptionSkill activeSkill in xmlOption.ActiveSkills)
        {
            Skill? skill = SkillData.Instance.GetSkill(activeSkill.Id, activeSkill.Level);
            if (skill != null)
                activeSkills.Add(skill);
            else
                _logger.Error(nameof(OptionData) + ": Could not find skill " + activeSkill.Id + "(" +
                    activeSkill.Level +
                    ") used by option " + id + ".");
        }

        List<Skill> passiveSkills = [];
        foreach (XmlOptionSkill passiveSkill in xmlOption.PassiveSkills)
        {
            Skill? skill = SkillData.Instance.GetSkill(passiveSkill.Id, passiveSkill.Level);
            if (skill != null)
                passiveSkills.Add(skill);
            else
                _logger.Error(nameof(OptionData) + ": Could not find skill " + passiveSkill.Id + "(" +
                    passiveSkill.Level +
                    ") used by option " + id + ".");
        }

        List<OptionSkillHolder> activationSkills = [];
        foreach (XmlOptionChanceSkill attackSkill in xmlOption.AttackSkills)
        {
            Skill? skill = SkillData.Instance.GetSkill(attackSkill.Id, attackSkill.Level);
            if (skill != null)
                activationSkills.Add(new OptionSkillHolder(skill, attackSkill.Chance, OptionSkillType.Attack));
            else
                _logger.Error(nameof(OptionData) + ": Could not find skill " + attackSkill.Id + "(" +
                    attackSkill.Level +
                    ") used by option " + id + ".");
        }

        foreach (XmlOptionChanceSkill magicSkill in xmlOption.MagicSkills)
        {
            Skill? skill = SkillData.Instance.GetSkill(magicSkill.Id, magicSkill.Level);
            if (skill != null)
                activationSkills.Add(new OptionSkillHolder(skill, magicSkill.Chance, OptionSkillType.Magic));
            else
                _logger.Error(nameof(OptionData) + ": Could not find skill " + magicSkill.Id + "(" + magicSkill.Level +
                    ") used by option " + id + ".");
        }

        foreach (XmlOptionChanceSkill criticalSkill in xmlOption.CriticalSkills)
        {
            Skill? skill = SkillData.Instance.GetSkill(criticalSkill.Id, criticalSkill.Level);
            if (skill != null)
                activationSkills.Add(new OptionSkillHolder(skill, criticalSkill.Chance, OptionSkillType.Critical));
            else
                _logger.Error(nameof(OptionData) + ": Could not find skill " + criticalSkill.Id + "(" +
                    criticalSkill.Level + ") used by option " + id + ".");
        }

        Option option = new(id, effects.ToImmutableArray(), activeSkills.ToImmutableArray(),
            passiveSkills.ToImmutableArray(), activationSkills.ToImmutableArray());

        optionMap.Add(id, option);
    }

    public Option? GetOptions(int id) => id >= 0 && id < _options.Length ? _options[id] : null;
}