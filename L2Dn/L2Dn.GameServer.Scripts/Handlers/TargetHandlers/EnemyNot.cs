using L2Dn.GameServer.Geo;
using L2Dn.GameServer.Handlers;
using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Skills;
using L2Dn.GameServer.Model.Skills.Targets;
using L2Dn.GameServer.Network.Enums;
using L2Dn.Geometry;

namespace L2Dn.GameServer.Scripts.Handlers.TargetHandlers;

/**
 * Any friendly selected target. Works on dead targets or doors as well. Unable to force use.
 * @author Nik
 */
public class EnemyNot: ITargetTypeHandler
{
	public TargetType getTargetType()
	{
		return TargetType.ENEMY_NOT;
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

		Creature target = (Creature) selectedTarget;

		// You can always target yourself.
		if (creature == target)
		{
			return target;
		}

		if (!target.isAutoAttackable(creature))
		{
			// Check for cast range if character cannot move. TODO: char will start follow until within castrange, but if his moving is blocked by geodata, this msg will be sent.
			if (dontMove && (creature.Distance2D(target) > skill.getCastRange()))
			{
				if (sendMessage)
				{
					creature.sendPacket(SystemMessageId.THE_DISTANCE_IS_TOO_FAR_AND_SO_THE_CASTING_HAS_BEEN_CANCELLED);
				}
				return null;
			}

			if ((skill.isFlyType()) && !GeoEngine.getInstance().canMoveToTarget(creature.Location.Location3D, target.Location.Location3D, creature.getInstanceWorld()))
			{
				if (sendMessage)
				{
					creature.sendPacket(SystemMessageId.YOU_CANNOT_ATTACK_THE_TARGET);
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

		if (sendMessage)
		{
			creature.sendPacket(SystemMessageId.INVALID_TARGET);
		}

		return null;
	}
}