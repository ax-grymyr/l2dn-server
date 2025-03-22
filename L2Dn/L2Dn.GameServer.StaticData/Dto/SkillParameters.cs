using L2Dn.Collections;
using L2Dn.GameServer.Enums;
using L2Dn.GameServer.Handlers;
using L2Dn.GameServer.StaticData.Xml.Skills;

namespace L2Dn.GameServer.Dto;

internal sealed class SkillParameters
{
    public required int Id { get; init; }
    public required int Level { get; init; }
    public required int SubLevel { get; init; }
    public required string Name { get; init; }
    public required int? DisplayId { get; init; }
    public required int? DisplayLevel { get; init; }
    public required int? ReferenceId { get; init; }
    public ParameterSet<XmlSkillParameterType> Parameters { get; } = new();
    public Dictionary<SkillConditionScope, List<ISkillConditionBase>> Conditions { get; } = [];
    public Dictionary<SkillEffectScope, List<IAbstractEffect>> Effects { get; } = [];
}