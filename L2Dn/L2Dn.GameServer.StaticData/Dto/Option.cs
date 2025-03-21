using System.Collections.Immutable;
using L2Dn.GameServer.Handlers;
using L2Dn.GameServer.Templates;

namespace L2Dn.GameServer.Dto;

public sealed record Option(int Id, ImmutableArray<IAbstractEffect> Effects, ImmutableArray<Skill> ActiveSkills,
    ImmutableArray<Skill> PassiveSkills, ImmutableArray<OptionSkillHolder> ActivationSkills);