namespace L2Dn.GameServer.Model.Holders;

public sealed class ElementalSpiritLevel(
    int level,
    int attack,
    int defense,
    int criticalRate,
    int criticalDamage,
    long maxExperience)
{
    public int Level => level;
    public int Attack => attack;
    public int Defense => defense;
    public int CriticalRate => criticalRate;
    public int CriticalDamage => criticalDamage;
    public long MaxExperience => maxExperience;
}