namespace L2Dn.GameServer.Dto;

public sealed record PetSkillAcquireHolder(int SkillId, int SkillLevel, int RequiredLevel, int Evolve, ItemHolder? Item);