namespace L2Dn.GameServer.Model;

public class CropProcure: SeedProduction
{
    private readonly int _rewardType;

    public CropProcure(int id, long amount, int type, long startAmount, long price)
        : base(id, amount, price, startAmount)
    {
        _rewardType = type;
    }

    public int getReward()
    {
        return _rewardType;
    }
}