namespace L2Dn.GameServer.Dto;

public sealed record ElementalSpiritLevel(int Level, int Attack, int Defense, int CriticalRate, int CriticalDamage,
    long MaxExperience);