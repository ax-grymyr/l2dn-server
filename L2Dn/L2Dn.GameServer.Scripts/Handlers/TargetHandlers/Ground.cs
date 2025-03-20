using L2Dn.GameServer.Enums;
using L2Dn.GameServer.Geo;
using L2Dn.GameServer.Handlers;
using L2Dn.GameServer.InstanceManagers;
using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Skills;
using L2Dn.GameServer.Model.Zones;
using L2Dn.GameServer.Network.Enums;
using L2Dn.Geometry;

namespace L2Dn.GameServer.Scripts.Handlers.TargetHandlers;

/**
 * Target ground location. Returns yourself if your current skill's ground location meets the conditions.
 * @author Nik
 */
public class Ground: ITargetTypeHandler
{
    public TargetType getTargetType()
    {
        return TargetType.GROUND;
    }

    public WorldObject? getTarget(Creature creature, WorldObject? selectedTarget, Skill skill, bool forceUse,
        bool dontMove, bool sendMessage)
    {
        Player? player = creature.getActingPlayer();
        if (creature.isPlayer() && player != null)
        {
            Location3D? worldPosition = player.getCurrentSkillWorldPosition();
            if (worldPosition != null)
            {
                if (dontMove && !creature.IsInsideRadius2D(worldPosition.Value.Location2D,
                        skill.CastRange + creature.getTemplate().getCollisionRadius()))
                {
                    return null;
                }

                if (!GeoEngine.getInstance().canSeeTarget(creature, worldPosition.Value))
                {
                    if (sendMessage)
                    {
                        creature.sendPacket(SystemMessageId.CANNOT_SEE_TARGET);
                    }

                    return null;
                }

                ZoneRegion? zoneRegion = ZoneManager.Instance.getRegion(creature.Location.Location2D);
                if (skill.IsBad && !creature.isInInstance() && zoneRegion != null &&
                    !zoneRegion.checkEffectRangeInsidePeaceZone(skill,
                        worldPosition.Value.X, worldPosition.Value.Y, worldPosition.Value.Z))
                {
                    if (sendMessage)
                    {
                        creature.sendPacket(SystemMessageId.YOU_CANNOT_USE_SKILLS_THAT_MAY_HARM_OTHER_PLAYERS_IN_HERE);
                    }

                    return null;
                }

                return creature; // Return yourself to know that your ground location is legit.
            }
        }

        return null;
    }
}