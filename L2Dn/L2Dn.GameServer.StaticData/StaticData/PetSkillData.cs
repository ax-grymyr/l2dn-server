using System.Collections.Frozen;
using L2Dn.GameServer.Model.Holders;
using L2Dn.GameServer.StaticData.Xml.PetSkills;
using L2Dn.GameServer.Templates;
using L2Dn.GameServer.Utilities;
using NLog;

namespace L2Dn.GameServer.StaticData;

public sealed class PetSkillData
{
    private static readonly Logger _logger = LogManager.GetLogger(nameof(PetSkillData));

    // Keys: NpcId, Skill hash
    // TODO: Inner dictionary can be just set of Skill objects
    private FrozenDictionary<int, FrozenDictionary<long, SkillHolder>> _skillTrees =
        FrozenDictionary<int, FrozenDictionary<long, SkillHolder>>.Empty;

    private PetSkillData()
    {
    }

    public static PetSkillData Instance { get; } = new();

    public void Load()
    {
        XmlPetSkillList xmlPetSkillList = XmlLoader.LoadXmlDocument<XmlPetSkillList>("PetSkillData.xml");

        _skillTrees = xmlPetSkillList.Skills.Where(x =>
            {
                if (x.SkillLevel == 0)
                    x.SkillLevel = 1;

                Skill? skill = SkillData.Instance.GetSkill(x.SkillId, x.SkillLevel);
                if (skill != null)
                    return true;

                _logger.Error($"{nameof(PetSkillData)}: Could not find skill with id {x.SkillId}, " +
                    $"level {x.SkillLevel} for NPC {x.NpcId}.");

                return false;

            }).
            GroupBy(x => x.NpcId).
            Select(g => KeyValuePair.Create(g.Key,
                g.Select(x => KeyValuePair.Create(Skill.GetSkillHashCode(x.SkillId, x.SkillLevel),
                    new SkillHolder(x.SkillId, x.SkillLevel))).ToFrozenDictionary())).
            ToFrozenDictionary();

        _logger.Info($"{nameof(PetSkillData)}: Loaded {_skillTrees.Count} skills.");
    }

    public FrozenDictionary<int, FrozenDictionary<long, SkillHolder>> SkillTrees => _skillTrees;

    public FrozenDictionary<long, SkillHolder> GetSkills(int npcId) =>
        _skillTrees.GetValueOrDefault(npcId, FrozenDictionary<long, SkillHolder>.Empty);
}