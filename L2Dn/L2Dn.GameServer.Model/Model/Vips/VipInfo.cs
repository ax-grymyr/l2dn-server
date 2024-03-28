namespace L2Dn.GameServer.Model.Vips;

public class VipInfo
{
    private readonly int _tier;
    private readonly long _pointsRequired;
    private readonly long _pointsDepreciated;
    private int _skill;

    public VipInfo(int tier, long pointsRequired, long pointsDepreciated)
    {
        _tier = tier;
        _pointsRequired = pointsRequired;
        _pointsDepreciated = pointsDepreciated;
    }

    public int getTier()
    {
        return _tier;
    }

    public long getPointsRequired()
    {
        return _pointsRequired;
    }

    public long getPointsDepreciated()
    {
        return _pointsDepreciated;
    }

    public int getSkill()
    {
        return _skill;
    }

    public void setSkill(int skill)
    {
        _skill = skill;
    }
}