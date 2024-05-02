using L2Dn.GameServer.Model.Interfaces;
using L2Dn.Geometry;

namespace L2Dn.GameServer.Model;

/**
 * Fort Siege Spawn.
 * @author xban1x
 */
public class FortSiegeSpawn: IIdentifiable
{
    private readonly LocationHeading _location;
    private readonly int _npcId;
    private readonly int _fortId;
    private readonly int _id;

    public FortSiegeSpawn(int fortId, LocationHeading location, int npcId, int id)
    {
        _location = location;
        _fortId = fortId;
        _npcId = npcId;
        _id = id;
    }

    public LocationHeading Location => _location;

    public int getFortId()
    {
        return _fortId;
    }

    /**
     * Gets the NPC ID.
     * @return the NPC ID
     */
    public int getId()
    {
        return _npcId;
    }

    public int getMessageId()
    {
        return _id;
    }
}