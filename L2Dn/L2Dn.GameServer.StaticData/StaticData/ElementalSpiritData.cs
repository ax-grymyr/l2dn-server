using System.Collections.Frozen;
using System.Collections.Immutable;
using L2Dn.GameServer.Dto;
using L2Dn.GameServer.StaticData.Xml.ElementalSpirits;
using L2Dn.GameServer.Utilities;
using L2Dn.Model.Enums;
using NLog;

namespace L2Dn.GameServer.StaticData;

public sealed class ElementalSpiritData
{
    private static readonly Logger _logger = LogManager.GetLogger(nameof(ElementalSpiritData));

    public const float FragmentXpConsume = 50000.0f;
    public const int TalentInitFee = 50000;

    // TODO load from xml or from config
    public static readonly ImmutableArray<int> ExtractFees = [100000, 200000, 300000, 600000, 1500000];

    private FrozenDictionary<(ElementalType, byte), ElementalSpiritTemplateHolder> _spiritData =
        FrozenDictionary<(ElementalType, byte), ElementalSpiritTemplateHolder>.Empty;

    private ElementalSpiritData()
    {
    }

    public static ElementalSpiritData Instance { get; } = new();

    public void Load()
    {
        XmlElementalSpiritList document = XmlLoader.LoadXmlDocument<XmlElementalSpiritList>("ElementalSpiritData.xml");

        Dictionary<(ElementalType, byte), ElementalSpiritTemplateHolder> spiritData = [];
        foreach (XmlElementalSpirit xmlElementalSpirit in document.Spirits)
        {
            ElementalType type = (ElementalType)xmlElementalSpirit.Type;
            byte stage = xmlElementalSpirit.Stage;

            ImmutableArray<ElementalSpiritLevel> levels = xmlElementalSpirit.Levels.Select(x
                => new ElementalSpiritLevel(x.Level, x.Attack, x.Defense, x.CriticalRate, x.CriticalDamage,
                    x.MaxExperience)).ToImmutableArray();

            ImmutableArray<ItemHolder> itemsToEvolve =
                xmlElementalSpirit.ItemsToEvolve.Select(x => new ItemHolder(x.Id, x.Count)).ToImmutableArray();

            ImmutableArray<ElementalSpiritAbsorbItemHolder> absorbItems = xmlElementalSpirit.AbsorbItems.
                Select(x => new ElementalSpiritAbsorbItemHolder(x.Id, x.Experience)).ToImmutableArray();

            ElementalSpiritTemplateHolder template = new(type, stage, xmlElementalSpirit.NpcId,
                xmlElementalSpirit.ExtractItem, xmlElementalSpirit.MaxCharacteristics, levels, itemsToEvolve,
                absorbItems);

            if (!spiritData.TryAdd((type, stage), template))
                _logger.Info($"{nameof(ElementalSpiritData)}: Duplicated data for spirit type {type} and stage {stage}.");
        }

        _spiritData = spiritData.ToFrozenDictionary();

        _logger.Info($"{nameof(ElementalSpiritData)}: Loaded {_spiritData.Count} elemental spirit templates.");
    }

    public ElementalSpiritTemplateHolder? GetSpirit(ElementalType type, byte stage) =>
        _spiritData.GetValueOrDefault((type, stage));
}