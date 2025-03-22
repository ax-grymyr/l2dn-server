using System.Collections.Frozen;
using System.Collections.Immutable;
using System.Globalization;
using L2Dn.GameServer.Dto;
using L2Dn.GameServer.StaticData.Xml.Subjugation;
using L2Dn.GameServer.Utilities;
using NLog;

namespace L2Dn.GameServer.StaticData;

public sealed class SubjugationData
{
    private static readonly Logger _logger = LogManager.GetLogger(nameof(SubjugationData));
    private FrozenDictionary<int, SubjugationHolder> _subjugations = FrozenDictionary<int, SubjugationHolder>.Empty;

    private SubjugationData()
    {
    }

    public static SubjugationData Instance { get; } = new();

    public void Load()
    {
        XmlSubjugationDataList xmlSubjugationDataList =
            XmlLoader.LoadXmlDocument<XmlSubjugationDataList>("SubjugationData.xml");

        XmlSubjugationGachaList xmlSubjugationGachaList =
            XmlLoader.LoadXmlDocument<XmlSubjugationGachaList>("SubjugationGacha.xml");

        HashSet<int> categories = xmlSubjugationDataList.Items.Select(x => x.Category).
            Concat(xmlSubjugationGachaList.Items.Select(x => x.Category)).ToHashSet();

        _subjugations = categories.
            Select(category => Create(category, xmlSubjugationDataList, xmlSubjugationGachaList)).
            Where(x => x != null).Select(x => x!).
            ToFrozenDictionary(x => x.Category);

        _logger.Info($"{nameof(SubjugationData)}: Loaded {_subjugations.Count} items.");
    }

    public SubjugationHolder? GetSubjugation(int category) => _subjugations.GetValueOrDefault(category);

    private static SubjugationHolder? Create(int category, XmlSubjugationDataList xmlSubjugationDataList,
        XmlSubjugationGachaList xmlSubjugationGachaList)
    {
        int dataItemCount = xmlSubjugationDataList.Items.Count(x => x.Category == category);
        if (dataItemCount == 0)
        {
            _logger.Error($"{nameof(SubjugationData)}: Missing subjugation data for category {category}.");
            return null;
        }

        if (dataItemCount > 1)
        {
            _logger.Error($"{nameof(SubjugationData)}: Duplicated subjugation data for category {category}.");
            return null;
        }

        int gachaItemCount = xmlSubjugationGachaList.Items.Count(x => x.Category == category);
        if (gachaItemCount > 1)
        {
            _logger.Error($"{nameof(SubjugationData)}: Duplicated subjugation gacha for category {category}.");
            return null;
        }

        if (gachaItemCount == 0)
        {
            _logger.Error($"{nameof(SubjugationData)}: Missing subjugation gacha for category {category}.");
            return null;
        }

        XmlSubjugationDataItem xmlSubjugationDataItem =
            xmlSubjugationDataList.Items.Single(x => x.Category == category);

        XmlSubjugationGachaItem xmlSubjugationGachaItem =
            xmlSubjugationGachaList.Items.Single(x => x.Category == category);

        return new SubjugationHolder(category, ParseHotTimes(xmlSubjugationDataItem.HotTimes),
            xmlSubjugationDataItem.Npcs.ToFrozenDictionary(x => x.Id, x => x.Points),
            xmlSubjugationGachaItem.Items.ToFrozenDictionary(x => x.Id, x => x.Rate));
    }

    private static ImmutableArray<SubjugationHotTime> ParseHotTimes(string hotTimes)
    {
        if (string.IsNullOrWhiteSpace(hotTimes))
            return ImmutableArray<SubjugationHotTime>.Empty;

        return hotTimes.Split(';').Select(range =>
        {
            string[] parts = range.Split('-');
            return new SubjugationHotTime(int.Parse(parts[0], CultureInfo.InvariantCulture),
                int.Parse(parts[1], CultureInfo.InvariantCulture));
        }).ToImmutableArray();
    }
}