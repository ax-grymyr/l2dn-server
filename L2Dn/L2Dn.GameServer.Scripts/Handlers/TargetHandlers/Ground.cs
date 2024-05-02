using L2Dn.GameServer.Geo;
using L2Dn.GameServer.Handlers;
using L2Dn.GameServer.InstanceManagers;
using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Skills;
using L2Dn.GameServer.Model.Skills.Targets;
using L2Dn.GameServer.Model.Zones;
using L2Dn.GameServer.Network.Enums;

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
	
	public WorldObject getTarget(Creature creature, WorldObject selectedTarget, Skill skill, bool forceUse, bool dontMove, bool sendMessage)
	{
		if (creature.isPlayer())
		{
			Location worldPosition = creature.getActingPlayer().getCurrentSkillWorldPosition();
			if (worldPosition != null)
			{
				if (dontMove && !creature.isInsideRadius2D(worldPosition.getX(), worldPosition.getY(), skill.getCastRange() + creature.getTemplate().getCollisionRadius()))
				{
					return null;
				}
				
				if (!GeoEngine.getInstance().canSeeTarget(creature, worldPosition))
				{
					if (sendMessage)
					{
						creature.sendPacket(SystemMessageId.CANNOT_SEE_TARGET);
					}
					return null;
				}

				ZoneRegion? zoneRegion = ZoneManager.getInstance().getRegion(creature.getLocation().ToLocation2D());
				if (skill.isBad() && !creature.isInInstance() && !zoneRegion.checkEffectRangeInsidePeaceZone(skill, worldPosition.getX(), worldPosition.getY(), worldPosition.getZ()))
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