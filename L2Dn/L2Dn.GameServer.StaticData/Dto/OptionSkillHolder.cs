using L2Dn.GameServer.Enums;
using L2Dn.GameServer.Templates;

namespace L2Dn.GameServer.Dto;

public sealed record OptionSkillHolder(Skill Skill, double Chance, OptionSkillType SkillType);