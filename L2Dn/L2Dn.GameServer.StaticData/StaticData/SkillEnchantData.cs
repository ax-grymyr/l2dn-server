using System.Collections.Frozen;
using L2Dn.GameServer.Dto;
using L2Dn.GameServer.StaticData.Xml.SkillEnchant;
using L2Dn.GameServer.Templates;
using L2Dn.GameServer.Utilities;
using NLog;

namespace L2Dn.GameServer.StaticData;

public sealed class SkillEnchantData
{
    private static readonly Logger _logger = LogManager.GetLogger(nameof(SkillEnchantData));

    private FrozenDictionary<int, SkillEnchantHolder> _skillEnchantMap =
        FrozenDictionary<int, SkillEnchantHolder>.Empty;

    private FrozenDictionary<int, EnchantStarHolder> _enchantStarMap = FrozenDictionary<int, EnchantStarHolder>.Empty;
    private FrozenDictionary<int, int> _chanceEnchantMap = FrozenDictionary<int, int>.Empty;

    private FrozenDictionary<int, FrozenDictionary<int, EnchantItemExpHolder>> _enchantItemMap =
        FrozenDictionary<int, FrozenDictionary<int, EnchantItemExpHolder>>.Empty;

    private SkillEnchantData()
    {
    }

    public static SkillEnchantData Instance { get; } = new();

    public void Load()
    {
        XmlSkillEnchantData xmlSkillEnchantData =
            XmlLoader.LoadXmlDocument<XmlSkillEnchantData>("SkillEnchantData.xml");

        _skillEnchantMap = xmlSkillEnchantData.Skills.
            Select(x => new SkillEnchantHolder(x.Id, x.StarLevel, x.MaxEnchantLevel)).
            ToFrozenDictionary(x => x.Id);

        _enchantStarMap = xmlSkillEnchantData.Stars.
            Select(x => new EnchantStarHolder(x.Level, x.ExpMax, x.ExpOnFail, x.FeeAdena)).
            ToFrozenDictionary(x => x.Level);

        _chanceEnchantMap = xmlSkillEnchantData.Chances.ToFrozenDictionary(x => x.EnchantLevel, x => x.Chance);

        _enchantItemMap = xmlSkillEnchantData.ItemPoints.Select(x => KeyValuePair.Create(x.Level,
                x.Items.Select(y => new EnchantItemExpHolder(y.Id, y.Exp, y.StarLevel)).
                    ToFrozenDictionary(x => x.Id))).
            ToFrozenDictionary();

        _logger.Info($"{nameof(SkillEnchantData)}: Loaded {_enchantStarMap.Count} star levels.");
        _logger.Info($"{nameof(SkillEnchantData)}: Loaded {_enchantItemMap.Count} enchant items.");
        _logger.Info($"{nameof(SkillEnchantData)}: Loaded {_skillEnchantMap.Count} skill enchants.");
    }

    public EnchantStarHolder? GetEnchantStar(int level) => _enchantStarMap.GetValueOrDefault(level);
    public SkillEnchantHolder? GetSkillEnchant(int id) => _skillEnchantMap.GetValueOrDefault(id);

    public EnchantItemExpHolder? GetEnchantItem(int level, int id) =>
        _enchantItemMap.GetValueOrDefault(level)?.GetValueOrDefault(id);

    public FrozenDictionary<int, EnchantItemExpHolder> GetEnchantItem(int level) =>
        _enchantItemMap.GetValueOrDefault(level, FrozenDictionary<int, EnchantItemExpHolder>.Empty);

    public int GetChanceEnchantMap(Skill skill)
    {
        int enchantLevel = skill.SubLevel == 0 ? 1 : skill.SubLevel + 1 - 1000;
        if (enchantLevel > GetSkillEnchant(skill.Id)?.MaxEnchantLevel)
            return 0;

        return _chanceEnchantMap.GetValueOrDefault(enchantLevel);
    }
}