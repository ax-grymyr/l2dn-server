using L2Dn.GameServer.Handlers;
using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Actor.Instances;
using L2Dn.GameServer.Model.Skills;
using L2Dn.GameServer.Model.Skills.Targets;
using L2Dn.GameServer.Utilities;

namespace L2Dn.GameServer.Scripts.Handlers.TargetHandlers.AffectScopes;

/**
 * Static Object affect scope implementation. Used to detect hidden doors.
 * @author Nik
 */
public class StaticObjectScope: IAffectScopeHandler
{
	public void forEachAffected<T>(Creature creature, WorldObject target, Skill skill, Action<T> action)
		where T: WorldObject
	{
		IAffectObjectHandler affectObject = AffectObjectHandler.getInstance().getHandler(skill.getAffectObject());
		int affectRange = skill.getAffectRange();
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
			
			if (!c.isDoor() && !(c is StaticObject))
			{
				return false;
			}
			
			if ((affectObject != null) && !affectObject.checkAffectedObject(creature, c))
			{
				return false;
			}
			
			affected.incrementAndGet();
			return true;
		};
		
		// Add object of origin since its skipped in the forEachVisibleObjectInRange method.
		if (target.isCreature() && filter((Creature) target))
		{
			action((T)(WorldObject)target);
		}
		
		// Check and add targets.
		World.getInstance().forEachVisibleObjectInRange<Creature>(target, affectRange, c =>
		{
			if (filter(c))
			{
				action((T)(WorldObject)c);
			}
		});
	}
	
	public AffectScope getAffectScopeType()
	{
		return AffectScope.STATIC_OBJECT_SCOPE;
	}
}
