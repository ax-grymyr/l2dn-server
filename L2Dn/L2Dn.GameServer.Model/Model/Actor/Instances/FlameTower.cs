using L2Dn.GameServer.Enums;
using L2Dn.GameServer.InstanceManagers;
using L2Dn.GameServer.Model.Actor.Templates;
using L2Dn.GameServer.Model.Zones;

namespace L2Dn.GameServer.Model.Actor.Instances;

/**
 * Class for Flame Control Tower instance.
 * @author JIV
 */
public class FlameTower: Tower
{
    private int _upgradeLevel = 0;
    private List<int> _zoneList;

    public FlameTower(NpcTemplate template): base(template)
    {
        InstanceType = InstanceType.FlameTower;
    }

    public override bool doDie(Creature killer)
    {
        enableZones(false);
        return base.doDie(killer);
    }

    public override bool deleteMe()
    {
        enableZones(false);
        return base.deleteMe();
    }

    public void enableZones(bool value)
    {
        if ((_zoneList != null) && (_upgradeLevel != 0))
        {
            int maxIndex = _upgradeLevel * 2;
            for (int i = 0; i < maxIndex; i++)
            {
                ZoneType zone = ZoneManager.getInstance().getZoneById(_zoneList[i]);
                if (zone != null)
                {
                    zone.setEnabled(value);
                }
            }
        }
    }

    public void setUpgradeLevel(int level)
    {
        _upgradeLevel = level;
    }

    public void setZoneList(List<int> list)
    {
        _zoneList = list;
        enableZones(true);
    }
}