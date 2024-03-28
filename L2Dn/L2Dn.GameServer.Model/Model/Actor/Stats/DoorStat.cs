using L2Dn.GameServer.Model.Actor.Instances;

namespace L2Dn.GameServer.Model.Actor.Stats;

public class DoorStat: CreatureStat
{
    private int _upgradeHpRatio = 1;

    public DoorStat(Door activeChar): base(activeChar)
    {
    }

    public override Door getActiveChar()
    {
        return (Door)base.getActiveChar();
    }

    public override int getMaxHp()
    {
        return base.getMaxHp() * _upgradeHpRatio;
    }

    public void setUpgradeHpRatio(int ratio)
    {
        _upgradeHpRatio = ratio;
    }

    public int getUpgradeHpRatio()
    {
        return _upgradeHpRatio;
    }
}