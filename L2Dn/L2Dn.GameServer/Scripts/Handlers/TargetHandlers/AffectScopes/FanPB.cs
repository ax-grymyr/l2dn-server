using L2Dn.GameServer.Geo;
using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Skills;
using L2Dn.GameServer.Model.Skills.Targets;
using L2Dn.GameServer.Utilities;

namespace L2Dn.GameServer.Handlers.TargetHandlers.AffectScopes;

/**
 * Fan point blank affect scope implementation. Gathers objects in a certain angle of circular area around yourself without taking target into account.
 * @author Nik
 */
public class FanPB: IAffectScopeHandler
{
	public void forEachAffected<T>(Creature creature, WorldObject target, Skill skill, Action<T> action)
		where T: WorldObject
	{
		IAffectObjectHandler affectObject = AffectObjectHandler.getInstance().getHandler(skill.getAffectObject());
		double headingAngle = Util.convertHeadingToDegree(creature.getHeading());
		int fanStartAngle = skill.getFanRange()[1];
		int fanRadius = skill.getFanRange()[2];
		int fanAngle = skill.getFanRange()[3];
		double fanHalfAngle = fanAngle / 2; // Half left and half right.
		int affectLimit = skill.getAffectLimit();
		
		// Target checks.
		AtomicInteger affected = new AtomicInteger(0);
		Predicate<Creature> filter = c =>
		{
			if ((affectLimit > 0) && (affected.get() >= affectLimit))
			{
				return false;
			}
			if (c.isDead())
			{
				return false;
			}
			if (Math.Abs(Util.calculateAngleFrom(creature, c) - (headingAngle + fanStartAngle)) > fanHalfAngle)
			{
				return false;
			}
			if ((affectObject != null) && !affectObject.checkAffectedObject(creature, c))
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
		return AffectScope.FAN_PB;
	}
}
