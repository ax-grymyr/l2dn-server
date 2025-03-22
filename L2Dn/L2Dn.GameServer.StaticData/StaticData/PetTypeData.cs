using System.Collections.Frozen;
using L2Dn.GameServer.Model.Holders;
using L2Dn.GameServer.StaticData.Xml.PetTypes;
using L2Dn.GameServer.Utilities;
using NLog;

namespace L2Dn.GameServer.StaticData;

public sealed class PetTypeData
{
    private static readonly Logger LOGGER = LogManager.GetLogger(nameof(PetTypeData));

    private FrozenDictionary<int, SkillHolder> _skills = FrozenDictionary<int, SkillHolder>.Empty;
    private FrozenDictionary<int, string> _names = FrozenDictionary<int, string>.Empty;

    private PetTypeData()
    {
    }

    public static PetTypeData Instance { get; } = new();

    public void Load()
    {
        XmlPetTypeList xmlPetTypeList = XmlLoader.LoadXmlDocument<XmlPetTypeList>("PetTypes.xml");

        _skills = xmlPetTypeList.Pets.Select(x => KeyValuePair.Create(x.Id, new SkillHolder(x.SkillId, x.SkillLevel))).
            ToFrozenDictionary();

        _names = xmlPetTypeList.Pets.Select(x => KeyValuePair.Create(x.Id, x.Name)).ToFrozenDictionary();

        LOGGER.Info($"{nameof(PetTypeData)}: Loaded {_skills.Count} pet types.");
    }

    public SkillHolder? GetSkillByName(string name)
    {
        foreach (KeyValuePair<int, string> entry in _names)
        {
            if (name.StartsWith(entry.Value))
                return _skills.GetValueOrDefault(entry.Key);
        }

        return null;
    }

    public int GetIdByName(string name)
    {
        foreach (KeyValuePair<int, string> entry in _names)
        {
            if (name.EndsWith(entry.Value))
                return entry.Key;
        }

        return 0;
    }

    public string? GetNamePrefix(int id) => _names.GetValueOrDefault(id);
    public string GetRandomName() => _names.First(e => e.Key > 100).Value;

    public KeyValuePair<int, SkillHolder> GetRandomSkill() =>
        _skills.First(e => e.Value.getSkillId() > 0); // TODO: not random
}