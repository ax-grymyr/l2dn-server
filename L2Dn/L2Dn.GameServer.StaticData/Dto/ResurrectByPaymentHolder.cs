namespace L2Dn.GameServer.Dto;

public sealed class ResurrectByPaymentHolder(int time, int amount, double percent)
{
    public int getTime() => time;
    public int getAmount() => amount;
    public double getResurrectPercent() => percent;
}