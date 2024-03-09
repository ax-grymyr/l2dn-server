using L2Dn.GameServer.Geo;
using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Skills;
using L2Dn.GameServer.Model.Skills.Targets;
using L2Dn.GameServer.Utilities;

namespace L2Dn.GameServer.Handlers.TargetHandlers.AffectScopes;

/**
 * Point Blank affect scope implementation. Gathers targets in specific radius except initial target.
 * @author Nik
 */
public class PointBlank: IAffectScopeHandler
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
			if (affectObject != null)
			{
				if (c.isDead() && (skill.getAffectObject() != AffectObject.OBJECT_DEAD_NPC_BODY))
				{
					return false;
				}
				if (!affectObject.checkAffectedObject(creature, c))
				{
					return false;
				}
			}
			if (!GeoEngine.getInstance().canSeeTarget(target, c))
			{
				return false;
			}
			
			affected.incrementAndGet();
			return true;
		};
		
		// Check and add targets.
		if (skill.getTargetType() == TargetType.GROUND)
		{
			if (creature.isPlayable())
			{
				Location worldPosition = creature.getActingPlayer().getCurrentSkillWorldPosition();
				if (worldPosition != null)
				{
					World.getInstance().forEachVisibleObjectInRange<Creature>(creature, (int) (affectRange + creature.calculateDistance2D(worldPosition)), c =>
					{
						if (!c.isInsideRadius3D(worldPosition, affectRange))
						{
							return;
						}
						if (filter(c))
						{
							action((T)(WorldObject)c);
						}
					});
				}
			}
		}
		else
		{
			World.getInstance().forEachVisibleObjectInRange<Creature>(target, affectRange, c =>
			{
				if (filter(c))
				{
					action((T)(WorldObject)c);
				}
			});
		}
	}
	
	public AffectScope getAffectScopeType()
	{
		return AffectScope.POINT_BLANK;
	}
}
