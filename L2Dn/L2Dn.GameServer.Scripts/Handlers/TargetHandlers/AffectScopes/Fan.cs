using L2Dn.GameServer.Enums;
using L2Dn.GameServer.Geo;
using L2Dn.GameServer.Handlers;
using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Skills;
using L2Dn.GameServer.Utilities;
using L2Dn.Geometry;

namespace L2Dn.GameServer.Scripts.Handlers.TargetHandlers.AffectScopes;

/**
 * Fan affect scope implementation. Gathers objects in a certain angle of circular area around yourself (including origin itself).
 * @author Nik
 */
public class Fan: IAffectScopeHandler
{
	public void forEachAffected<T>(Creature creature, WorldObject target, Skill skill, Action<T> action)
		where T: WorldObject
	{
		IAffectObjectHandler? affectObject = AffectObjectHandler.getInstance().getHandler(skill.AffectObject);
		double headingAngle = HeadingUtil.ConvertHeadingToDegrees(creature.getHeading());
		int fanStartAngle = skill.FanRange[1];
		int fanRadius = skill.FanRange[2];
		int fanAngle = skill.FanRange[3];
		double fanHalfAngle = fanAngle / 2.0; // Half left and half right.
		int affectLimit = skill.GetAffectLimit();
		// Target checks.
		TargetType targetType = skill.TargetType;
		AtomicInteger affected = new AtomicInteger(0);
		Predicate<Creature> filter = c =>
		{
			if ((affectLimit > 0) && (affected.get() >= affectLimit))
			{
				return false;
			}
			if (c.isDead() && (targetType != TargetType.NPC_BODY) && (targetType != TargetType.PC_BODY))
			{
				return false;
			}

			double angle = new Location2D(creature.getX(), creature.getY()).AngleDegreesTo(new Location2D(c.getX(), c.getY()));
			if (Math.Abs(angle - (headingAngle + fanStartAngle)) > fanHalfAngle)
			{
				return false;
			}
			if ((c != target) && (affectObject != null) && !affectObject.checkAffectedObject(creature, c))
			{
				return false;
			}
			if (!GeoEngine.getInstance().canSeeTarget(creature, c))
			{
				return false;
			}

			affected.incrementAndGet();
			return true;
		};

		// Add object of origin since its skipped in the forEachVisibleObjectInRange method.
		if (filter(creature))
		{
			action((T)(WorldObject)creature);
		}

		// Check and add targets.
		World.getInstance().forEachVisibleObjectInRange<Creature>(creature, fanRadius, c =>
		{
			if (filter(c))
			{
				action((T)(WorldObject)c);
			}
		});
	}

	public AffectScope getAffectScopeType()
	{
		return AffectScope.FAN;
	}
}