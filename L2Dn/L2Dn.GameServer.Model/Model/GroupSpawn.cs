using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Actor.Instances;
using L2Dn.GameServer.Model.Actor.Templates;
using L2Dn.Geometry;
using L2Dn.Utilities;

namespace L2Dn.GameServer.Model;

/**
 * @author littlecrow A special spawn implementation to spawn controllable mob
 */
public class GroupSpawn: Spawn
{
    private readonly NpcTemplate _template;

    public GroupSpawn(NpcTemplate mobTemplate): base(mobTemplate)
    {
        _template = mobTemplate;
        setAmount(1);
    }

    public Npc? doGroupSpawn()
    {
        try
        {
            if (_template.isType("Pet") || _template.isType("Minion"))
            {
                return null;
            }

            if (Location is { X: 0, Y: 0 }) // TODO: location must be verified when loading spawn data
            {
                if (getLocationId() == 0) // TODO: ????
                {
                    return null;
                }

                return null;
            }

            Location3D newLocation = Location.Location3D;

            Npc mob = new ControllableMob(_template);
            mob.setCurrentHpMp(mob.getMaxHp(), mob.getMaxMp());
            mob.setHeading(Location.Heading == -1 ? Rnd.get(61794) : Location.Heading);
            mob.setSpawn(this);
            mob.spawnMe(newLocation);
            return mob;
        }
        catch (Exception e)
        {
            LOGGER.Warn("NPC class not found: " + e);
            return null;
        }
    }
}