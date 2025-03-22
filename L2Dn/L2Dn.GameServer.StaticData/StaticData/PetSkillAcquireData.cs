using System.Collections.Frozen;
using System.Collections.Immutable;
using L2Dn.GameServer.Dto;
using L2Dn.GameServer.StaticData.Xml.PetSkillAcquire;
using L2Dn.GameServer.Utilities;
using NLog;

namespace L2Dn.GameServer.StaticData;

public sealed class PetSkillAcquireData
{
    private static readonly Logger _logger = LogManager.GetLogger(nameof(PetSkillAcquireData));

    private FrozenDictionary<int, ImmutableArray<PetSkillAcquireHolder>> _skills =
        FrozenDictionary<int, ImmutableArray<PetSkillAcquireHolder>>.Empty;

    private PetSkillAcquireData()
    {
    }

    public static PetSkillAcquireData Instance { get; } = new();

    public void Load()
    {
        XmlPetSkillAcquireData xmlPetSkillAcquireData =
            XmlLoader.LoadXmlDocument<XmlPetSkillAcquireData>("PetAcquireList.xml");

        _skills = xmlPetSkillAcquireData.Pets.Select(x => KeyValuePair.Create(x.Type, x.Skills.Select(y =>
            new PetSkillAcquireHolder(y.Id, y.Level, y.RequiredLevel, y.Evolve, y.ItemId >= 0
                ? new ItemHolder(y.ItemId, y.ItemAmount)
                : null)).ToImmutableArray())).ToFrozenDictionary();

        _logger.Info($"{nameof(PetSkillAcquireData)}: Loaded {_skills.Count} pet skills.");
    }

    public ImmutableArray<PetSkillAcquireHolder> GetSkills(int type) =>
        _skills.GetValueOrDefault(type, ImmutableArray<PetSkillAcquireHolder>.Empty);

    public FrozenDictionary<int, ImmutableArray<PetSkillAcquireHolder>> Skills => _skills;

    public static int GetSpecialSkillByType(int petType)
    {
        return petType switch
        {
            15 => 49001,
            14 => 49011,
            12 => 49021,
            13 => 49031,
            17 => 49041,
            16 => 49051,
            _ => throw new ArgumentException($"Unexpected value: {petType}"),
        };
    }
}