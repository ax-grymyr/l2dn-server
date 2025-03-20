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
 * Square affect scope implementation (actually more like a rectangle).
 * @author Nik
 */
public class Square: IAffectScopeHandler
{
	public void forEachAffected<T>(Creature creature, WorldObject target, Skill skill, Action<T> action)
		where T: WorldObject
	{
		IAffectObjectHandler? affectObject = AffectObjectHandler.getInstance().getHandler(skill.AffectObject);
		int squareStartAngle = skill.FanRange[1];
		int squareLength = skill.FanRange[2];
		int squareWidth = skill.FanRange[3];
		int radius = (int) Math.Sqrt((squareLength * squareLength) + (squareWidth * squareWidth));
		int affectLimit = skill.GetAffectLimit();

		int rectX = creature.getX();
		int rectY = creature.getY() - (squareWidth / 2);
		double heading = double.DegreesToRadians(squareStartAngle + HeadingUtil.ConvertHeadingToDegrees(creature.getHeading()));
		double cos = Math.Cos(-heading);
		double sin = Math.Sin(-heading);

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

			// Check if inside square.
			int xp = c.getX() - creature.getX();
			int yp = c.getY() - creature.getY();
			int xr = (int) ((creature.getX() + (xp * cos)) - (yp * sin));
			int yr = (int) (creature.getY() + (xp * sin) + (yp * cos));
			if ((xr > rectX) && (xr < (rectX + squareLength)) && (yr > rectY) && (yr < (rectY + squareWidth)))
			{
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
			}

			return false;
		};

		// Add object of origin since its skipped in the forEachVisibleObjectInRange method.
		if (filter(creature))
		{
			action((T)(WorldObject)creature);
		}

		// Check and add targets.
		World.getInstance().forEachVisibleObjectInRange<Creature>(creature, radius, c =>
		{
			if (filter(c))
			{
				action((T)(WorldObject)c);
			}
		});
	}

	public AffectScope getAffectScopeType()
	{
		return AffectScope.SQUARE;
	}
}