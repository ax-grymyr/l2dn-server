namespace L2Dn.GameServer.Model.Holders;

public sealed class EnchantStarHolder(int level, int expMax, int expOnFail, long feeAdena)
{
    public int getLevel() => level;
    public int getExpMax() => expMax;
    public int getExpOnFail() => expOnFail;
    public long getFeeAdena() => feeAdena;
}