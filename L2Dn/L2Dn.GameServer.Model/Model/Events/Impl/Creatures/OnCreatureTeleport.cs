using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Events.Impl.Base;
using L2Dn.GameServer.Model.InstanceZones;
using L2Dn.Geometry;

namespace L2Dn.GameServer.Model.Events.Impl.Creatures;

/**
 * @author Nik
 */
public class OnCreatureTeleport(Creature creature, Location location, Instance? destInstance)
    : LocationEventBase
{
    public Creature getCreature()
    {
        return creature;
    }

    public Location Location => location;

    public Instance? getDestInstance()
    {
        return destInstance;
    }
}