using L2Dn.GameServer.Enums;
using L2Dn.GameServer.InstanceManagers;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.InstanceZones;
using L2Dn.Geometry;
using L2Dn.Utilities;
using Config = L2Dn.GameServer.Configuration.Config;

namespace L2Dn.GameServer.Model;

public static class CreatureExtensions
{
    public static void teleToLocation(this Creature creature, Location location, Instance? instance, int randomOffset)
    {
        creature.teleToLocation(Config.Character.OFFSET_ON_TELEPORT_ENABLED && randomOffset > 0
            ? location with
            {
                X = location.X + Rnd.get(-randomOffset, randomOffset),
                Y = location.Y + Rnd.get(-randomOffset, randomOffset),
            }
            : location, instance);
    }

    public static void teleToLocation(this Creature creature, Location location, Instance? instance, bool randomOffset)
    {
        creature.teleToLocation(location, instance, randomOffset ? Config.Character.MAX_OFFSET_ON_TELEPORT : 0);
    }

    public static void teleToLocation(this Creature creature, Location location, int randomOffset = 0)
    {
        creature.teleToLocation(location, creature.getInstanceWorld(), randomOffset);
    }

    public static void teleToLocation(this Creature creature, Location location, bool randomOffset)
    {
        creature.teleToLocation(location, creature.getInstanceWorld(),
            randomOffset ? Config.Character.MAX_OFFSET_ON_TELEPORT : 0);
    }

    public static void teleToLocation(this Creature creature, Location3D location, Instance? instance,
        int randomOffset = 0)
    {
        creature.teleToLocation(new Location(location, creature.getHeading()), instance, randomOffset);
    }

    public static void teleToLocation(this Creature creature, Location3D location, Instance? instance,
        bool randomOffset)
    {
        creature.teleToLocation(new Location(location, creature.getHeading()), instance,
            randomOffset ? Config.Character.MAX_OFFSET_ON_TELEPORT : 0);
    }

    public static void teleToLocation(this Creature creature, Location3D location, int randomOffset = 0)
    {
        creature.teleToLocation(new Location(location, creature.getHeading()), creature.getInstanceWorld(),
            randomOffset);
    }

    public static void teleToLocation(this Creature creature, Location3D location, bool randomOffset)
    {
        creature.teleToLocation(new Location(location, creature.getHeading()), creature.getInstanceWorld(),
            randomOffset ? Config.Character.MAX_OFFSET_ON_TELEPORT : 0);
    }

    public static void teleToLocation(this Creature creature, TeleportWhereType teleportWhere)
    {
        creature.teleToLocation(teleportWhere, creature.getInstanceWorld());
    }

    public static void teleToLocation(this Creature creature, TeleportWhereType teleportWhere, Instance? instance)
    {
        creature.teleToLocation(MapRegionManager.GetTeleToLocation(creature, teleportWhere), instance, true);
    }
}