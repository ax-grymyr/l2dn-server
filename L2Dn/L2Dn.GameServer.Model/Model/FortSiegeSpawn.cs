using L2Dn.GameServer.Model.Interfaces;

namespace L2Dn.GameServer.Model;

/**
 * Fort Siege Spawn.
 * @author xban1x
 */
public class FortSiegeSpawn: Location, IIdentifiable
{
    private readonly int _npcId;
    private readonly int _fortId;
    private readonly int _id;

    public FortSiegeSpawn(int fortId, int x, int y, int z, int heading, int npcId, int id): base(x, y, z, heading)
    {
        _fortId = fortId;
        _npcId = npcId;
        _id = id;
    }

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
