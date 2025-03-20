using L2Dn.GameServer.Enums;
using L2Dn.GameServer.Geo;
using L2Dn.GameServer.Handlers;
using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Skills;
using L2Dn.GameServer.Network.Enums;
using L2Dn.GameServer.Templates;
using L2Dn.Geometry;

namespace L2Dn.GameServer.Scripts.Handlers.TargetHandlers;

/**
 * Target dead monster.
 * @author Nik
 */
public class NpcBody: ITargetTypeHandler
{
	public TargetType getTargetType()
	{
		return TargetType.NPC_BODY;
	}

	public WorldObject? getTarget(Creature creature, WorldObject? selectedTarget, Skill skill, bool forceUse, bool dontMove, bool sendMessage)
	{
		if (selectedTarget == null)
		{
			return null;
		}

		if (!selectedTarget.isCreature())
		{
			return null;
		}

		if (!selectedTarget.isNpc() && !selectedTarget.isSummon())
		{
			if (sendMessage)
			{
				creature.sendPacket(SystemMessageId.INVALID_TARGET);
			}
			return null;
		}

		Creature target = (Creature) selectedTarget;
		if (target.isDead())
		{
			// Check for cast range if character cannot move. TODO: char will start follow until within castrange, but if his moving is blocked by geodata, this msg will be sent.
			if (dontMove && (creature.Distance2D(target) > skill.CastRange))
			{
				if (sendMessage)
				{
					creature.sendPacket(SystemMessageId.THE_DISTANCE_IS_TOO_FAR_AND_SO_THE_CASTING_HAS_BEEN_CANCELLED);
				}
				return null;
			}

			// Geodata check when character is within range.
			if (!GeoEngine.getInstance().canSeeTarget(creature, target))
			{
				if (sendMessage)
				{
					creature.sendPacket(SystemMessageId.CANNOT_SEE_TARGET);
				}
				return null;
			}
			return target;
		}

		// If target is not dead or not player/pet it will not even bother to walk within range, unlike Enemy target type.
		if (sendMessage)
		{
			creature.sendPacket(SystemMessageId.INVALID_TARGET);
		}
		return null;
	}
}