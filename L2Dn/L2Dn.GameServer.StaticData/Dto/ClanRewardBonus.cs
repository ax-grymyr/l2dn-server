using L2Dn.GameServer.Enums;
using L2Dn.GameServer.Model.Holders;

namespace L2Dn.GameServer.Dto;

public sealed record ClanRewardBonus(ClanRewardType RewardType, int Level, int RequiredAmount, SkillHolder RewardSkill);