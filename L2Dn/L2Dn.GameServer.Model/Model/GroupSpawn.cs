using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Actor.Instances;
using L2Dn.GameServer.Model.Actor.Templates;
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

    public Npc doGroupSpawn()
    {
        try
        {
            if (_template.isType("Pet") || _template.isType("Minion"))
            {
                return null;
            }

            int newlocx = 0;
            int newlocy = 0;
            int newlocz = 0;
            if ((Location.getX() == 0) && (Location.getY() == 0))
            {
                if (getLocationId() == 0)
                {
                    return null;
                }

                return null;
            }

            newlocx = Location.getX();
            newlocy = Location.getY();
            newlocz = Location.getZ();

            Npc mob = new ControllableMob(_template);
            mob.setCurrentHpMp(mob.getMaxHp(), mob.getMaxMp());
            mob.setHeading(Location.getHeading() == -1 ? Rnd.get(61794) : Location.getHeading());
            mob.setSpawn(this);
            mob.spawnMe(newlocx, newlocy, newlocz);
            return mob;
        }
        catch (Exception e)
        {
            LOGGER.Warn("NPC class not found: " + e);
            return null;
        }
    }
}