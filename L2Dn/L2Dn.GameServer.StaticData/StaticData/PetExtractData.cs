using System.Collections.Frozen;
using L2Dn.GameServer.Dto;
using L2Dn.GameServer.StaticData.Xml.PetExtracts;
using L2Dn.GameServer.Utilities;
using NLog;

namespace L2Dn.GameServer.StaticData;

public sealed class PetExtractData
{
    private static readonly Logger _logger = LogManager.GetLogger(nameof(PetExtractData));

    // Key: PetId, PetLevel
    private FrozenDictionary<(int, int), PetExtractionHolder> _extractionData =
        FrozenDictionary<(int, int), PetExtractionHolder>.Empty;

    private PetExtractData()
    {
    }

    public static PetExtractData Instance { get; } = new();

    public void Load()
    {
        XmlPetExtractList xmlPetExtractList = XmlLoader.LoadXmlDocument<XmlPetExtractList>("PetExtractData.xml");

        _extractionData = xmlPetExtractList.Items.Select(x => new PetExtractionHolder(x.PetId, x.PetLevel, x.ExtractExp,
                x.ExtractItem, new ItemHolder(x.DefaultCostId, x.DefaultCostCount),
                new ItemHolder(x.ExtractCostId, x.ExtractCostCount))).
            ToFrozenDictionary(x => (x.PetId, x.PetLevel));

        _logger.Info($"{nameof(PetExtractData)}: Loaded {_extractionData.Count} pet extraction data.");
    }

    public PetExtractionHolder? GetExtraction(int petId, int petLevel) =>
        _extractionData.GetValueOrDefault((petId, petLevel));
}