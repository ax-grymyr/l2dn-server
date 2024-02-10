using L2Dn.GameServer.Model.Interfaces;

namespace L2Dn.GameServer.Model;

public class TowerSpawn: IIdentifiable
{
    private readonly int _npcId;
    private readonly Location _location;
    private List<int> _zoneList = null;
    private int _upgradeLevel = 0;

    public TowerSpawn(int npcId, Location location)
    {
        _location = location;
        _npcId = npcId;
    }

    public TowerSpawn(int npcId, Location location, List<int> zoneList)
    {
        _location = location;
        _npcId = npcId;
        _zoneList = zoneList;
    }

    /**
     * Gets the NPC ID.
     * @return the NPC ID
     */
    public int getId()
    {
        return _npcId;
    }

    public Location getLocation()
    {
        return _location;
    }

    public List<int> getZoneList()
    {
        return _zoneList;
    }

    public void setUpgradeLevel(int level)
    {
        _upgradeLevel = level;
    }

    public int getUpgradeLevel()
    {
        return _upgradeLevel;
    }
}