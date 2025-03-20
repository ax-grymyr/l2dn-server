using L2Dn.GameServer.Dto;
using L2Dn.GameServer.Model.Interfaces;
using L2Dn.Geometry;

namespace L2Dn.GameServer.Model;

public class TowerSpawn: IIdentifiable
{
    private readonly int _npcId;
    private readonly Location3D _location;
    private readonly List<int> _zoneList;
    private int _upgradeLevel;

    public TowerSpawn(int npcId, Location3D location)
    {
        _location = location;
        _npcId = npcId;
        _zoneList = [];
    }

    public TowerSpawn(int npcId, Location3D location, List<int> zoneList)
    {
        _location = location;
        _npcId = npcId;
        _zoneList = zoneList;
    }

    /**
     * Gets the NPC ID.
     * @return the NPC ID
     */
    public int Id => _npcId;

    public Location3D getLocation()
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