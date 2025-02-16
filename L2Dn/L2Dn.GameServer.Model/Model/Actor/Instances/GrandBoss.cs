using L2Dn.GameServer.Enums;
using L2Dn.GameServer.Model.Actor.Templates;

namespace L2Dn.GameServer.Model.Actor.Instances;

/**
 * This class manages all Grand Bosses.
 * @version $Revision: 1.0.0.0 $ $Date: 2006/06/16 $
 */
public class GrandBoss: Monster
{
    private bool _useRaidCurse = true;

    /**
     * Constructor for GrandBoss. This represent all grandbosses.
     * @param template NpcTemplate of the instance
     */
    public GrandBoss(NpcTemplate template): base(template)
    {
        InstanceType = InstanceType.GrandBoss;
        setIsRaid(true);
        setLethalable(false);
    }

    public override void onSpawn()
    {
        setRandomWalking(false);
        base.onSpawn();
    }

    public override int getVitalityPoints(int level, double exp, bool isBoss)
    {
        return -base.getVitalityPoints(level, exp, isBoss);
    }

    public override bool useVitalityRate()
    {
        return false;
    }

    public void setUseRaidCurse(bool value)
    {
        _useRaidCurse = value;
    }

    public override bool giveRaidCurse()
    {
        return _useRaidCurse;
    }
}